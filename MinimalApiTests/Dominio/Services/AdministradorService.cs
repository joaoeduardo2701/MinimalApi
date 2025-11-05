using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura;

namespace MinimalApiTests.Dominio.Services
{
    [TestClass]
    public class AdministradorServiceTests
    {
        private MinimalApiDbContext CriaContextoDeTeste()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            return new MinimalApiDbContext(configuration);
        }

        [TestMethod]
        public void Deve_Salvar_Administrador()
        {
            // Arrange1
            var adm = new Administrador
            {
                Id = 1,
                Email = "teste@teste.com",
                Senha = "senha123",
                Perfil = "Admin"
            };

            var context = CriaContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores;");

            var administradorService = new AdministradorServico(context);

            // Act 
            administradorService.Incluir(adm);

            // Assert

            Assert.AreEqual(1, administradorService.Todos(1).Count());
        }
    }
}