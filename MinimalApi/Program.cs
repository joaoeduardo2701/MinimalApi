using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<MinimalApiDbContext>();

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

builder.Services.AddScoped<IVeiculosServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MinimalApiDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDTO) != null)
        return Results.Ok("Login realizado com sucesso!");
    else
        return Results.Unauthorized(); 
}).WithTags("Administrador");

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosServico veiculosServico) =>
{
    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculosServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculo");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculosServico veiculosServico) =>
{
    var veiculos = veiculosServico.Todos(pagina);

    return Results.Ok(veiculos);
}).WithTags("Veiculo");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculosServico veiculosServico) =>
{
    var veiculo = veiculosServico.BuscaPorId(id);

    if (veiculo == null)
        return Results.NotFound();

    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculosServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
        return Results.NotFound();

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculosServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
        return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}).WithTags("Veiculo");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
