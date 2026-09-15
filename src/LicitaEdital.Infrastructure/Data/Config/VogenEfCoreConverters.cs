using LicitaEdital.Core.Catalog.CompatibilityAggregate;
using LicitaEdital.Core.Catalog.OpportunityAggregate;
using LicitaEdital.Core.Collections.CollectionRunAggregate;
using LicitaEdital.Core.Collections.CoverageSettingsAggregate;
using LicitaEdital.Core.Companies.CompanyProfileAggregate;
using LicitaEdital.Core.Engagement.AlertPreferencesAggregate;
using LicitaEdital.Core.Engagement.SavedOpportunityAggregate;
using LicitaEdital.Core.Engagement.SubscriptionAggregate;
using LicitaEdital.Core.Identity.MembershipAggregate;
using LicitaEdital.Core.Identity.OrganizationAggregate;
using LicitaEdital.Core.Identity.RoleAggregate;
using LicitaEdital.Core.Offerings.OfferingAggregate;
using LicitaEdital.Core.Shared;
using Vogen;

namespace LicitaEdital.Infrastructure.Data.Config;

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
