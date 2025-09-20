using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enums;
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

#region Administradores

app.MapPost("/administradores/login", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    return Results.Ok(administradorServico.Todos(pagina));
}).WithTags("Administrador");

static ErrosDeValidacao validacaoAdmDTO(AdministradorDTO administradorDTO)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagem = new List<string>()
    };

    if (string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagem.Add("O email do administrador é obrigatório.");

    if (string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagem.Add("A senha do administrador é obrigatória.");

    if (administradorDTO.Perfil == null)
        validacao.Mensagem.Add("O perfil do administrador é obrigatório.");

    return validacao;
}

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = validacaoAdmDTO(administradorDTO);

    if (validacao.Mensagem.Count > 0)
        return Results.BadRequest(validacao);

    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil?.ToString()!
    };

    administradorServico.Incluir(administrador);

    return Results.Created($"/administrador/{administrador.Id}", administrador);
}).WithTags("Administrador");

app.MapGet("/administradores/{id}", ([FromRoute] int id, [FromServices] IAdministradorServico administradorServico) =>
{
    var adms = new List<AdministradorModelView>();

    var administrador = administradorServico.BuscarPorId(id);

    if (administrador == null)
        return Results.NotFound();

    adms.Add(new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });

    return Results.Ok(adms);
}).WithTags("Administrador");

app.MapGet("/administradores", ([FromQuery] int? pagina, [FromServices] IAdministradorServico administradorServico) =>
{
    var adms = new List<AdministradorModelView>();

    var administradores = administradorServico.Todos(pagina);

    foreach (var adm in administradores)
    {
        adms.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }

    return Results.Ok(adms);
}).WithTags("Administrador");

#endregion

#region Veiculos

ErrosDeValidacao validacaoDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagem = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagem.Add("O nome do veículo é obrigatório.");

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagem.Add("O marca do veículo é obrigatório.");

    if (veiculoDTO.Ano == null)
        validacao.Mensagem.Add("O ano do veículo é obrigatório.");
    
    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosServico veiculosServico) =>
{
    var validacao = validacaoDTO(veiculoDTO);

    if (validacao.Mensagem.Count > 0)
        return Results.BadRequest(validacao);

    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculosServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculo");

app.MapGet("/veiculos", ([FromQuery] int? pagina, [FromServices] IVeiculosServico veiculosServico) =>
{
    var veiculos = veiculosServico.Todos(pagina);

    return Results.Ok(veiculos);
}).WithTags("Veiculo");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, [FromServices] IVeiculosServico veiculosServico) =>
{
    var veiculo = veiculosServico.BuscaPorId(id);

    if (veiculo == null)
        return Results.NotFound();

    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, [FromServices] IVeiculosServico veiculoServico) =>
{

    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
        return Results.NotFound();

    var validacao = validacaoDTO(veiculoDTO);

    if (validacao.Mensagem.Count > 0)
        return Results.BadRequest(validacao);

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, [FromServices] IVeiculosServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
        return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}).WithTags("Veiculo");

#endregion

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
