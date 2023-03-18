using Npgsql;
using Dapper;
using System.Web;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Melpominee.app.Models.Auth;
using Melpominee.app.Utilities.Database;
namespace Melpominee.app.Utilities.Auth;

public class UserManager
{
    private static Lazy<UserManager> _instance = new Lazy<UserManager>(() => new UserManager());
    public static UserManager Instance => _instance.Value;

    private static int _saltSize = 128 / 8;
    private static int _iterCount = 10000;
    private static int _passwordBytes = 512 / 8;

    public UserManager() {}

    public User? GetUser(string? email, bool onlyActive = false)
    {
        User? user;
        if(string.IsNullOrEmpty(email)) 
        { 
            return null; 
        }

        using (var conn = DataContext.Instance.Connect())
        {
            string sql = "SELECT * FROM melpominee_users WHERE Email = @Email";
            user = conn.QueryFirstOrDefault<User>(sql, new { Email = email });
            if (user is null || (onlyActive && !user.Active))
            {
                // unable to find this user!
                return null;
            }
        }
        return user;
    }

    public User? Login(string email, string password)
    {
        // validate input
        if(string.IsNullOrEmpty(email)) 
        { 
            return null; 
        }

        // get user object
        User? user = GetUser(email, true);
        if (user is null)
        {
            // unable to find this user!
            return null;
        }

        // check password
        if (VerifyPassword(user.Password, password))
        {
            // login successful!
            return user;
        }
        return null;
    }

    public bool BeginResetPassword(string email, string origin)
    {
        // quick check validity of object
        if(string.IsNullOrEmpty(email)) 
        { 
            return false; 
        }
        
        // generate confirmation key
        byte[] confirmKeyBytes = RandomNumberGenerator.GetBytes(64);
        string rescueKey = Convert.ToBase64String(confirmKeyBytes);

        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var sql =
                    @"
                        INSERT INTO melpominee_users_rescue
                            (Email, RescueKey, RescueRequested)
                        VALUES
                            (@Email, @RescueKey, CURRENT_TIMESTAMP);
                    ";
                    if (conn.Execute(sql, new { Email = email, RescueKey = HashPassword(rescueKey) }) <= 0)
                    {
                        trans.Rollback();
                        return false;
                    }
                    trans.Commit();
                }
                catch(NpgsqlException)
                {
                    // foreign key failed, this user does not exist!
                    trans.Rollback();
                    return false;
                }
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        // send email
        Utilities.MailManager.Instance.SendMail(
            email,
            "Melpominee.app: Reset Password",
            $@"
            <html style='width:100%;height:100%;padding:2rem;'>
                <body style='width:100%;height:100%;padding:4rem 0 4rem 0;display:flex;justify-content:center;align-items:center;background-color:#121212;'>
                    <div style='box-sizing:border-box;width:600px;padding:3rem;background-color:#1f1f1f;'>
                        <h2 style='color:white;text-align:center;font-size:1.625rem;margin-top:0;'>Password Reset Instructions</h2>
                        <p style='color:white;text-align:center;font-size:1rem;'>Hello from the Melpominee team! We have received a request to reset your password at Melpominee.app.</p>
                        <p style='color:white;text-align:center;font-size:1rem;'>To continue, please hit the button below.</p>
                        <p style='width:100%;height:2.5rem;border:none;border-radius:0.5rem;background-color:#aa2e25;'>
                            <a href='{origin}/forgot-password?email={email}&key={HttpUtility.UrlEncode(rescueKey)}' style='width:100%;height:100%;color:white;text-decoration:none;display:flex;justify-content:center;align-items:center;'>
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

    public bool FinishResetPassword(string email, string key, string password)
    {
        // quick check validity of object
        if (string.IsNullOrEmpty(email) || 
            string.IsNullOrEmpty(key) || 
            string.IsNullOrEmpty(password)) 
        { 
            return false; 
        }

        // handle database actions
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    // build and execute select query
                    var sql =
                    @"
                        SELECT rescue.*
                        FROM melpominee_users header
                        INNER JOIN melpominee_users_rescue rescue
                            ON header.Email = rescue.Email
                        WHERE header.Email = @Email
                        ORDER BY rescue.RescueRequested DESC;
                    ";
                    var rescueData = conn.QueryFirstOrDefault<UserRescue>(sql, new { Email = email });
                    if (rescueData is null)
                    {
                        trans.Rollback();
                        return false;
                    }

                    // check if this key does not exist or has already been used
                    // then check if attached key matches. if not,
                    // return false and rollback any changes
                    if (string.IsNullOrEmpty(rescueData.RescueKey) || 
                        (rescueData.RescueCompleted is not null) ||
                        !VerifyPassword(rescueData.RescueKey, key))
                    {
                        trans.Rollback();
                        return false;
                    }

                    // update parameters
                    rescueData.RescueKey = null;

                    // build and execute update query
                    sql =
                    @"
                        UPDATE melpominee_users
                        SET Password = @Password
                        WHERE Email = @Email;
                        UPDATE melpominee_users_rescue
                        SET RescueKey = null,
                            RescueCompleted = CURRENT_TIMESTAMP
                        WHERE Id = @Id;
                    ";
                    if (conn.Execute(sql, 
                        new {
                            Id=rescueData.Id, 
                            Email=rescueData.Email, 
                            Password=HashPassword(password)
                        }) < 2)
                    {
                        trans.Rollback();
                        return false;
                    }
                    trans.Commit();
                }
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        return true;
    }

