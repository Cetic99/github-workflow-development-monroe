using CMSMock.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var certPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "certs", "public_key.pem");
var privateCertPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "certs", "private_key.pem");
var publicKeyPem = File.ReadAllText(certPath);
var privateKeyPem = File.ReadAllText(privateCertPath);
builder.Services.AddSingleton(new RequestVerifier(publicKeyPem, privateKeyPem));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
