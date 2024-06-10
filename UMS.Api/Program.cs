using Data;
using EmailLibrary.Interfaces;
using EmailLibrary.Models.EmailModels;
using EmailLibrary.Repositories;
using LicenseLibrary;
using LoggerLibrary.Interface;
using LoggerLibrary.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using UMS.Api.Interfaces;
using UMS.Api.Models;
using UMS.Api.Repositories;
using UMS.Api.Services;

//Build Version             Date            Developed by                    Change
//1.0.0                     2024/01/03                                      Initial Development

var builder = WebApplication.CreateBuilder(args);

//Register the configurations
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer schema (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

//Add Auto mapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//Add the database service
builder.Services.AddDbContextFactory<UmsContext>(opts =>
opts.UseSqlServer(builder.Configuration.GetConnectionString("UmsDB")));

//Add emailDB service
builder.Services.AddDbContext<EmailDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("EmailDB"));
});

builder.Services.AddSingleton<ILogService, LogService>();
builder.Services.AddSingleton<ILicense, LicenseService>();

builder.Services.AddScoped<IActivityLogsRepository, ActivityLogsRepository>();
builder.Services.AddScoped<IActivityLogsService, ActivityLogsService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IPasswordPolicyRepository, PasswordPolicyRepository>();
builder.Services.AddScoped<IPasswordPolicyService, PasswordPolicyService>();
builder.Services.AddScoped<IPermissionConfigurationRepository, PermissionConfigurationRepository>();
builder.Services.AddScoped<IPlatformConfigurationRepository, PlatformConfigurationRepository>();
builder.Services.AddScoped<IRoleConfigurationRepository, RoleConfigurationRepository>();
builder.Services.AddScoped<ISystemTokensRepository, SystemTokensRepository>();
builder.Services.AddScoped<ISystemTokensService, SystemTokensService>();
builder.Services.AddScoped<IUserRepository, UsersRepository>();
builder.Services.AddScoped<IUserService, UsersService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IRepository, Repository<UmsContext>>();
builder.Services.AddScoped<IEmailService, EmailRepository>();

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<UserRolePlatformService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
        .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddCors(opt => opt.AddPolicy("CorsPolicy", c =>
{
    c.AllowAnyOrigin()
       .AllowAnyHeader()
       .AllowAnyMethod();
}));

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI(c =>
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UMS API V1.0"));

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();