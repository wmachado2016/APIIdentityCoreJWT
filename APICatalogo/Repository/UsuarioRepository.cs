using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiCatalogo.Repository
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository (AppDbContext contexto) : base(contexto)
        {
        }

        public async Task<Usuario> BuscarPermissoesUsuario(Guid idLogin)
        {
            var result = await _context.Usuario.AsNoTracking()
                                .Where(x => x.LoginId == idLogin)
                                .Include(x => x.Perfil).ThenInclude(x => x.Permissao)
                                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<Usuario> BuscarUsuarioPeloEmail (string email)
        {
            return  await _context.Usuario.AsNoTracking().Where(x => x.Email ==  email).FirstOrDefaultAsync();

        }
    }
}
