namespace Bryan.TokenAuth.Interfaces;

public interface IGraphService
{
    Task<Result<List<AppRole>>> GetApplicationRoles();

    Task<Result<User>> GetUser(string email);

    Task<Result<List<AppRoleAssignments>>> GetUserAppRoles(string email);
}