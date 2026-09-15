using System.Threading.RateLimiting;
using LicitaEdital.BuildingBlocks.Web.Defaults;
using Microsoft.AspNetCore.RateLimiting;

namespace LicitaEdital.Web.Configurations;

public static class RateLimitConfigs
{
  /// <summary>
  /// Rate limiting dos endpoints sensiveis a forca bruta e a enumeracao: login, registro e
  /// recuperacao de senha.
  ///
  /// <para>
  /// Mora aqui, e nao na lib, por uma limitacao concreta: <c>AddRateLimiter</c> nao e' alcancavel
  /// de uma class library com <c>FrameworkReference Microsoft.AspNetCore.App</c> nesta instalacao
  /// do SDK. O **nome** da politica vem da lib (<see cref="RateLimiterPolicies.Sensitive"/>), que e'
  /// o que impede o registro e o atributo do endpoint divergirem — divergir ali nao da erro de
  /// compilacao, da endpoint sem limite nenhum.
  /// </para>
  ///
  /// Limitar aqui **nao** substitui o lockout do Identity: aquele protege a conta, este protege o
  /// servidor e atrasa a varredura de e-mails validos, que nenhum lockout por conta impede.
  /// </summary>
  public static IServiceCollection AddRateLimitPolicies(this IServiceCollection services)
    => services.AddRateLimiter(limiter =>
    {
      limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

      limiter.AddPolicy(RateLimiterPolicies.Sensitive, context =>
        RateLimitPartition.GetFixedWindowLimiter(
          // Particiona por IP, nao por usuario: no login ainda nao ha usuario, e e' o login que
          // mais precisa do limite.
          context.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
          _ => new FixedWindowRateLimiterOptions
          {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
          }));
    });
}
