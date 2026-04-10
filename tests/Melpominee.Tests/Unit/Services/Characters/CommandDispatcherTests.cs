using System.Collections.Frozen;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Melpominee.app.Models.Characters.VTMV5;
using Melpominee.app.Models.Web.VTMV5;
using Melpominee.app.Services.Characters;
using Shouldly;
namespace Melpominee.Tests.Unit.Services.Characters;

public class CommandDispatcherTests {
    private readonly CharacterCommandDispatcher _dispatcher;

    public CommandDispatcherTests() {
        var cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));
        _dispatcher = new CharacterCommandDispatcher(cache);
    }

    [Fact]
    public async Task ApplyCommands_UnknownType_ThrowsArgumentException() {
        var commands = new List<CharacterCommand> {
            new CharacterCommand {
                Type = "bogus",
                Data = JsonSerializer.Deserialize<JsonElement>("{}"),
            }
        };
        var character = new VampireV5Character();

        await Should.ThrowAsync<ArgumentException>(
            () => _dispatcher.ApplyCommands(character, commands));
    }

    [Fact]
    public async Task ApplyCommands_NullType_SkipsCommand() {
        var commands = new List<CharacterCommand> {
            new CharacterCommand {
                Type = null,
                Data = JsonSerializer.Deserialize<JsonElement>("{}"),
            }
        };
        var character = new VampireV5Character();

        var applied = await _dispatcher.ApplyCommands(character, commands);

        applied.ShouldBeEmpty();
    }

    [Fact]
    public async Task ApplyCommands_NullData_SkipsCommand() {
        var commands = new List<CharacterCommand> {
            new CharacterCommand {
                Type = "header",
                Data = null,
            }
        };
        var character = new VampireV5Character();

        var applied = await _dispatcher.ApplyCommands(character, commands);

        applied.ShouldBeEmpty();
    }

    [Fact]
    public void AllElevenCommandTypesAreRegistered() {
        var field = typeof(CharacterCommandDispatcher)
            .GetField("CommandMap", BindingFlags.NonPublic | BindingFlags.Static);
        field.ShouldNotBeNull();

        var commandMap = field.GetValue(null) as FrozenDictionary<string, Type>;
        commandMap.ShouldNotBeNull();
        commandMap.Count.ShouldBe(11);

        var expectedKeys = new[] {
            "header", "attributes", "skills", "stats",
            "disciplines", "powers", "beliefs",
            "backgrounds", "merits", "flaws", "profile"
        };

        foreach (var key in expectedKeys) {
            commandMap.ContainsKey(key).ShouldBeTrue($"CommandMap should contain key '{key}'");
        }
    }
}
