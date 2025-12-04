namespace HealthcareDashboard.UI.Models;

public class PatientDashboardModel
{
    public PatientDto? Patient { get; set; }
    public List<MedicalRecordDto>? MedicalHistory { get; set; }
    public List<AppointmentDto>? Appointments { get; set; }
    public List<BillingDetailDto>? BillingDetails { get; set; }
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

public class BillingDetailDto
{
    public Guid BillingDetailId { get; set; }
    public Guid PatientId { get; set; }
    public decimal Amount { get; set; }
    public DateTime BillingDate { get; set; }
}

public class MedicalRecordDto{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Diagnosis { get; set; }
    public DateTime RecordDate { get; set; }
};
