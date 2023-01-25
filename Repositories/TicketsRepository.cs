using Microsoft.Data.Sqlite;
using WebAutoTest.Models;

namespace WebAutoTest.Repositories;

public class TicketsRepository
{
    private const string _connectionString = "Data Source=autotest.db";
    private SqliteConnection _connection;


    public TicketsRepository()
    {
        _connection = new SqliteConnection(_connectionString);
        AddTicketTable();
    }


    private void AddTicketTable()
    {
        _connection.Open();

        var command = _connection.CreateCommand();

        command.CommandText = "CREATE TABLE IF NOT EXISTS tickets(id INTEGER PRIMARY KEY AUTOINCREMENT," +
                              "user_id INTEGER, " +
                              "from_index INTEGER, " +
                              "questions_count INTEGER, " +
                              "correct_count INTEGER," +
                              "is_training BOOLEAN)";
        command.ExecuteNonQuery();


        command.CommandText = "CREATE TABLE IF NOT EXISTS tickets_data(id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                              "ticket_id INTEGER," +
                              "question_id INTEGER, " +
                              "choice_id INTGERER, " +
                              "answer BOOLEAN)";
        command.ExecuteNonQuery();

        _connection.Close();
    }


    public int GetLastRowId()
    {
        _connection.Open();

        int id = 0;
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT id FROM tickets ORDER BY id DESC LIMIT 1";

        var data = command.ExecuteReader();

        while (data.Read())
        {
            id = data.GetInt32(0);
        }

        _connection.Close();

        return id;
    }


    public void InsertTicket(Ticket ticket)
    {
        _connection.Open();

        var command = _connection.CreateCommand();
        
        command.CommandText = "INSERT INTO tickets(user_id, from_index, questions_count, correct_count, is_training)" +
                              "VALUES(@userId, @fromIndex, @questionsCount, @correctCount, @isTraining)";

        command.Parameters.AddWithValue("@userId", ticket.UserId);
        command.Parameters.AddWithValue("@fromIndex", ticket.FromIndex);
        command.Parameters.AddWithValue("@questionsCount", ticket.QuestionsCount);
        command.Parameters.AddWithValue("@correctCount", ticket.CorrectCount);
        command.Parameters.AddWithValue("@isTraining", ticket.IsTraining);

        command.Prepare();
        command.ExecuteNonQuery();

        _connection.Close();

    }


    public void InsertTicketData(TicketData ticketData)
    {
        _connection.Open();

        var command = _connection.CreateCommand();
        command.CommandText = "INSERT INTO tickets_data(ticket_id, question_id, choice_id, answer)" +
                              "VALUES(@ticketId, @questionId, @choiceId, @answer )";
        command.Parameters.AddWithValue("@ticketId" ,ticketData.TicketId);
        command.Parameters.AddWithValue("@questionId" ,ticketData.QuestionId);
        command.Parameters.AddWithValue("@choiceId" ,ticketData.ChoiceId);
        command.Parameters.AddWithValue("@answer" ,ticketData.Answer);
        command.Prepare();
        command.ExecuteNonQuery();

        _connection.Close();
    }


    public TicketData? GetTicketDataByQuestionId(int ticketId, int questionId)
    {
        _connection.Open();
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM tickets_data " +
                              $"WHERE ticket_id = {ticketId} and question_id = {questionId}";
        var data = command.ExecuteReader();

        var ticketData = new TicketData();
        while (data.Read())
        {
            ticketData.Id = data.GetInt32(0);
            ticketData.TicketId = data.GetInt32(1);
            ticketData.QuestionId = data.GetInt32(2);
            ticketData.ChoiceId = data.GetInt32(3);
            ticketData.Answer = data.GetBoolean(4);
        }

        if (ticketData.QuestionId == questionId)
        {
            return ticketData;
        }

        _connection.Close();
        return null;
    }


    public int GetTicketAnswerCount(int ticketId)
    {
        _connection.Open();
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM " +
                              $"tickets_data WHERE ticket_id = {ticketId}";

        var data = command.ExecuteReader();

        while (data.Read())
        {
            var count = data.GetInt32(0);
            _connection.Close();
            data.Close();
            return count;
        }

        _connection.Close();
        return 0;
    } 


    public void UpdateTicketCorrectCount(int ticketId)
    {
        _connection.Open();

        var command = _connection.CreateCommand();
        command.CommandText = "UPDATE tickets " +
                              $"SET correct_count = correct_count + 1 WHERE id = {ticketId}";
        command.ExecuteNonQuery();

        _connection.Close();
    }


    public List<TicketData> GetTicketDataById(int ticketId)
    {
        var ticketData = new List<TicketData>();

        _connection.Open();
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT question_id, choice_id, answer " +
                              $"FROM tickets_data WHERE ticket_id ={ticketId}";

        var data = command.ExecuteReader();
        while (data.Read())
        {
            var insertToModelTicketData = new TicketData()
            {
                QuestionId = data.GetInt32(0),
                ChoiceId = data.GetInt32(1),
                Answer = data.GetBoolean(2)
            };
            ticketData.Add(insertToModelTicketData);
        }
        _connection.Close();

        return ticketData;
    }


    public List<Ticket> GetTicketsByUserId(int userId)
    {
        var tickets = new List<Ticket>();

        _connection.Open();
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT id, from_index, questions_count, correct_count " +
                              $"FROM tickets  WHERE user_id = {userId} AND is_training = true";
        var data = command.ExecuteReader();
        while (data.Read())
        {
            var insertToModelTicket = new Ticket()
            {
                Id = data.GetInt32(0),
                FromIndex = data.GetInt32(1),
                QuestionsCount = data.GetInt32(2),
                CorrectCount = data.GetInt32(3),
                UserId = userId
            };
            tickets.Add(insertToModelTicket);
        }
        _connection.Close();

        return tickets;
    }


    public Ticket GetTicketById(int id, int userId)
    {
        _connection.Open();
        var ticket = new Ticket();

        var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM tickets WHERE id = @id AND user_id = @userId;";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Prepare();

        var data = cmd.ExecuteReader();
        while (data.Read())
        {
            ticket.Id = data.GetInt32(0);
            ticket.UserId = data.GetInt32(1);
            ticket.FromIndex = data.GetInt32(2);
            ticket.QuestionsCount = data.GetInt32(3);
            ticket.CorrectCount = data.GetInt32(4);
            ticket.IsTraining = data.GetBoolean(5);
        }

        _connection.Close();
        return ticket;
    }


    public void InsertUserTrainingTickets(int userId, int ticketsCount, int ticketQuestionsCount)
    {
        for (int i = 0; i <= ticketsCount; i++)
        {
            InsertTicket(new Ticket()
            {
                UserId = userId,
                CorrectCount = 0,
                IsTraining = true,
                FromIndex = i * ticketQuestionsCount + 1,
                QuestionsCount = ticketQuestionsCount

            });
        }
    }
}
