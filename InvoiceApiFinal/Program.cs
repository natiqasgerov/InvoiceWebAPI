using FluentValidation.AspNetCore;
using FluentValidation;
using InvoiceApiFinal.Data;
using InvoiceApiFinal.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using InvoiceApiFinal.Services.TokenServices;
using InvoiceApiFinal;
using InvoiceApiFinal.Providers;
using InvoiceApiFinal.Services.CustomerServices;
using InvoiceApiFinal.DTOs.Validations;
using InvoiceApiFinal.Services.InvoiceServices;
using InvoiceApiFinal.Services.RowServices;
using InvoiceApiFinal.Services.ReportServices;
using Serilog.Events;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{NewLine}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddDbContext<InvoiceDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Context"));
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidation>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService,InvoiceService>();
builder.Services.AddScoped<IRowService,RowService>();
builder.Services.AddScoped<IReportService,ReportService>();


builder.Services.AuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddSwagger();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x => x.EnablePersistAuthorization());
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
