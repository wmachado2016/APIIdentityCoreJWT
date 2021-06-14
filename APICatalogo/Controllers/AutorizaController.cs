using ApiCatalogo.DTOs;
using ApiCatalogo.Filters;
using ApiCatalogo.Repository;
using APICatalogo.Models;
using AutoMapper;
using DevIO.Api.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AutorizaController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;


        public AutorizaController (UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration, IUnitOfWork context, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "AutorizaController ::  Acessado em  : "
               + DateTime.Now.ToLongDateString();
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody]UsuarioDTO model)
        {

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = model.Password == model.ConfirmPassword
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.SignInAsync(user, false);

            GerenciarUsuarioAutenticado(user);

            return Ok(new
            {
                success = true,
                data = GeraToken(model),
            });
        }

        [HttpPost("login")]        
        public async Task<ActionResult> Login([FromBody] UsuarioLogadoDTO userLog)
        {
            //throw new Exception("Erro ao fazer login!");

            //verifica se o modelo é válido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            }

            //verifica as credenciais do usuário e retorna um valor
            var result = await _signInManager.PasswordSignInAsync(userLog.Email,
                userLog.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var authUser = new UsuarioDTO()
                {
                    Password = userLog.Password,
                    Email = userLog.Email,
                    ConfirmPassword = userLog.Password
                };

                return Ok(new
                {
                    success = true,
                    data = GeraToken(authUser),
                });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login Inválido....");
                return BadRequest(ModelState);
            }
        }

        private LoginResponseViewModel GeraToken (UsuarioDTO userInfo)
        {
            var user = _userManager.FindByEmailAsync(userInfo.Email);
            var permissoesUser = BuscarPermissoesUsuario(Guid.Parse(user.Result.Id));

            //define declarações do usuário
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Result.Id),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64)
            };

            foreach (var permissao in permissoesUser.Result.Perfil.Permissao)
            {
                claims.Add(new Claim(permissoesUser.Result.Perfil.Descricao.ToString(), permissao.Descricao));
            };

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            //gera uma chave com base em um algoritmo simetrico
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));

            //gera a assinatura digital do token usando o algoritmo Hmac e a chave privada
            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Tempo de expiracão do token.
            var expiracao = _configuration["TokenConfiguration:ExpireHours"];
            var expiration = DateTime.UtcNow.AddHours(double.Parse(expiracao));

            // classe que representa um token JWT e gera o token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _configuration["TokenConfiguration:Issuer"],
                Audience = _configuration["TokenConfiguration:Audience"],
                Subject = identityClaims,
                Expires = expiration,
                SigningCredentials = credenciais,
            });

            //retorna os dados com o token e informacoes
            
            return new LoginResponseViewModel()
            {
                AccessToken = tokenHandler.WriteToken(token),
                ExpiresIn = double.Parse(expiracao),
                UserToken = new UserTokenViewModel
                {
                    Id = Guid.Parse(user.Result.Id),
                    Email = userInfo.Email,
                    Claims = claims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value })
                }
            };

        }

        private void GerenciarUsuarioAutenticado (IdentityUser userAutenticado)
        {
            var user = _context.UsuarioRepository.GetById(x => x.Email == userAutenticado.Email);

            if(user != null)
            {
                user.LoginId = Guid.Parse(userAutenticado.Id);

                _context.UsuarioRepository.Update(user);
                _context.Commit();
            }
            else
            {
                var userNovo = new Usuario()
                {
                    UsuarioId = Guid.NewGuid(),
                    Nome = userAutenticado.NormalizedUserName,
                    Email = userAutenticado.Email,
                    PerfilId = 1,
                    LoginId = Guid.Parse(userAutenticado.Id)
                };

                _context.UsuarioRepository.Add(userNovo);
                _context.Commit();

            }
        }

        private async Task<Usuario> BuscarPermissoesUsuario (Guid idLogin)
        {
            var userPermissoes = await _context.UsuarioRepository.BuscarPermissoesUsuario(idLogin);

            return userPermissoes;

        }
    }
}
