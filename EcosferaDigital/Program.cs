

using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OracleDbConnection");


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddHttpClient();

//builder.Services.AddSingleton<MLModel>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    Process.Start(new ProcessStartInfo("cmd", $"/c start http://localhost:5105/swagger/index.html") { CreateNoWindow = true });

}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();


public partial class Program { }