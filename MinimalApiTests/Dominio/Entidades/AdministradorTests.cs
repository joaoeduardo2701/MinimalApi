
using MinimalApi.Dominio.Entidades;

namespace MinimalApiTests.Dominio.Entidades
{
    [TestClass]
    public class AdministradorTests
    {
        [TestMethod]
        public void Deve_Testar_Get_Set_Propriedades()
        {
            // Arrange

            var adm = new Administrador();

            // Act

            adm.Id = 1;
            adm.Email = "teste@teste.com";
            adm.Senha = "senha123";
            adm.Perfil = "Admin";

            // Assert

            Assert.AreEqual(1, adm.Id);
            Assert.AreEqual("teste@teste.com", adm.Email);
            Assert.AreEqual("senha123", adm.Senha);
            Assert.AreEqual("Admin", adm.Perfil);
        }
    }
}
