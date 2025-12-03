using System.Text.Json;
using AppointmentsApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsApi.Services;
public class RebuildEventsService(EventStoreDbContext _eventStoreDbContext)
{
    public async Task<IEnumerable<T>> RebuildStateAsync<T>(Guid aggregateId) where T : class, new()
    {
        var events = await _eventStoreDbContext.Events
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.EventTimestamp)
            .ToListAsync();

        var result = new List<T>();

        foreach (var @event in events)
        {
            var deserializedEvent = JsonSerializer.Deserialize<T>(@event.Payload);
            if (deserializedEvent != null)
            {
                result.Add(deserializedEvent);
            }
        }

        return result;
    }
}
