using LicitaEdital.Queries.Contracts.Engagement;

namespace LicitaEdital.Application.Engagement.Settings.Get;

public sealed record GetSettingsQuery : IQuery<Result<SettingsDto>>;
