namespace Melpominee.Tests.Infrastructure;

public static class HttpClientExtensions
{
    public static HttpClient WithTestUser(this HttpClient client, string email, string role = "user")
    {
        client.DefaultRequestHeaders.Remove(TestAuthHandler.UserHeader);
        client.DefaultRequestHeaders.Remove(TestAuthHandler.RoleHeader);
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserHeader, email);
        client.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeader, role);
        return client;
    }

    public static HttpClient AsAnonymous(this HttpClient client)
    {
        client.DefaultRequestHeaders.Remove(TestAuthHandler.UserHeader);
        client.DefaultRequestHeaders.Remove(TestAuthHandler.RoleHeader);
        return client;
    }
}
