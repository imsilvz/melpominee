using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Melpominee.app.Services.Auth;
using Shouldly;
namespace Melpominee.Tests.Unit.Services.Auth;

public class PasswordHashingTests {
    [Fact]
    public void HashPassword_ProducesDifferentHashesForSameInput() {
        var hash1 = UserManager.HashPassword("password123!");
        var hash2 = UserManager.HashPassword("password123!");

        hash1.ShouldNotBe(hash2);
    }

    [Fact]
    public void HashPassword_OutputStartsWithCurrentVersionByte() {
        var hash = UserManager.HashPassword("password123!");
        var bytes = Convert.FromBase64String(hash);

        bytes[0].ShouldBe((byte)0x03);
    }

    [Fact]
    public void HashPassword_OutputIsValidBase64() {
        var hash = UserManager.HashPassword("password123!");
        var bytes = Convert.FromBase64String(hash);

        bytes.Length.ShouldBe(1 + 64 + 16);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue() {
        var hash = UserManager.HashPassword("correct-horse-battery-staple");

        UserManager.VerifyPassword(hash, "correct-horse-battery-staple").ShouldBeTrue();
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ReturnsFalse() {
        var hash = UserManager.HashPassword("correct");

        UserManager.VerifyPassword(hash, "wrong").ShouldBeFalse();
    }

    [Fact]
    public void VerifyPassword_NullHash_ReturnsFalse() {
        UserManager.VerifyPassword(null, "pw").ShouldBeFalse();
    }

    [Fact]
    public void VerifyPassword_NullPassword_ReturnsFalse() {
        var hash = UserManager.HashPassword("something");

        UserManager.VerifyPassword(hash, null).ShouldBeFalse();
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("", "pw")]
    [InlineData("hash", "")]
    public void VerifyPassword_EmptyInputs_ReturnsFalse(string hashed, string password) {
        UserManager.VerifyPassword(hashed, password).ShouldBeFalse();
    }

    [Fact]
    public void NeedsRehash_CurrentVersion_ReturnsFalse() {
        var hash = UserManager.HashPassword("password123!");

        UserManager.NeedsRehash(hash).ShouldBeFalse();
    }

    [Fact]
    public void NeedsRehash_LegacyVersion_ReturnsTrue() {
        var bytes = new byte[1 + 64 + 16];
        bytes[0] = 0x02;
        RandomNumberGenerator.Fill(bytes.AsSpan(1));
        var legacyHash = Convert.ToBase64String(bytes);

        UserManager.NeedsRehash(legacyHash).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NeedsRehash_NullOrEmpty_ReturnsFalse(string? hashed) {
        UserManager.NeedsRehash(hashed).ShouldBeFalse();
    }

    [Fact]
    public void VerifyPassword_LegacyHash_StillVerifies() {
        var password = "legacy-password-test";
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 64
        );

        var outputBytes = new byte[1 + 64 + 16];
        outputBytes[0] = 0x02;
        Buffer.BlockCopy(hash, 0, outputBytes, 1, 64);
        Buffer.BlockCopy(salt, 0, outputBytes, 1 + 64, 16);
        var legacyHash = Convert.ToBase64String(outputBytes);

        UserManager.VerifyPassword(legacyHash, password).ShouldBeTrue();
    }
}
