using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PDMS.Configurations;
using PDMS.Domain.Entities;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using PDMS.Domain.Abstractions;
using PDMS.Infrastructure.Extensions;
using PDMS.Infrastructure.Persistence;
using PDMS.Models;
using PDMS.Security;
using PDMS.Services;
using PDMS.Services.Interface;

var userProfileDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
var staticRootDir = Path.Combine(userProfileDir, "PDMS");
if (!Directory.Exists(staticRootDir)) {
    Directory.CreateDirectory(staticRootDir);
}

var defaultDirs = new List<string>() {
    "images"
};
foreach (var dir in defaultDirs) {
    var fullPath = Path.Combine(staticRootDir, dir);
    if (!Directory.Exists(fullPath)) {
        Directory.CreateDirectory(fullPath);
    }
}

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddCors(
    options => {
        options.AddPolicy(
            "allowAll", x => {
                x.WithOrigins("http://localhost:5238", "https://localhost:44318", "https://localhost:7036")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
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
builder.Services.AddScoped<IPasswordHasher<User>, BCryptPasswordHasher>();
builder.Services.AddIdentity<User, Role>(
        options => {
            options.SignIn.RequireConfirmedEmail = true;
            options.ClaimsIdentity.UserNameClaimType = JwtRegisteredClaimNames.Sub;
        }
    )
    .AddEntityFrameworkStores<PdmsDbContext>()
    .AddUserStore<UserStore<User, Role, PdmsDbContext, string, IdentityUserClaim<string>,
        UserRole, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
    .AddRoleStore<RoleStore<Role, PdmsDbContext, string, UserRole, IdentityRoleClaim<string>>
    >()
    .AddDefaultTokenProviders();
builder.Services.AddControllers()
    .AddJsonOptions(
        options => {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        }
    );
builder.Services.Configure<ApiBehaviorOptions>(
    options => {
        options.InvalidModelStateResponseFactory = ValidationError.GenerateErrorResponse;
    }
);
builder.Services
    .AddAuthentication(
        op => {
            op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    )
    .AddJwtBearer(
        options => {
            options.Events = new JwtBearerEvents() {
                OnMessageReceived = context => {
                    context.Token = context.Request.Cookies["accessToken"];
                    return Task.CompletedTask;
                }
            };
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        }
    );
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerModule();
builder.Services.AddTransient<IUserService, UserService>();

var app = builder.Build();
app.UseCors("allowAll");
if (app.Environment.IsDevelopment()) {
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
app.UseStaticFiles(
    new StaticFileOptions() {
        FileProvider = new PhysicalFileProvider(staticRootDir),
        RequestPath = "/files",
        OnPrepareResponse = ctx => {
            ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            ctx.Context.Response.Headers.Append(
                "Access-Control-Allow-Headers",
                "Origin, X-Requested-With, Content-Type, Accept"
            );
        },
    }
);
app.Run();


void AddSerilog(IConfiguration configuration) {
    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}