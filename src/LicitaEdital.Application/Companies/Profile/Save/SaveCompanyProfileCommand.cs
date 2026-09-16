using LicitaEdital.Queries.Contracts.Companies;

namespace LicitaEdital.Application.Companies.Profile.Save;

/// <summary>
/// `PUT /company-profile` — **upsert idempotente**: o mesmo corpo enviado duas vezes produz o mesmo
/// estado, e o cliente nao precisa saber se o cadastro ja existia.
///
/// Os campos chegam como string porque e' assim que o contrato os traz. Virar value object
/// (<c>Cnpj</c>, <c>StateCode</c>) e' trabalho do endpoint e do validador, onde a entrada malformada
/// ainda vira 400 com o campo nomeado, e nao excecao no meio do caso de uso.
/// </summary>
public sealed record SaveCompanyProfileCommand(
  string CompanyName,
  string Cnpj,
  string City,
  string State,
  string BusinessArea) : ICommand<Result<CompanyProfileDto>>;
