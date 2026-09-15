using LicitaEdital.Domain.Catalog.CompatibilityAggregate;
using LicitaEdital.Domain.Catalog.OpportunityAggregate;
using LicitaEdital.Domain.Collections.CollectionRunAggregate;
using LicitaEdital.Domain.Collections.CoverageSettingsAggregate;
using LicitaEdital.Domain.Companies.CompanyProfileAggregate;
using LicitaEdital.Domain.Engagement.AlertPreferencesAggregate;
using LicitaEdital.Domain.Engagement.SavedOpportunityAggregate;
using LicitaEdital.Domain.Engagement.SubscriptionAggregate;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.OrganizationAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate;
using LicitaEdital.Domain.Offerings.OfferingAggregate;
using LicitaEdital.Facade.Shared;
using Vogen;

namespace LicitaEdital.Data.Config;

/// <summary>
/// Um <c>[EfCoreConverter]</c> por value object usado como propriedade mapeada. Sem a entrada aqui,
/// o EF Core nao sabe traduzir o tipo e a propriedade some do modelo em silencio — nao da erro de
/// compilacao, da tabela sem coluna.
///
/// Fica num arquivo so, e nao um por modulo, porque o Vogen gera uma classe parcial por assembly.
/// </summary>
// Cnpj e StateCode NAO entram aqui: sao tipos da lib, escritos a mao, e tem conversor proprio
// (`HasCnpjConversion` / `HasStateCodeConversion` de BrasilValueConverters).
// --- identidades compartilhadas ---
[EfCoreConverter<OrganizationId>]
[EfCoreConverter<UserId>]
[EfCoreConverter<OpportunityId>]
[EfCoreConverter<OfferingId>]
// --- Identity ---
[EfCoreConverter<OrganizationName>]
[EfCoreConverter<MembershipId>]
[EfCoreConverter<RoleId>]
[EfCoreConverter<RoleName>]
// --- Companies ---
[EfCoreConverter<CompanyProfileId>]
[EfCoreConverter<CompanyName>]
// --- Catalog ---
[EfCoreConverter<OpportunityLineItemId>]
[EfCoreConverter<OpportunityDocumentId>]
[EfCoreConverter<OpportunityCompatibilityId>]
[EfCoreConverter<CompatibilityScore>]
// --- Offerings ---
[EfCoreConverter<OfferingName>]
// --- Engagement ---
[EfCoreConverter<SavedOpportunityId>]
[EfCoreConverter<AlertPreferencesId>]
[EfCoreConverter<SubscriptionId>]
// --- Collections ---
[EfCoreConverter<CollectionRunId>]
[EfCoreConverter<CoverageSettingsId>]
internal partial class VogenEfCoreConverters;
