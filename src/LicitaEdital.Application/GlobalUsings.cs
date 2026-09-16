global using Ardalis.Result;
global using LicitaEdital.BuildingBlocks.Application.Paging;
global using LicitaEdital.BuildingBlocks.Domain.Entities;
global using LicitaEdital.BuildingBlocks.Domain.Repositories;
global using Mediator;

// Os DTOs e os contratos de leitura vivem em Queries, e e' de la' que a Application os consome —
// o diagrama manda Application -> Queries. Global porque quase todo handler de Identity devolve
// `AuthenticatedUserDto`; repetir o `using` em cada arquivo so' escondia a dependencia real.
global using LicitaEdital.Queries.Contracts.Catalog;
global using LicitaEdital.Queries.Contracts.Identity;
