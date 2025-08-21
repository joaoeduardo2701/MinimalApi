using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Infraestrutura;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MinimalApiDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))
    );
});

var app = builder.Build();

app.MapPost("/login", (LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "1234")
        return Results.Ok("Login realizado com sucesso!");
    else
        return Results.Unauthorized(); 
});

app.Run();
