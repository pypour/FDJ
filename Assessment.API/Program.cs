using Assessment.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCorsDefaults(builder.Configuration)
    .AddLog(builder.Configuration)
    .AddBusinessServices(builder.Configuration);

builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 50 * 1024 * 1024);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exception => { });

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>(LogLevel.Information);

app.UseHttpsRedirection();


app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