    public User? Register(string email, string password, string origin)
    {
        // quick check validity of object
        User? user = null;
        if(string.IsNullOrEmpty(email)) 
        { 
            return user; 
        }
        
        // generate confirmation key
        byte[] confirmKeyBytes = RandomNumberGenerator.GetBytes(64);
        string activationKey = Convert.ToBase64String(confirmKeyBytes);

        // create new entry
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using(var transaction = conn.BeginTransaction())
            {
                // create user object
                user = new User
                {
                    Email = email, 
                    Password = HashPassword(password), 
                    ActivationKey = activationKey,
                    ActivationRequested = DateTime.UtcNow,
                };

                try
                {
                    // sync database
                    var sql = 
                    @"
                        INSERT INTO melpominee_users 
                            (email, password, activationkey, activationrequested)
                        VALUES
                            (@Email, @Password, @ActivationKey, CURRENT_TIMESTAMP)
                        ON CONFLICT(email) DO NOTHING;
                    ";
                    if (conn.Execute(sql, user) <= 0) {
                        transaction.Rollback();
                        return user;
                    }
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        // send email
        Utilities.MailManager.Instance.SendMail(
            email,
            "Melpominee.app: Account Activation",
            $@"
            <html style='width:100%;height:100%;padding:2rem;'>
                <body style='width:100%;height:100%;padding:4rem 0 4rem 0;display:flex;justify-content:center;align-items:center;background-color:#121212;'>
                    <div style='box-sizing:border-box;width:600px;padding:3rem;background-color:#1f1f1f;'>
                        <h2 style='color:white;text-align:center;font-size:1.625rem;margin-top:0;'>Hey there!</h2>
                        <p style='color:white;text-align:center;font-size:1rem;'>Welcome to Melpominee.app. We're almost ready for you.</p>
                        <p style='color:white;text-align:center;font-size:1rem;'>Before you can log in, your account needs to be activated. In order to activate your account, please click on the button below.</p>
                        <p style='width:100%;height:2.5rem;border:none;border-radius:0.5rem;background-color:#aa2e25;'>
                            <a href='{origin}/api/auth/register/confirmation?email={email}&activationkey={HttpUtility.UrlEncode(activationKey)}' style='width:100%;height:100%;color:white;text-decoration:none;display:flex;justify-content:center;align-items:center;'>
                                Activate my Account!
                            </a>
                        </p>
                        <p style='color:rgba(255, 255, 255, 0.7);font-size:0.875rem;margin:0;margin-top:1rem;text-align:center;'>This email was sent by Melpominee.app. If you didn't sign up with Melpominee.app, don't worry! You can just ignore this.</p>
                    </div>
                </body>
            </html>
            "
        );

        return user;
    }

    public User? RegistrationFinish(string email, string key)
    {
        User? user = null;
        // quick check validity of object
        if(string.IsNullOrEmpty(email)) 
        { 
            return user; 
        }

        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    // fetch user object
                    var sql =
                    @"
                        SELECT *
                        FROM melpominee_users
                        WHERE email = @Email;
                    ";
                    user = conn.QuerySingleOrDefault<User>(
                        sql, new {
                            Email = email
                        }
                    );

                    // validate returned results
                    if (user is null || 
                        string.IsNullOrEmpty(user.Password) || 
                        string.IsNullOrEmpty(user.ActivationKey))
                    {
                        trans.Rollback();
                        return null;
                    }

                    // compare activation key and querystring
                    if (key != user.ActivationKey)
                    {
                        trans.Rollback();
                        return null;
                    }

                    // update fields
                    user.ActivationKey = null;
                    user.ActivationCompleted = DateTime.UtcNow;
                    user.Active = true;

                    // make db query
                    sql =
                    @"
                        UPDATE melpominee_users
                        SET ActivationKey = @ActivationKey,
                            ActivationCompleted = CURRENT_TIMESTAMP,
                            Active = @Active
                        WHERE Email = @Email;
                    ";
                    if (conn.Execute(sql, user) <= 0)
                    {
                        trans.Rollback();
                        return null;
                    }
                    trans.Commit();
                }
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        return user;
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

    public static bool VerifyPassword(string? hashed, string? password)
    {
        if (string.IsNullOrEmpty(hashed) || string.IsNullOrEmpty(password))
        {
            // bad inputs, just return false
            return false;
        }
        byte[] hashedBytes = Convert.FromBase64String(hashed);
        byte[] salt = new byte[_saltSize];
        Buffer.BlockCopy(hashedBytes, 1 + _passwordBytes, salt, 0, _saltSize);
        string newHashedPassword = HashPassword(salt, password);
        return hashed == newHashedPassword;
    }
}