using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura;

namespace MinimalApi.Dominio.Servicos
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly MinimalApiDbContext _contexto;
        public AdministradorServico(MinimalApiDbContext contexto)
        {
            _contexto = contexto;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();

            return adm;            
        }

        public Administrador Incluir(Administrador administrador)
        {
            _contexto.Administradores.Add(new Administrador
            {
                Email = administrador.Email,
                Senha = administrador.Senha,
                Perfil = administrador.Perfil
            });

            _contexto.SaveChanges();

            return administrador;
        }

        public List<Administrador> Todos(int? pagina)
        {
            var query = _contexto.Administradores.AsQueryable();

            int itensPorPagina = 10;

            if (pagina != null)
                query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);

            return query.ToList();
        }

        public Administrador? BuscarPorId(int id)
        {
            return _contexto.Administradores.Where(a => a.Id == id).FirstOrDefault();
        }
    }
}
