using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
namespace Melpominee.app.Utilities.Database;


public class DataContext
{
    private static Lazy<DataContext> _instance = new Lazy<DataContext>(() => new DataContext());
    public static DataContext Instance => _instance.Value;

    public DataContext() {}

    public IDbConnection Connect()
    {
        return new SqliteConnection("Data Source=data/melpominee.db");
    }

    public void Initalize()
    {
        using(var conn = Connect())
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS melpominee_users (
                    Email TEXT NOT NULL PRIMARY KEY,
                    Password TEXT NOT NULL,
                    ActivationKey TEXT,
                    ActivationRequested DATETIME,
                    ActivationCompleted DATETIME,
                    Active BOOL DEFAULT false
                );
                CREATE TABLE IF NOT EXISTS melpominee_users_rescue (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    Email TEXT NOT NULL,
                    RescueKey TEXT,
                    RescueRequested DATETIME,
                    RescueCompleted DATETIME,
                    FOREIGN KEY(Email) REFERENCES melpominee_users(Email)
                );
                CREATE TABLE IF NOT EXISTS melpominee_characters (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    concept TEXT NOT NULL,
                    chronicle TEXT NOT NULL,
                    ambition TEXT NOT NULL,
                    desire TEXT NOT NULL,
                    sire TEXT NOT NULL,
                    generation INTEGER NOT NULL,
                    clan TEXT NOT NULL,
                    predator_type TEXT NOT NULL,
                    hunger INTEGER NOT NULL,
                    resonance TEXT NOT NULL,
                    blood_potency INTEGER NOT NULL
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_attributes (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    sheet_id INTEGER NOT NULL,
                    attribute TEXT NOT NULL,
                    score INTEGER NOT NULL,
                    UNIQUE(sheet_id, attribute),
                    FOREIGN KEY(sheet_id) REFERENCES melpominee_characters(id)
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_skills (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    sheet_id INTEGER NOT NULL,
                    skill TEXT NOT NULL,
                    speciality TEXT NOT NULL,
                    score INTEGER NOT NULL,
                    UNIQUE(sheet_id, skill),
                    FOREIGN KEY(sheet_id) REFERENCES melpominee_characters(id)
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_secondary (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    sheet_id INTEGER NOT NULL,
                    stat_name TEXT NOT NULL,
                    base_value INTEGER NOT NULL,
                    superficial_damage INTEGER NOT NULL,
                    aggravated_damage INTEGER NOT NULL,
                    UNIQUE(sheet_id, stat_name),
                    FOREIGN KEY(sheet_id) REFERENCES melpominee_characters(id)
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_disciplines (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    sheet_id INTEGER NOT NULL,
                    discipline TEXT NOT NULL,
                    score INTEGER NOT NULL,
                    UNIQUE(sheet_id, discipline),
                    FOREIGN KEY(sheet_id) REFERENCES melpominee_characters(id)
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_discipline_powers (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    sheet_id INTEGER NOT NULL,
                    power_name TEXT NOT NULL,
                    UNIQUE(sheet_id, power_name),
                    FOREIGN KEY(sheet_id) REFERENCES melpominee_characters(id)
                );
            ";
            conn.Execute(sql);
        }
    }
}