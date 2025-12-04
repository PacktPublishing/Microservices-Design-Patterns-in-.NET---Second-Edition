using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration)
    .AddPolly();
    
var authenticationProviderKey = builder.Configuration["AuthenticationProviderKey"]; ;
builder.Services
    .AddAuthentication()
    .AddJwtBearer(authenticationProviderKey, x =>
    {
        x.Authority = "https://localhost:5001";
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });


var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
await app.UseOcelot();

app.UseHttpsRedirection();


app.Run();

