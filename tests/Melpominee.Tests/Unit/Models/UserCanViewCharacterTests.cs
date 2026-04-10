using Melpominee.app.Models.Auth;
using Melpominee.app.Models.Characters.VTMV5;
using Shouldly;
namespace Melpominee.Tests.Unit.Models;

public class UserCanViewCharacterTests {
    [Fact]
    public void OwnerMatchesEmail_ReturnsTrue() {
        var user = new User { Email = "a@b.com" };
        var character = new VampireV5Character { Owner = "a@b.com" };

        user.CanViewCharacter(character).ShouldBeTrue();
    }

    [Fact]
    public void OwnerDoesNotMatch_ReturnsFalse() {
        var user = new User { Email = "a@b.com" };
        var character = new VampireV5Character { Owner = "other@b.com" };

        user.CanViewCharacter(character).ShouldBeFalse();
    }

    [Fact]
    public void NullCharacter_ReturnsFalse() {
        var user = new User { Email = "a@b.com" };

        user.CanViewCharacter(null).ShouldBeFalse();
    }

    [Fact]
    public void NullOwner_ReturnsFalse() {
        var user = new User { Email = "a@b.com" };
        var character = new VampireV5Character { Owner = null };

        user.CanViewCharacter(character).ShouldBeFalse();
    }
}
