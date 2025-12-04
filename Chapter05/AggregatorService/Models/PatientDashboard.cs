using System;
using AggregatorService.Services;

namespace AggregatorService.Models;

public class PatientDashboard
{
    public PatientDto? Patient { get; set; }
    public IEnumerable<MedicalRecordDto>? MedicalHistory { get; set; }
    public IEnumerable<AppointmentDto>? Appointments { get; set; }
    public IEnumerable<BillingDetailDto>? BillingDetails { get; set; }
}

public class PatientDto
{
    public Guid Id { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
}

public class AppointmentDto
{
    public Guid Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Doctor { get; set; } = string.Empty;
}

