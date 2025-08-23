// Models/PatientTableEntity.cs
using Azure;
using Azure.Data.Tables;

public class PatientTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "Patients";
    public string RowKey { get; set; } = Guid.NewGuid().ToString("N");
    public string FirstName { get; set; } = default!;
    public string LastName  { get; set; } = default!;
    public string Email     { get; set; } = default!;
    public DateTime DateOfBirth { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}

