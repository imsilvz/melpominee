using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Melpominee.Tests.Infrastructure;
using Shouldly;
namespace Melpominee.Tests.Integration.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCurrentUser_Unauthenticated_ReturnsOk()
    {
        var response = await _client.AsAnonymous().GetAsync("/auth");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_EmptyPayload_ReturnsSuccessFalse()
    {
        var content = new StringContent(
            """{"email":"","password":""}""",
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _client.PostAsync("/auth/login", content);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("success").GetBoolean().ShouldBeFalse();
    }

    [Fact]
    public async Task Register_EmptyPayload_ReturnsSuccessFalse()
    {
        var content = new StringContent(
            """{"email":"","password":""}""",
            System.Text.Encoding.UTF8,
            "application/json");
        var response = await _client.PostAsync("/auth/register", content);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("success").GetBoolean().ShouldBeFalse();
    }
}
