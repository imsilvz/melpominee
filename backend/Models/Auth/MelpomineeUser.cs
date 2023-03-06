using System.Web;
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
        using (var connection = new SqliteConnection("Data Source=data/melpominee.db"))
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

    public bool BeginResetPassword(string origin)
    {
        // quick check validity of object
        if(string.IsNullOrEmpty(Email)) 
        { 
            return false; 
        }
        
        // generate confirmation key
        byte[] confirmKeyBytes = RandomNumberGenerator.GetBytes(64);
        string rescueKey = Convert.ToBase64String(confirmKeyBytes);

        // create new entry
        using (var connection = new SqliteConnection("Data Source=data/melpominee.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT OR REPLACE INTO melpominee_users_rescue
                    (user_email, rescue_key, requested_timestamp, completed_timestamp)
                VALUES
                    ($email, $rescuekey, CURRENT_TIMESTAMP, null)
            ";
            command.Parameters.AddWithValue("$email", Email);
            command.Parameters.AddWithValue("$rescuekey", HashPassword(rescueKey));

            try
            {
                if(command.ExecuteNonQuery() < 1) 
                {
                    // 
                    return false;
                }
            }
            catch(SqliteException)
            {
                // catch foreign key failed
                return false;
            }
        }

        // send email
        Utilities.MailManager.Instance.SendMail(
            Email,
            "Melpominee.app: Reset Password",
            $@"
            <html style='width:100%;height:100%;padding:2rem;'>
                <body style='width:100%;height:100%;padding:4rem 0 4rem 0;display:flex;justify-content:center;align-items:center;background-color:#121212;'>
                    <div style='box-sizing:border-box;width:600px;padding:3rem;background-color:#1f1f1f;'>
                        <h2 style='color:white;text-align:center;font-size:1.625rem;margin-top:0;'>Password Reset Instructions</h2>
                        <p style='color:white;text-align:center;font-size:1rem;'>Hello from the Melpominee team! We have received a request to reset your password at Melpominee.app.</p>
                        <p style='color:white;text-align:center;font-size:1rem;'>To continue, please hit the button below.</p>
                        <p style='width:100%;height:2.5rem;border:none;border-radius:0.5rem;background-color:#aa2e25;'>
                            <a href='{origin}/forgot-password?email={Email}&key={HttpUtility.UrlEncode(rescueKey)}' style='width:100%;height:100%;color:white;text-decoration:none;display:flex;justify-content:center;align-items:center;'>
                                Reset Password
                            </a>
                        </p>
                        <p style='color:rgba(255, 255, 255, 0.7);font-size:0.875rem;margin:0;margin-top:1rem;text-align:center;'>If you did not request this password reset, you can safely ignore this email.</p>
                    </div>
                </body>
            </html>
            "
        );

        return true;
    }

    public bool FinishResetPassword(string key, string password)
    {
        return true;
    }

    public bool Register(string origin, string password)
    {
        // quick check validity of object
        if(string.IsNullOrEmpty(Email)) 
        { 
            return false; 
        }
        
        // generate confirmation key
        byte[] confirmKeyBytes = RandomNumberGenerator.GetBytes(64);
        string activationKey = Convert.ToBase64String(confirmKeyBytes);

        // create new entry
        using (var connection = new SqliteConnection("Data Source=data/melpominee.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT OR IGNORE INTO melpominee_users
                    (email, password, activation_key)
                VALUES
                    ($email, $password, $activationkey)
            ";
            command.Parameters.AddWithValue("$email", Email);
            command.Parameters.AddWithValue("$password", HashPassword(password));
            command.Parameters.AddWithValue("$activationkey", activationKey);

            if(command.ExecuteNonQuery() < 1) 
            {
                return false;
            }
        }
        
        // send email
        Utilities.MailManager.Instance.SendMail(
            Email,
            "Melpominee.app: Account Activation",
            $@"
            <html style='width:100%;height:100%;padding:2rem;'>
                <body style='width:100%;height:100%;padding:4rem 0 4rem 0;display:flex;justify-content:center;align-items:center;background-color:#121212;'>
                    <div style='box-sizing:border-box;width:600px;padding:3rem;background-color:#1f1f1f;'>
                        <h2 style='color:white;text-align:center;font-size:1.625rem;margin-top:0;'>Hey there!</h2>
                        <p style='color:white;text-align:center;font-size:1rem;'>Welcome to Melpominee.app. We're almost ready for you.</p>
                        <p style='color:white;text-align:center;font-size:1rem;'>Before you can log in, your account needs to be activated. In order to activate your account, please click on the button below.</p>
                        <p style='width:100%;height:2.5rem;border:none;border-radius:0.5rem;background-color:#aa2e25;'>
                            <a href='{origin}/api/auth/register/confirmation?email={Email}&activationkey={HttpUtility.UrlEncode(activationKey)}' style='width:100%;height:100%;color:white;text-decoration:none;display:flex;justify-content:center;align-items:center;'>
                                Activate my Account!
                            </a>
                        </p>
                        <p style='color:rgba(255, 255, 255, 0.7);font-size:0.875rem;margin:0;margin-top:1rem;text-align:center;'>This email was sent by Melpominee.app. If you didn't sign up with Melpominee.app, don't worry! You can just ignore this.</p>
                    </div>
                </body>
            </html>
            "
        );

        return true;
    }

    public bool RegistrationFinish(string key)
    {
        // quick check validity of object
        if(string.IsNullOrEmpty(Email)) 
        { 
            return false; 
        }
        
        // make db connection
        using (var connection = new SqliteConnection("Data Source=data/melpominee.db"))
        {
            connection.Open();

            // get key
            var getCommand = connection.CreateCommand();
            getCommand.CommandText = 
            @"
                SELECT email, activation_key
                FROM melpominee_users
                WHERE email = $email
            ";
            getCommand.Parameters.AddWithValue("$email", Email);

            string? dbEmail = null;
            string? dbKey = null;
            using (var reader = getCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    dbEmail = reader.GetString(0);
                    dbKey = reader.GetString(1);
                }
            }

            // break out early if either value is bad
            if (string.IsNullOrEmpty(dbEmail) || string.IsNullOrEmpty(dbKey))
            {
                return false;
            }

            // compare
            if(key != dbKey)
            {
                return false;
            }

            var setCommand = connection.CreateCommand();
            setCommand.CommandText =
            @"
                UPDATE melpominee_users
                SET activation_key = null,
                    active = true
                WHERE email = $email
            ";
            setCommand.Parameters.AddWithValue("$email", Email);

            if(setCommand.ExecuteNonQuery() < 1) 
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