namespace LicitaEdital.Queries.Contracts.Identity;

/// <summary>
/// Filtro de `GET /users`. <see cref="OrganizationId"/> **nao vem do cliente** — e' a claim da
/// sessao (SEC-08), e esta aqui para que a consulta nao consiga ser escrita sem ele.
///
/// <para>
/// <see cref="Status"/> e' a string do contrato (<c>active</c>/<c>inactive</c>) e nao o SmartEnum:
/// valor invalido na query string e' erro de entrada, resolvido pelo validador do endpoint, e nao
/// uma excecao de conversao vinda do meio da consulta.
/// </para>
/// </summary>
public sealed record ListUsersFilter(
  OrganizationId OrganizationId,
  string? Search = null,
  string? Status = null);
