using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using TaskManager.Core.Configs;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Validators;
using TaskManager.Core.ViewModels;
using TaskManager.Data;
using TaskManager.Service;
using Task = TaskManager.Core.Models.Task;

var builder = WebApplication.CreateBuilder(args);

DataContext.GenerateDatabase();

#region Mappers

builder.Services.Configure<TokenConfig>(builder.Configuration.GetSection("Token")).AddSingleton(sp => sp.GetRequiredService<IOptions<TokenConfig>>().Value);

#endregion

#region Services

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IAppData, AppData>();
builder.Services.AddTransient<IOrganizationService, OrganizationService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ITaskService, TaskService>();

#endregion


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("Token").GetSection("Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("Token").GetSection("Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Token").GetSection("Key").Value))
    };
});

builder.Services.AddCors();

builder.Services.AddControllers().AddFluentValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Task Manager API"
    });
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    s.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                    },
                    new string[] { }
                }
                });

    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string path = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    s.IncludeXmlComments(path);
});

#region Validators

builder.Services.AddTransient<IValidator<Organization>, OrganizationValidator>();
builder.Services.AddTransient<IValidator<User>, UserValidator>();
builder.Services.AddTransient<IValidator<OrganizationRegistration>, OrganizationRegistrationValidator>();
builder.Services.AddTransient<IValidator<OrganizationUpdate>, OrganizationUpdateValidator>();
builder.Services.AddTransient<IValidator<PasswordUpdate>, PasswordUpdateValidator>();
builder.Services.AddTransient<IValidator<Task>, TaskValidator>();
builder.Services.AddTransient<IValidator<Assigment>, AssigmentValidator>();

#endregion

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "docs/{documentname}/api.json";
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/docs/v1/api.json", $"Task Manager API V1");
        c.RoutePrefix = "documentation";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors(options =>
{
    options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().Build();
});
app.UseAuthorization();
app.MapControllers();
app.Run();
