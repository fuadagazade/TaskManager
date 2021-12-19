using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Validators;
using TaskManager.Core.ViewModels;
using TaskManager.Data;
using TaskManager.Service;

var builder = WebApplication.CreateBuilder(args);

DataContext.GenerateDatabase();

builder.Services.AddTransient<IAppData, AppData>();
builder.Services.AddTransient<IOrganizationService, OrganizationService>();
builder.Services.AddTransient<IUserService, UserService>();


builder.Services.AddControllers().AddFluentValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Task Manager API"
    });

    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string path = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    s.IncludeXmlComments(path);
});

builder.Services.AddTransient<IValidator<Organization>, OrganizationValidator>();
builder.Services.AddTransient<IValidator<User>, UserValidator>();
builder.Services.AddTransient<IValidator<OrganizationRegistration>, OrganizationRegistrationValidator>();


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
app.MapControllers();
app.Run();
