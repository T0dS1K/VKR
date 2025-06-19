using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using VKRServer.BackgroundFunc;
using VKRServer.DataBase;
using VKRServer.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<TempDataCon>();
builder.Services.AddSingleton<ITempKeyGenerator, GeneratorTempKeys>();
builder.Services.AddSingleton<IHostedService, ConcatenationAttendance>();
builder.Services.AddSingleton(provider =>(IHostedService)provider.GetRequiredService<ITempKeyGenerator>());
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateAudience = false,
        RequireExpirationTime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = JWT =>
        {
            var Claims = JWT.Principal?.Claims;

            if (Claims != null)
            {
                var ID   = Claims.FirstOrDefault(z => z.Type == "ID");
                var Role = Claims.FirstOrDefault(z => z.Type == "Role");
                var Type = Claims.FirstOrDefault(z => z.Type == "Type");

                if (ID == null || Role == null || Type == null || (Type.Value != "Access" && Type.Value != "Refresh"))
                {
                    JWT.Fail("Invalid Token");
                }
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorizationBuilder()

    .AddPolicy("Access", policy => policy
        .RequireClaim("Type", "Access"))

    .AddPolicy("AccessUser", policy => policy
        .RequireClaim("Type", "Access")
        .RequireClaim("Role", "User"))

    .AddPolicy("AccessModer", policy => policy
        .RequireClaim("Type", "Access")
        .RequireClaim("Role", "Moder"))

    .AddPolicy("AccessAdmin", policy => policy
        .RequireClaim("Type", "Access")
        .RequireClaim("Role", "Admin"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseRouting();
//app.UseHttpsRedirection(); HTTP
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
