using ApiCatalogo.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogo.Filters
{
    public class ApiLoggingFilter : IActionFilter
    {
        private readonly ILogger<ApiLoggingFilter> _logger;
        private readonly IUnitOfWork _context;
        private readonly IUser _user;
        public ApiLoggingFilter (ILogger<ApiLoggingFilter> logger, IUnitOfWork context, IUser user)
        {
            _logger = logger;
            _context = context;
            _user = user;
        }

        //executa antes da Action
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (_user.IsAuthenticated())
            {
                var userAuth = _user.GetUserId();
                //var userPermissoes = _context.UsuarioRepository.BuscarPermissoesUsuario(userAuth);
                var claims = _user.GetClaimsIdentity();

                _logger.LogInformation("### Executando -> OnActionExecuting");
                _logger.LogInformation("###################################################");
                _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
                _logger.LogInformation($"ModelState : {context.ModelState.IsValid}");
                _logger.LogInformation("###################################################");

                //if (!claims.Any(c => c.Type == "Admin" && c.Value.Contains("Ativar")))
                //{
                //    context.Result = new StatusCodeResult(403);
                //}
            }
            else
            {
                context.Result = new StatusCodeResult(401);
                return;
            }



        }
        //executa depois da Action
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("### Executando -> OnActionExecuted");
            _logger.LogInformation("###################################################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation("###################################################");
        }
    }
}
