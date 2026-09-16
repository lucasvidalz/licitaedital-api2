using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Offerings;

namespace LicitaEdital.Application.Offerings.Update;

public sealed record UpdateOfferingCommand(OfferingId OfferingId, OfferingPayload Payload)
  : ICommand<Result<OfferingDto>>;
