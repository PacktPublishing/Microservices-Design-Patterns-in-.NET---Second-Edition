using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentsApi.Models;
using AppointmentsApi.Services;
using Grpc.Net.Client;
using AppointmentsApi.Protos;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using MassTransit;
using AppointmentsApi.Models.Messages;


namespace AppointmentsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController(AppointmentContext _context, PatientsApiClient _patientsApiClient, DoctorsApiClient _doctorsApiClient,
    IConfiguration _configuration, IDistributedCache _cache, IPublishEndpoint _publishEndpoint) : ControllerBase
    {

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            return await _context.Appointments.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(Guid id)
        {
            var cacheKey = $"appointment-details:{id}";
            var cached = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                var cachedDetails = JsonSerializer.Deserialize<AppointmentDetails>(cached);
                return Ok(cachedDetails);
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            var patient = await _patientsApiClient.GetPatientAsync(id);
            var doctor = await _doctorsApiClient.GetDoctorAsync(id);

            using var channel = GrpcChannel.ForAddress(_configuration["GrpcEndpoints:DocumentService"]);
            var client = new DocumentService.DocumentServiceClient(channel);
            var documents = await client.GetAllAsync(new PatientId { Id = patient.PatientId.ToString() });

            var appointmentDetails = new AppointmentDetails(
                id,
                patient,
                doctor,
                appointment.Slot.Start,
                appointment.Slot.End,
                appointment.Location.RoomNumber,
                appointment.Location.Building,
                documents
            );

            var serialized = JsonSerializer.Serialize(appointmentDetails);
            await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // TTL
            });

            return Ok(appointmentDetails);
        }


        // PUT: api/Appointments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(Guid id, Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return BadRequest();
            }

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Appointments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            _context.Appointments.Add(appointment);

            AppointmentCreated appointmentCreatedEvent = new()
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentDate = appointment.Slot.Start,
                Timestamp = DateTime.UtcNow,
                MessageId = appointment.AppointmentId
            };

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventType = nameof(AppointmentCreated),
                Payload = JsonSerializer.Serialize(appointmentCreatedEvent),
                OccurredOnUtc = DateTime.UtcNow,
                Processed = false
            };

            _context.OutboxMessages.Add(outboxMessage);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return CreatedAtAction("GetAppointment", new { id = appointment.AppointmentId }, appointment);
        }


        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentExists(Guid id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
    public record AppointmentDetails(
        Guid AppointmentId,
        Patient Patient,
        Doctor Doctor,
        DateTime StartTime,
        DateTime EndTime,
        string RoomNumber,
        string Building,
        DocumentList Documents
    );
}
