using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using API.PDV.Infra;
using API.PDV.Domain;
using System.Security.Claims;

namespace API.PDV.WebApi
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantRepository tenantRepository, ISchemaContextAccessor schemaContextAccessor)
        {
            // Obtém o slug do tenant do header
            var tenantSlug = context.Request.Headers["X-Tenant"].ToString();

            // Se o usuário está autenticado, verifica se o tenant do header bate com o claim do usuário
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userTenant = context.User.FindFirst("tenant")?.Value;
                if (string.IsNullOrEmpty(userTenant) || !string.Equals(userTenant, tenantSlug, System.StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Tenant não autorizado para este usuário.");
                    return;
                }
            }

            if (!string.IsNullOrEmpty(tenantSlug))
            {
                var tenant = await tenantRepository.GetBySlugAsync(tenantSlug);
                if (tenant != null)
                {
                    schemaContextAccessor.CurrentSchema = $"tenant_{tenant.Slug}";
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Tenant inválido.");
                    return;
                }
            }
            else
            {
                // Se não informado, usar schema público
                schemaContextAccessor.CurrentSchema = "public";
            }

            await _next(context);
        }
    }
}
