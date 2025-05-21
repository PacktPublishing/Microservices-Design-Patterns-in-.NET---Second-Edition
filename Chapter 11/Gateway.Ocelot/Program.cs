using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration)
    .AddPolly();


var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
await app.UseOcelot();

app.UseHttpsRedirection();


app.Run();

