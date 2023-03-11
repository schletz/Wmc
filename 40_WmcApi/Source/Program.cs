using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spengernews.Application.Infrastructure;
using WmcApi.Dto;
using WmcApi.HolidayCalendar;

var builder = WebApplication.CreateBuilder(args);
// *************************************************************************************************
// BUILDER CONFIGURATION
// *************************************************************************************************

// Database********** ******************************************************************************
// Read the sql server connection string from appsettings.json located at
// ConnectionStrings -> Default.
builder.Services.AddDbContext<WmcApiContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(DtoMappingProfile));
builder.Services.AddSingleton(provider => new CalendarService(2000, 2100));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    });
}

// *************************************************************************************************
// APP
// *************************************************************************************************
var app = builder.Build();
// Creating the database.
using (var scope = app.Services.CreateScope())
{
    using (var db = scope.ServiceProvider.GetRequiredService<WmcApiContext>())
    {
        db.CreateDatabase(isDevelopment: true);
        //db.CreateDatabase(isDevelopment: app.Environment.IsDevelopment());
    }
}
app.MapControllers();
app.Run();