using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private static DatabaseManager _instance;
    public static DatabaseManager Instance => _instance;

    private const string ConnectionString = "Host=localhost;Username=postgres;Password=123456;Database=CityBuilder";

    private NpgsqlConnection _connection;
    private QueryHelper _queryHelper = new QueryHelper();

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public List<UserModel> GetAllUser()
    {
        string query = "Select u.*, ur.role_id, r.energy,r.money,r.food From users u, user_roles ur, resources r WHERE u.user_id = ur.user_id AND u.user_id = r.user_id";

        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            var result = _queryHelper.ExecuteReaderWithoutParametres(query, connection);

            List<UserModel> list = new();

            foreach (DbDataRecord parameter in result)
            {
                list.Add(new UserModel
                {
                    UserId = (int)parameter["user_id"],
                    Username = parameter["username"].ToString(),
                    PasswordHash = parameter["password_hash"].ToString(),
                    Email = parameter["email"].ToString(),
                    IsBanned = (bool)parameter["is_banned"],
                    CreatedAt = (DateTime)parameter["created_at"],
                    UserRoleID = (int)parameter["role_id"],
                    Energy = (int)parameter["energy"],
                    Money = (int)parameter["money"],
                    Food = (int)parameter["food"]
                });
            }
            return list;
        }
    }

    public bool BanUser(int userId, string banned)
    {
        try
        {
            string query = $"UPDATE public.users SET is_banned = {banned} WHERE user_id = @userId;";

            NpgsqlParameter[] parameters = new NpgsqlParameter[] {
                new NpgsqlParameter("@UserId", userId),
            };
            // PostgreSQL baðlantýsý oluþturma
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                return _queryHelper.ExecuteQuery(query, connection, parameters);
            }
        }
        catch (Exception ex)
        {
            // Hata durumunda konsola yazdýr
            Console.WriteLine($"Hata: {ex.Message}");
            return false;
        }
    }

    public bool UpdateUserRole(int userId, int roleId)
    {
        try
        {
            string query = "UPDATE public.user_roles SET role_id = @RoleId WHERE user_id = @UserId;";
            NpgsqlParameter[] parameters = new NpgsqlParameter[] {
                new NpgsqlParameter("@UserId", userId),
                new NpgsqlParameter("@RoleId", roleId),
            };
            // PostgreSQL baðlantýsý oluþturma
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                return _queryHelper.ExecuteQuery(query, connection, parameters);

            }
        }
        catch (Exception ex)
        {
            // Hata durumunda konsola yazdýr
            Console.WriteLine($"Hata: {ex.Message}");
            return false;
        }
    }

    public List<UserModel> OrderBy(string param, string type)
    {
        string query = $"Select u.*, ur.role_id, r.energy,r.money,r.food From users u, user_roles ur, resources r WHERE u.user_id = ur.user_id AND u.user_id = r.user_id ORDER BY {param} {type}";

        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            var result = _queryHelper.ExecuteReaderWithoutParametres(query, connection);

            List<UserModel> list = new();

            foreach (DbDataRecord parameter in result)
            {
                list.Add(new UserModel
                {
                    UserId = (int)parameter["user_id"],
                    Username = parameter["username"].ToString(),
                    PasswordHash = parameter["password_hash"].ToString(),
                    Email = parameter["email"].ToString(),
                    IsBanned = (bool)parameter["is_banned"],
                    CreatedAt = (DateTime)parameter["created_at"],
                    UserRoleID = (int)parameter["role_id"],
                    Energy = (int)parameter["energy"],
                    Money = (int)parameter["money"],
                    Food = (int)parameter["food"]
                });
            }
            return list;
        }
    }

    // Kayýt Olma
    public bool Signup(string username, string email, string password)
    {
        try
        {
            string hashedPassword = HashPassword(password);

            bool emailVerified = false;

            DateTime createdAt = DateTime.Now;

            string query = "INSERT INTO users (username, password_hash, email, email_verified, created_at) " +
                           "VALUES (@username, @passwordHash, @Email, @EmailVerified, @CreatedAt)";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
            new NpgsqlParameter("@username", username),
            new NpgsqlParameter("@passwordHash", hashedPassword),
            new NpgsqlParameter("@Email", email),
            new NpgsqlParameter("@EmailVerified", emailVerified),
            new NpgsqlParameter("@CreatedAt", createdAt)
            };

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                _queryHelper.ExecuteQuery(query, connection, parameters);
            }

            Debug.Log("Signup successful!");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Signup Error: " + ex.Message);
            return false;
        }
    }

    // Giriþ Yapma
    public UserModel Login(string username, string password)
    {
        try
        {
            string query = "SELECT u.*,ur.role_id FROM users u , user_roles ur  WHERE u.username = @username AND u.user_id = ur.user_id ";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@username", username)
            };

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                var result = _queryHelper.ExecuteQuerySingle(query, connection, parameters);

                if (result != null)
                {
                    UserModel model = new UserModel
                    {
                        UserId = (int)result["user_id"],
                        Username = result["username"].ToString(),
                        PasswordHash = result["password_hash"].ToString(),
                        Email = result["email"].ToString(),
                        IsBanned = (bool)result["is_banned"],
                        CreatedAt = (DateTime)result["created_at"],
                        UserRoleID = (int)result["role_id"]
                    };

                    if (VerifyPassword(password, model.PasswordHash))
                    {
                        Debug.Log("Login successful.");
                        return model;
                    }
                }
            }

            Debug.LogWarning("Login failed: Invalid username or password.");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError("Login Error: " + ex.Message);
            return null;
        }
    }

    // Þifreyi Hashleme
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Þifre doðrulama
    private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
    {
        string hashedEnteredPassword = HashPassword(enteredPassword);
        return hashedEnteredPassword == storedHashedPassword;
    }

    // Building ekleme
    public bool AddBuilding(int userId, BuildingModel model)
    {
        string query = "INSERT INTO Buildings (user_id, building_id, building_type, building_name, energy_cost, food_cost, money_cost, production_type, production_rate, created_at, position, building_size,rotation) " +
                       "VALUES (@UserId, @BuildingId, @BuildingType, @BuildingName, @EnergyCost, @FoodCost, @MoneyCost, @ProductionType, @ProductionRate, @CreatedAt, @Position, @BuildingSize,@Rotation)";

        NpgsqlParameter[] parameters = new NpgsqlParameter[] {
            new NpgsqlParameter("@UserId", userId),
            new NpgsqlParameter("@BuildingId", model.BuildingId),
            new NpgsqlParameter("@BuildingType", model.BuildingType.ToString()),
            new NpgsqlParameter("@BuildingName", model.BuildingName),
            new NpgsqlParameter("@EnergyCost", model.EnergyCost),
            new NpgsqlParameter("@FoodCost", model.FoodCost),
            new NpgsqlParameter("@MoneyCost", model.MoneyCost),
            new NpgsqlParameter("@ProductionType", model.ProductionType.ToString()),
            new NpgsqlParameter("@ProductionRate", model.ProductionRate),
            new NpgsqlParameter("@CreatedAt", DateTime.Now),
            new NpgsqlParameter("@Position", model.Position),
            new NpgsqlParameter("@BuildingSize", model.buildingSize.ToString()),
            new NpgsqlParameter("@Rotation",model.Rotation)
        };

        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            return _queryHelper.ExecuteQuery(query, connection, parameters);
        }
    }

    public List<(int id, int buildingId, string position, string rotation)> SelectBuilding(int userId)
    {
        string query = "SELECT * FROM Buildings WHERE user_id = @UserId";

        NpgsqlParameter[] parameters = new NpgsqlParameter[] {
            new NpgsqlParameter("@UserId", userId),
        };

        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            var result = _queryHelper.ExecuteReader(query, connection, parameters);
            List<(int id, int buildingId, string position, string rotation)> list = new();

            foreach (DbDataRecord parameter in result)
            {
                list.Add((
                    Convert.ToInt32(parameter["id"]),
                    Convert.ToInt32(parameter["building_id"]),
                    parameter["position"].ToString(),
                    parameter["rotation"].ToString()
                ));
            }
            return list;
        }
    }

    public bool DeleteBuilding(int id)
    {
        string query = "DELETE FROM buildings WHERE id = @Id";
        NpgsqlParameter[] parameters = new NpgsqlParameter[] {
            new NpgsqlParameter("@Id", id),
        };

        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            return _queryHelper.ExecuteQuery(query, connection, parameters);
        }
    }

    // Road ekleme
    public bool AddRoad(int userId, RoadModel model)
    {
        string query = "INSERT INTO Roads (user_id,road_id,money_cost,created_at,position,rotation)" +
                       "VALUES (@UserId,@RoadId,@MoneyCost,@CreatedAt,@Position,@Rotation)";
        NpgsqlParameter[] parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@UserId",userId),
            new NpgsqlParameter("@RoadId",model.RoadId),
            new NpgsqlParameter("@MoneyCost",model.MoneyCost),
            new NpgsqlParameter("@CreatedAt", DateTime.Now),
            new NpgsqlParameter("@Position",model.Position),
            new NpgsqlParameter("@Rotation",model.Rotation)
        };

        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            return _queryHelper.ExecuteQuery(query, connection, parameters);
        }
    }
    public List<(int id, int roadId, string position, string rotation)> SelectRoad(int userId)
    {
        string query = "SELECT * FROM Roads WHERE user_id = @UserId";

        NpgsqlParameter[] parameters = new NpgsqlParameter[] {
            new NpgsqlParameter("@UserId", userId),
        };

        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            var result = _queryHelper.ExecuteReader(query, connection, parameters);
            List<(int id, int roadId, string position, string rotation)> list = new();

            foreach (DbDataRecord parameter in result)
            {
                list.Add((
                    Convert.ToInt32(parameter["id"]),
                    Convert.ToInt32(parameter["road_id"]),
                    parameter["position"].ToString(),
                    parameter["rotation"].ToString()
                ));
            }
            return list;
        }
    }

    public bool DeleteRoad(int id)
    {
        string query = "DELETE FROM roads WHERE id = @Id";
        NpgsqlParameter[] parameters = new NpgsqlParameter[] {
            new NpgsqlParameter("@Id", id),
        };
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            return _queryHelper.ExecuteQuery(query, connection, parameters);
        }
    }

    // Kaynaklarý alma
    public (int Energy, int Food, int Money) GetResources(int userId)
    {
        string query = "SELECT energy, food, money FROM Resources WHERE user_id = @UserId";
        NpgsqlParameter[] parameters = new NpgsqlParameter[] {
            new NpgsqlParameter("@UserId", userId)
        };

        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Open();
            var result = _queryHelper.ExecuteQuerySingle(query, connection, parameters);
            if (result != null)
            {
                return (
                    Convert.ToInt32(result["energy"]),
                    Convert.ToInt32(result["food"]),
                    Convert.ToInt32(result["money"])
                );
            }
        }

        return (0, 0, 0); // Varsayýlan olarak 0 döndür
    }

    public void CallProcessBuildingProduction()
    {
        string query = "CALL produce_resources();";
        try
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                _queryHelper.ExecuteNonQuery(query, connection);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Prosedür çaðrýsý sýrasýnda hata: {ex.Message}");
        }
    }

    public void CallReduceBuildingCost()
    {
        string query = "CALL reduce_resources();";

        try
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                _queryHelper.ExecuteNonQuery(query, connection);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Prosedür çaðrýsý sýrasýnda hata: {ex.Message}");
        }
    }

}

