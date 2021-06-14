using APICatalogo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiCatalogo.Repository
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario> BuscarPermissoesUsuario (Guid idLogin);
        Task<Usuario> BuscarUsuarioPeloEmail(string email);
    }
}
