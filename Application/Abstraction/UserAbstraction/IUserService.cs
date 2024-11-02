using FS.Keycloak.RestApiClient.Model;

namespace Application.Abstraction.UserAbstraction;

public interface IUserService
{
    public Task<IList<UserRepresentation>> GetUsersFromRealmAsync(string nameRealm);

    public Task<UserRepresentation> GetUserFromRealmByIdAsync(string nameRealm, string userId);
}
