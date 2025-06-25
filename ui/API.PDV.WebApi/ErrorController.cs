/// <summary>
/// Controller para tratamento global de erros da API.
/// </summary>
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.PDV.WebApi
{
    /// <summary>
    /// Gerencia o endpoint de erro global da aplicação.
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Manipula erros não tratados e retorna resposta padrão.
        /// Este endpoint é oculto do Swagger e serve apenas para tratamento global de exceções.
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error")]
        [HttpGet]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            // Log detalhado pode ser adicionado aqui se necessário

            return Problem(
                detail: exception?.Message,
                statusCode: 500,
                title: "An unexpected error occurred."
            );
        }
    }
}
