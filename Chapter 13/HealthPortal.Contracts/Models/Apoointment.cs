namespace HealthPortal.Contracts.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }
}

public class Doctor
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Specialty { get; set; }
}

public class Patient
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
}
