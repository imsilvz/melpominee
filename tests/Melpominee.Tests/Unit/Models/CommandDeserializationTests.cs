using System.Text.Json;
using Melpominee.app.Models.Characters.VTMV5;
using Shouldly;
namespace Melpominee.Tests.Unit.Models;

public class CommandDeserializationTests {
    private static readonly JsonSerializerOptions Options = new() {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void HeaderUpdate_CamelCase_BindsCorrectly() {
        var json = """{"name":"Dracula","generation":8,"clan":"Ventrue"}""";
        var element = JsonSerializer.Deserialize<JsonElement>(json);

        var result = element.Deserialize<VampireCharacterUpdate>(Options);

        result.ShouldNotBeNull();
        result.Name.ShouldBe("Dracula");
        result.Generation.ShouldBe(8);
        result.Clan.ShouldBe("Ventrue");
    }

    [Fact]
    public void AttributeUpdate_CamelCase_BindsCorrectly() {
        var json = """{"strength":3,"dexterity":2,"stamina":4}""";
        var element = JsonSerializer.Deserialize<JsonElement>(json);

        var result = element.Deserialize<VampireAttributeUpdate>(Options);

        result.ShouldNotBeNull();
        result.Strength.ShouldBe(3);
        result.Dexterity.ShouldBe(2);
        result.Stamina.ShouldBe(4);
    }

    [Fact]
    public void SkillUpdate_NestedObject_BindsCorrectly() {
        var json = """{"athletics":{"score":2,"speciality":"Running"}}""";
        var element = JsonSerializer.Deserialize<JsonElement>(json);

        var result = element.Deserialize<VampireSkillUpdate>(Options);

        result.ShouldNotBeNull();
        result.Athletics.ShouldNotBeNull();
        result.Athletics.Score.ShouldBe(2);
        result.Athletics.Speciality.ShouldBe("Running");
    }

    [Fact]
    public void StatsUpdate_NestedStructure_BindsCorrectly() {
        var json = """{"health":{"baseValue":7,"superficialDamage":2}}""";
        var element = JsonSerializer.Deserialize<JsonElement>(json);

        var result = element.Deserialize<VampireStatsUpdate>(Options);

        result.ShouldNotBeNull();
        result.Health.ShouldNotBeNull();
        result.Health.BaseValue.ShouldBe(7);
        result.Health.SuperficialDamage.ShouldBe(2);
    }

    [Fact]
    public void PowersUpdate_ListItems_BindCorrectly() {
        var json = """{"powerIds":[{"powerId":"claws-of-the-beast","remove":false}]}""";
        var element = JsonSerializer.Deserialize<JsonElement>(json);

        var result = element.Deserialize<VampirePowersUpdate>(Options);

        result.ShouldNotBeNull();
        result.PowerIds.ShouldNotBeNull();
        result.PowerIds.Count.ShouldBe(1);
        result.PowerIds[0].PowerId.ShouldBe("claws-of-the-beast");
        result.PowerIds[0].Remove.ShouldBeFalse();
    }

    [Fact]
    public void DisciplineUpdate_JsonPropertyNames_BindCorrectly() {
        var json = """{"Animalism":3,"Auspex":2}""";
        var element = JsonSerializer.Deserialize<JsonElement>(json);

        var result = element.Deserialize<VampireDisciplineUpdate>(Options);

        result.ShouldNotBeNull();
        result.Animalism.ShouldBe(3);
        result.Auspex.ShouldBe(2);
    }
}
