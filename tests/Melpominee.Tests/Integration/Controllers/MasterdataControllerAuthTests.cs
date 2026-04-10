using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Melpominee.Tests.Infrastructure;
using Shouldly;
namespace Melpominee.Tests.Integration.Controllers;

public class MasterdataControllerAuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MasterdataControllerAuthTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetBloodPotency_Unauthenticated_Returns401()
    {
        var response = await _client.AsAnonymous()
            .GetAsync("/vtmv5/masterdata/bloodpotency?id=1");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetClanList_Unauthenticated_Returns401()
    {
        var response = await _client.AsAnonymous()
            .GetAsync("/vtmv5/masterdata/clan/list");
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBloodPotencyList_Authenticated_Returns200()
    {
        var response = await _client.WithTestUser("test@test.com")
            .GetAsync("/vtmv5/masterdata/bloodpotency/list");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetClanList_Authenticated_Returns200WithData()
    {
        var response = await _client.WithTestUser("test@test.com")
            .GetAsync("/vtmv5/masterdata/clan/list");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("success").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public async Task GetDisciplineList_Authenticated_Returns200()
    {
        var response = await _client.WithTestUser("test@test.com")
            .GetAsync("/vtmv5/masterdata/discipline/list");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPredatorTypeList_Authenticated_Returns200()
    {
        var response = await _client.WithTestUser("test@test.com")
            .GetAsync("/vtmv5/masterdata/predatortype/list");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetResonanceList_Authenticated_Returns200()
    {
        var response = await _client.WithTestUser("test@test.com")
            .GetAsync("/vtmv5/masterdata/resonance/list");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
