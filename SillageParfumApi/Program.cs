using Microsoft.EntityFrameworkCore;
using SillageParfumApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// 1. Agregar servicios al contenedor.
builder.Services.AddControllers();

// Conectamos el ApplicationDbContext usando SQL Server y la cadena de conexión
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Estas dos líneas son las que activan la generación de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. Configurar el pipeline de peticiones HTTP.
// Aquí le decimos: "Si estamos desarrollando, muestra la interfaz de Swagger"
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("PermitirTodo");

app.MapControllers();

app.Run();