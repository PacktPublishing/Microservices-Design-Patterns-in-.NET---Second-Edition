using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentsApi.Data;
using AppointmentsApi.Commands.CreateAppoinments;
using MediatR;
using AppointmentsApi.Queries.GetAppointmentById;
using AppointmentsApi.Queries.GetAppointments;
using AppointmentsApi.Queries.GetAppointmentsByPatientId;
using AppointmentsApi.Models.Events;

namespace AppointmentsApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController(AppointmentContext _context, IMediator _mediator) : ControllerBase
{

    // GET: api/Appointments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
    {
        return await _mediator.Send(new GetAppointmentsQuery());
    }

    // GET: api/Appointments/GetAppointmentsByPatientId/5
    [HttpGet("GetAppointmentsByPatientId/{patientId}")]
    public async Task<ActionResult<IEnumerable<AppointmentByPatientId>>> GetAppointmentsByPatientId(string patientId)
    {
        return await _mediator.Send(new GetAppointmentByPatientIdQuery(patientId)); 
    }

    // GET: api/Appointments/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetAppointment(string id)
    {
        var appointmentDetails = await _mediator.Send(new GetAppointmentByIdQuery(id));
        return appointmentDetails == null ? NotFound() : Ok(appointmentDetails);
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
    public async Task<ActionResult<Appointment>> PostAppointment(CreateAppointmentCommand createAppointmentCommand)
    {
        var appointment = await _mediator.Send(createAppointmentCommand);
        var correlationId = Guid.NewGuid();

        await _mediator.Publish(new AppointmentCreatedEvent{
            CorrelationId = correlationId,
            AppointmentId = appointment.AppointmentId,
            AppointmentDate = appointment.Slot.Start,
            PatientId = appointment.PatientId,
            Location = appointment.Location,
            Slot = appointment.Slot
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
    