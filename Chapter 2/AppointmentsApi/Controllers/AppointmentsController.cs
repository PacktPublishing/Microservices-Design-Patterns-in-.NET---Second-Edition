using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentsApi.Models;

namespace AppointmentsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController(AppointmentContext _context) : ControllerBase
    {
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

            return Ok(appointment);
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
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(CreateAppointmentRequest request)
        {
            try
            {
                var timeSlot = new TimeSlot(request.StartTime, request.EndTime);
                var location = new Location(request.RoomNumber, request.Building);
                var appointment = new Appointment(
                    request.PatientId,
                    request.DoctorId,
                    timeSlot,
                    location,
                    request.Purpose
                );

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAppointment", new { id = appointment.AppointmentId }, appointment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Appointments/5/reschedule
        [HttpPut("{id}/reschedule")]
        public async Task<IActionResult> RescheduleAppointment(Guid id, RescheduleRequest request)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            try
            {
                var newSlot = new TimeSlot(request.NewStartTime, request.NewEndTime);
                appointment.Reschedule(newSlot);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Appointments/5/confirm
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmAppointment(Guid id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            try
            {
                appointment.Confirm();
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Appointments/5/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(Guid id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            try
            {
                appointment.Cancel();
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Appointments/5/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteAppointment(Guid id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            try
            {
                appointment.Complete();
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
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

    // DTOs for API requests
    public record CreateAppointmentRequest(
        Guid PatientId,
        Guid DoctorId,
        DateTime StartTime,
        DateTime EndTime,
        string RoomNumber,
        string Building,
        string Purpose
    );

    public record RescheduleRequest(
        DateTime NewStartTime,
        DateTime NewEndTime
    );
}
