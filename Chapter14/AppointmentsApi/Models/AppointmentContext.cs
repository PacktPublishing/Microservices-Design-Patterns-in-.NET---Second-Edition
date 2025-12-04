using AppointmentsApi.Models;
using Microsoft.EntityFrameworkCore;

public class AppointmentContext : DbContext
{
    public AppointmentContext(DbContextOptions<AppointmentContext> options)
        : base(options)
    {
    }

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = default!;
}
