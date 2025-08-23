using Azure.Data.Tables;

public static class TableStorage
{
    public static TableClient GetClient(string tableName)
    {
        var conn = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                  ?? "UseDevelopmentStorage=true";
        var service = new TableServiceClient(conn);
        var client  = service.GetTableClient(tableName);
        client.CreateIfNotExists();
        return client;
    }
}
