using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentsApi.Data;
using AppointmentsApi.Services;
using Grpc.Net.Client;
using AppointmentsApi.Protos;
using MassTransit;
using AppointmentsApi.Models.Messages;

namespace AppointmentsApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController(AppointmentContext context, PatientsApiClient patientsApiClient, DoctorsApiClient doctorsApiClient,
IPublishEndpoint publishEndpoint) : ControllerBase
{
    private readonly AppointmentContext _context = context;
    private readonly PatientsApiClient _patientsApiClient = patientsApiClient;
    private readonly DoctorsApiClient _doctorsApiClient = doctorsApiClient;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    // GET: api/Appointments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
    {
        return await _context.Appointments.ToListAsync();
    }

    // GET: api/Appointments/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetAppointment(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
        {
            return NotFound();
        }

        // Use the clients to fetch patient and doctor details
        var patient = await _patientsApiClient.GetPatientAsync(id);
        var doctor = await _doctorsApiClient.GetDoctorAsync(id);

        // Use GRPC service to retrieve document information on patient
        using var channel = GrpcChannel.ForAddress("http://localhost:5170");
        var client = new DocumentService.DocumentServiceClient(channel);
        var documents = await client.GetAllAsync(new PatientId{ Id = patient.PatientId.ToString()});

        // Combine data and return response
        AppointmentDetails appointmentDetails = new AppointmentDetails(
        
            id,
            patient,
            doctor, 
            appointment.Slot.Start,
            appointment.Slot.End,
            appointment.Location.RoomNumber,
            appointment.Location.Building,
            documents
        );

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
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        await _publishEndpoint.Publish<AppointmentCreated>(new
        {
            appointment.AppointmentId,
            appointment.PatientId,
            appointment.DoctorId,
            appointment.Slot.Start,
            DateTime.UtcNow,
            MessageId = appointment.AppointmentId
        });

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
