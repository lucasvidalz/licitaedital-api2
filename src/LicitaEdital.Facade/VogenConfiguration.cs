using Vogen;

// Configuracao global do Vogen para os value objects deste assembly.
// Ficava em ContributorId.cs no template; foi movida para ca ao remover o agregado de exemplo,
// porque vale para todo value object do Core e nao para um em particular.
[assembly: VogenDefaults(
  staticAbstractsGeneration: StaticAbstractsGeneration.MostCommon | StaticAbstractsGeneration.InstanceMethodsAndProperties)]
