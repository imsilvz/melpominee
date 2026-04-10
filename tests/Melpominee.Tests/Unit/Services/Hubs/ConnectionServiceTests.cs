using Microsoft.AspNetCore.SignalR;
using Melpominee.app.Hubs.VTMV5;
using Melpominee.app.Hubs.Clients.VTMV5;
using Melpominee.app.Services.Hubs;
using NSubstitute;
using Shouldly;
namespace Melpominee.Tests.Unit.Services.Hubs;

public class ConnectionServiceTests {
    private readonly ConnectionService _svc;
    private readonly IHubContext<CharacterHub, ICharacterClient> _hub;

    public ConnectionServiceTests() {
        _svc = new ConnectionService();
        _hub = Substitute.For<IHubContext<CharacterHub, ICharacterClient>>();
        var clientProxy = Substitute.For<ICharacterClient>();
        _hub.Clients.Group(Arg.Any<string>()).Returns(clientProxy);
    }

    [Fact]
    public void OnConnect_AddsToConnectionMap() {
        _svc.OnConnect(_hub, "user@test.com", "conn1");

        _svc.ConnectionMap["conn1"].ShouldBe("user@test.com");
    }

    [Fact]
    public void OnConnect_AddsToUserMap() {
        _svc.OnConnect(_hub, "user@test.com", "conn1");

        _svc.UserMap["user@test.com"].ContainsKey("conn1").ShouldBeTrue();
    }

    [Fact]
    public void OnConnect_MultipleConnections_AllTracked() {
        _svc.OnConnect(_hub, "user@test.com", "conn1");
        _svc.OnConnect(_hub, "user@test.com", "conn2");

        _svc.UserMap["user@test.com"].ContainsKey("conn1").ShouldBeTrue();
        _svc.UserMap["user@test.com"].ContainsKey("conn2").ShouldBeTrue();
    }

    [Fact]
    public void OnGroupAdd_AddsToGroupMap() {
        _svc.OnConnect(_hub, "user@test.com", "conn1");
        _svc.OnGroupAdd(_hub, "conn1", "character_1");

        _svc.GroupMap["character_1"].ContainsKey("conn1").ShouldBeTrue();
    }

    [Fact]
    public void OnDisconnect_RemovesFromMaps() {
        _svc.OnConnect(_hub, "user@test.com", "conn1");
        _svc.OnDisconnect(_hub, "user@test.com", "conn1");

        _svc.ConnectionMap.ContainsKey("conn1").ShouldBeFalse();
        _svc.UserMap["user@test.com"].ContainsKey("conn1").ShouldBeFalse();
    }

    [Fact]
    public void GetUserConnections_ReturnsAllConnections() {
        _svc.OnConnect(_hub, "user@test.com", "conn1");
        _svc.OnConnect(_hub, "user@test.com", "conn2");

        var connections = _svc.GetUserConnections("user@test.com");

        connections.Count.ShouldBe(2);
        connections.ShouldContain("conn1");
        connections.ShouldContain("conn2");
    }

    [Fact]
    public void GetUserConnections_UnknownUser_ReturnsEmptyList() {
        var connections = _svc.GetUserConnections("nobody@test.com");

        connections.ShouldBeEmpty();
    }

    [Fact]
    public void GetConnectedUser_ReturnsIdentifier() {
        _svc.OnConnect(_hub, "user@test.com", "conn1");

        _svc.GetConnectedUser("conn1").ShouldBe("user@test.com");
    }
}
