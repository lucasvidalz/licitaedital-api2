using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Domain.Engagement.SavedOpportunityAggregate;
using LicitaEdital.Domain.Engagement.SavedOpportunityAggregate.Specifications;
using LicitaEdital.Facade.Catalog;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Engagement;

namespace LicitaEdital.Application.Engagement.SavedOpportunities.Save;

/// <summary>
/// Salva uma licitacao. **Idempotente**: salvar duas vezes devolve a mesma linha, sem criar a
/// segunda — o botao da tela e' um alternador, e um duplo clique nao pode virar erro.
/// </summary>
public class SaveOpportunityHandler(
  IExecutionContext execution,
  IRepository<SavedOpportunity> saved,
  ISavedOpportunitiesQueryService reader,
  ICatalogFacade catalog)
  : ICommandHandler<SaveOpportunityCommand, Result<SavedOpportunityView>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IRepository<SavedOpportunity> _saved = saved;
  private readonly ISavedOpportunitiesQueryService _reader = reader;
  private readonly ICatalogFacade _catalog = catalog;

  public async ValueTask<Result<SavedOpportunityView>> Handle(SaveOpportunityCommand command,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result<SavedOpportunityView>.Unauthorized();
    if (_execution.UserId is not { } userId) return Result<SavedOpportunityView>.Unauthorized();

    var organizationId = OrganizationId.From(tenantId);

    // Confere na fachada **antes de gravar**: salvar um id que nao existe criaria uma linha orfa que
    // a listagem descartaria em silencio para sempre — o usuario clicaria em salvar e nada
    // apareceria, sem nenhum erro.
    var summaries = await _catalog.GetSummariesAsync(organizationId, [command.OpportunityId],
      cancellationToken);

    if (summaries.Count == 0) return Result<SavedOpportunityView>.NotFound();

    var existing = await _saved.FirstOrDefaultAsync(
      new SavedOpportunitySpec(organizationId, command.OpportunityId), cancellationToken);

    if (existing is null)
    {
      existing = SavedOpportunity.Create(organizationId, command.OpportunityId, UserId.From(userId));
      await _saved.AddAsync(existing, cancellationToken);
    }

    return new SavedOpportunityView(existing.CreatedAt, summaries[0]);
  }
}
