using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// For cross origin cookies we need a secure cookie (https only) and samesite None in dev mode.
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.OnAppendCookie = cookieContext =>
    {
        cookieContext.CookieOptions.Secure = true;
        cookieContext.CookieOptions.SameSite = builder.Environment.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Strict;
    };
});
// Configure cookie. By default ASP sends a redirect to the login page. This makes no sense with a SPA,
// so we send 401 if we try to request a protected route.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return System.Threading.Tasks.Task.CompletedTask;
        };
    });
// Set CORS only in development mode for vue devserver.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueDevserver",
        builder => builder.SetIsOriginAllowed(origin => new System.Uri(origin).IsLoopback)
            .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowVueDevserver");
}
app.UseHttpsRedirection();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// Serve SPA in request pipeline if no controller match request.
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();