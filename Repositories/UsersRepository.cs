using WebAutoTest.Models;
using Microsoft.Data.Sqlite;


namespace WebAutoTest.Repositories;

public class UsersRepository
{
    private const string ConnectionDb = "Data Source=users.db";
    private SqliteConnection connection;
    private SqliteCommand command;

    public UsersRepository()
    {
        OpenConnection();
        AddUsersTable();
    }

    public void OpenConnection()
    {
        connection = new SqliteConnection(ConnectionDb);
        connection.Open();
    }

    public void AddUsersTable() // Yangi users table ochib beradi
    {
        command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS users(id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                "name TEXT," +
                                "phone TEXT," +
                                "password TEXT)";
        command.ExecuteNonQuery();
    }

    public void InsertUser(User user) // Databasaga userni qoshib qoyadi (kerakli malumotlarini tanlab olib)
    {
        command.CommandText = $"INSERT INTO users(name, phone, password) " +
                              $"VALUES ('{user.Name}', '{user.PhoneNumber}', '{user.Password}')";
        command.ExecuteNonQuery();
    }

    public List<User> GetUsers() // Databasadan Hamma Userlarni malumotlarini olib beradi.
    {
        var users = new List<User>();
        command.CommandText = "SELECT * FROM users";
        var data = command.ExecuteReader();

        while (data.Read())
        {
            var user = new User();
            user.Index = data.GetInt32(0);
            user.Name = data.GetString(1);
            user.PhoneNumber = data.GetString(2);

            users.Add(user);
        }

        return users;
    }

    public User GetUsersByIndex(int index) // Index boyicha databasadan bita Userni malumotini olib beradi.
    {
        var user = new User();

        command.CommandText = $"SELECT * FROM users WHERE id = {index}";
        var data = command.ExecuteReader();

        while (data.Read())
        {
            user.Index = data.GetInt32(0);
            user.Name = data.GetString(1);
            user.PhoneNumber = data.GetString(2);
        }
        return user;
    }

    public User GetUsersByPhoneNumber(string phoneNumber) // PhoneNumber boyicha databasadan bita Userni malumotini olib beradi.
    {
        var user = new User();

        command.CommandText = $"SELECT * FROM users WHERE phone = {phoneNumber}";
        var data = command.ExecuteReader();

        while (data.Read())
        {
            user.Index = data.GetInt32(0);
            user.Name = data.GetString(1);
            user.PhoneNumber = data.GetString(2);
            user.Password= data.GetString(3);
        }
        return user;
    }

    public void DeleteUserByIndex(int index) // Index boyicha Userni databasadan ochirib tashlaydi.
    {
        command.CommandText = $"DELETE FROM users WHERE id = {index}";
        command.ExecuteNonQuery();
    }

    public void UpdateUserByIndex(User user) // Index boyicha Userni (berilgan) malumotlarini ozgartirib beradi.
    {
        command.CommandText = $"UPDATE users SET name = '{user.Name}', phone = '{user.PhoneNumber}', password = '{user.Password}' WHERE id = '{user.Index}'";
        command.ExecuteNonQuery();
    }
}