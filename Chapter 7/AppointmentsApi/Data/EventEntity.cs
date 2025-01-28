using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentsApi.Data;

public class EventEntity
{
    [Key]
    public Guid EventId { get; set; }
    public Guid AggregateId { get; set; }
    public string EventType { get; set; }
    public DateTime EventTimestamp { get; set; }
    [Column(TypeName = "jsonb")] 
    public string Payload { get; set; }
}