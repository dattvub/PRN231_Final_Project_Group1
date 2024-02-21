using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PDMS.Configurations;
using PDMS.Domain.Entities;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using PDMS.Domain.Abstractions;
using PDMS.Infrastructure.Extensions;
using PDMS.Infrastructure.Persistence;
using PDMS.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddCors(
    options => {
        options.AddPolicy(
            "allowAll", x => {
                x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }
        );
    }
);
builder.Services.AddHttpContextAccessor();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Add serilog
AddSerilog(builder.Configuration);
// Add database
builder.Services.AddDatabaseModule<PdmsDbContext>(builder.Configuration);
builder.Services.AddScoped<IPdmsDbContext, PdmsDbContext>();
builder.Services.AddInfrastructure(builder.Configuration);
//builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));

//Jwt
builder.Services
    .AddAuthentication(op =>
    {
        op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddIdentity<User, Role>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.ClaimsIdentity.UserNameClaimType = JwtRegisteredClaimNames.Sub;
})
    .AddEntityFrameworkStores<PdmsDbContext>()
    .AddUserStore<UserStore<User, Role, PdmsDbContext, string, IdentityUserClaim<string>,
        UserRole, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
    .AddRoleStore<RoleStore<Role, PdmsDbContext, string, UserRole, IdentityRoleClaim<string>>
    >()
    .AddDefaultTokenProviders();
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(
    options => {
        options.InvalidModelStateResponseFactory = ValidationError.GenerateErrorResponse;
    }
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerModule();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseCors();
    app.UseApplicationSwagger();
}
IHostApplicationLifetime lifetime = app.Lifetime;
IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
app.UseApplicationDatabase<PdmsDbContext>(serviceProvider, app.Environment);
app.UseApplicationIdentity(serviceProvider);
app.UseSerilogRequestLogging();
app.UseHsts();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


void AddSerilog(IConfiguration configuration)
{
    Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(configuration)

    .CreateLogger();
}
