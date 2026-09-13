using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetBuddies_API.Application.Dtos.Dev;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Presentation.Conventions;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Presentation.Controllers
{
    // Em producao este servico nao emite token — ele valida o que o Java emite.
    // O atributo abaixo faz o controller sumir das rotas fora de Development.
    [ApenasEmDesenvolvimento]
    [Route("api/dev/token")]
    [ApiController]
    [AllowAnonymous]
    public class TokenDevController : ControllerBase
    {
        private readonly ITokenDevService _servico;

        public TokenDevController(ITokenDevService servico)
        {
            _servico = servico;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Emite um token para testar este serviço sem o Java no ar",
            Description = "Assina um JWT com o mesmo PETBUDDIES_JWT_SECRET que a API valida. "
                        + "Copie o campo 'token' e cole em Authorize. Só existe em Development.")]
        [ProducesResponseType(typeof(TokenDevDto), StatusCodes.Status200OK)]
        public ActionResult<TokenDevDto> Emitir([FromQuery] PerfilEnum perfil = PerfilEnum.VET)
            => Ok(_servico.Emitir(perfil));
    }
}
