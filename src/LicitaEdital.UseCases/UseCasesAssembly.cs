namespace LicitaEdital.UseCases;

/// <summary>
/// Marcador do assembly, para o <c>options.Assemblies</c> do Mediator em
/// <c>Web/Configurations/MediatorConfig.cs</c>. O source generator descobre o assembly a partir de
/// um tipo qualquer dele, e sem um tipo publico estavel a lista dependeria do primeiro handler que
/// alguem escrevesse — e quebraria quando ele fosse renomeado.
///
/// Paginacao (<c>PagedResult</c>, <c>PageRequest</c>) e catalogo de erros vem da lib,
/// <c>LicitaEdital.BuildingBlocks.Application</c>.
/// </summary>
public sealed class UseCasesAssembly;
