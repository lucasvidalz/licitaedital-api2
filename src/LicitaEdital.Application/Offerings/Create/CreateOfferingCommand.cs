using LicitaEdital.Queries.Contracts.Offerings;

namespace LicitaEdital.Application.Offerings.Create;

public sealed record CreateOfferingCommand(OfferingPayload Payload) : ICommand<Result<OfferingDto>>;
