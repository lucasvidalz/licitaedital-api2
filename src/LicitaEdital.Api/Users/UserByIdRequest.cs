namespace LicitaEdital.Api.Users;

/// <summary>
/// O `{id}` da rota, para os tres endpoints que agem sobre um usuario.
///
/// Tipado como <c>Guid</c>, e nao <c>string</c>: id malformado vira 400 no binding, antes de virar
/// consulta. Um `Guid.Parse` dentro do handler daria 500 para a mesma entrada.
/// </summary>
public class UserByIdRequest
{
  public Guid Id { get; set; }
}
