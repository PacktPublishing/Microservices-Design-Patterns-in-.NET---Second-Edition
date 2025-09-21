using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class AddPatientActivity
{
    [Function(nameof(AddPatientActivity))]
    public async Task Run([ActivityTrigger] Patient patient, FunctionContext ctx)
    {
        var logger = ctx.GetLogger(nameof(AddPatientActivity));
        TableClient client = TableStorage.GetClient("Patients");
        var entity = new PatientTableEntity
        {
            FirstName = patient.FirstName,
            LastName  = patient.LastName,
            Email     = patient.Email,
            DateOfBirth = patient.DateOfBirth
        };
        await client.AddEntityAsync(entity);
        logger.LogInformation("Patient added: {Email}", patient.Email);
    }
}
