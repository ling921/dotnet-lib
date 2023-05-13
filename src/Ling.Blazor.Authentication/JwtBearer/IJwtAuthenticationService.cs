namespace Ling.Blazor.Authentication.JwtBearer;

/// <summary>
/// An interface that defines the methods for JWT authentication service
/// </summary>
public interface IJwtAuthenticationService
{
    /// <summary>
    /// A method that performs asynchronous login with the given username, password and persistence option
    /// </summary>
    /// <param name="username">A string that represents the user name</param>
    /// <param name="password">A string that represents the password</param>
    /// <param name="isPersistent">A boolean value that indicates whether the login should be persistent or not</param>
    /// <param name="cancellationToken">A CancellationToken that can be used to cancel the login operation</param>
    /// <returns>A Task that represents the asynchronous login operation</returns>
    Task LoginAsync(string username, string password, bool isPersistent = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// A method that performs asynchronous logout
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken that can be used to cancel the logout operation</param>
    /// <returns>A Task that represents the asynchronous logout operation</returns>
    Task LogoutAsync(CancellationToken cancellationToken = default);
}
