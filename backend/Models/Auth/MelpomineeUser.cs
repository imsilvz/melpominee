using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Melpominee.app.Models.Auth;

public class MelpomineeUser {
    private static int _saltSize = 128 / 8;
    private static int _iterCount = 10000;
    private static int _passwordBytes = 512 / 8;
    public string? Email { get; set; }

    public MelpomineeUser()
    {
        Email = "";
    }

    public MelpomineeUser(string email)
    {
        Email = email;
    }

    public bool Load()
    {
        return true;
    }

    public bool Login(string password)
    {
        // validate Login
        if (string.IsNullOrEmpty(Email)) { 
            return false; 
        }

        // check database
        string? dbEmail = null;
        string? dbPassword = null;
        using (var connection = new SqliteConnection("Data Source=melpominee.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT email, password
                FROM melpominee_users
                WHERE email = $email
            ";
            command.Parameters.AddWithValue("$email", Email);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    dbEmail = reader.GetString(0);
                    dbPassword = reader.GetString(1);
                }
            }
        }

        // check if a match is found
        if (string.IsNullOrEmpty(dbEmail) || string.IsNullOrEmpty(dbPassword))
        {
            return false;
        }
        return MelpomineeUser.VerifyPassword(dbPassword, password);
    }

    public bool Register(string password)
    {
        using (var connection = new SqliteConnection("Data Source=melpominee.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT OR IGNORE INTO melpominee_users
                    (email, password)
                VALUES
                    ($email, $password)
            ";
            command.Parameters.AddWithValue("$email", Email);
            command.Parameters.AddWithValue("$password", HashPassword(password));

            if(command.ExecuteNonQuery() < 1) 
            {
                return false;
            }
        }
        return true;
    }

    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(_saltSize);
        return HashPassword(salt, password);
    }

    public static string HashPassword(byte[] salt, string password)
    {
        byte[] hashedBytes = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: _iterCount,
            numBytesRequested: _passwordBytes
        );
        
        byte[] outputBytes = new byte[1 + salt.Length + hashedBytes.Length];
        outputBytes[0] = 0x02;
        Buffer.BlockCopy(hashedBytes, 0, outputBytes, 1, hashedBytes.Length);
        Buffer.BlockCopy(salt, 0, outputBytes, 1 + hashedBytes.Length, salt.Length);

        string hashedPassword = Convert.ToBase64String(outputBytes);
        return hashedPassword;
    }

    public static bool VerifyPassword(string hashed, string password)
    {
        byte[] hashedBytes = Convert.FromBase64String(hashed);
        byte[] salt = new byte[_saltSize];
        Buffer.BlockCopy(hashedBytes, 1 + _passwordBytes, salt, 0, _saltSize);
        string newHashedPassword = HashPassword(salt, password);
        return hashed == newHashedPassword;
    }
}