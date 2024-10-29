using Application.UserFeatures;
using Refit;

namespace API.Abstraction.Users;

public interface IUserManagement
{
    [Get("{base-url}/admin/realms/{realm}/users")]
    public Task<IReadOnlyList<UserFromRealmGetDto>> GetUsersFromRealm(string baseUrl, string realmName);
}
