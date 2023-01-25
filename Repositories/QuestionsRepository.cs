using Microsoft.Data.Sqlite;
using WebAutoTest.Models;

namespace WebAutoTest.Repositories;

public class QuestionsRepository
{
    private const string _connectionString = "Data Source=autotest.db";
    private SqliteConnection _connection;

    public QuestionsRepository()
    {
        _connection = new SqliteConnection(_connectionString);   
    }


    public int GetQuestionsCount()
    {
        _connection.Open();
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM questions";

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


    public QuestionEntity GetQuestionById(int id)
    {
        _connection.Open();
        var question = new QuestionEntity();

        var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM questions WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.Prepare();

        var data = command.ExecuteReader();

        while (data.Read())
        {
            question.Id = data.GetInt32(0);
            question.Question = data.GetString(1);
            question.Description = data.GetString(2);
            question.Image = data.IsDBNull(3) ? null : data.GetString(3);
        }

        question.Choices = GetQuestionChoice(id);

        _connection.Close();
        return question;
    }

    public List<QuestionEntity> GetQuestionRange(int from, int count)
    {
        _connection.Open();
        var questions = new List<QuestionEntity>();


        for (var i = from; i < from + count; i++)
        {
            questions.Add(GetQuestionById(i));
        }

        _connection.Close();
        return questions;

    }


    public List<Choice> GetQuestionChoice(int questionId)
    {
        _connection.Open();
        var choices = new List<Choice>();

        var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM choices WHERE question_Id = @questionId";
        command.Parameters.AddWithValue("@questionId", questionId);
        command.Prepare();

        var data = command.ExecuteReader();

        while (data.Read())
        {
            var choice = new Choice();
            choice.Id = data.GetInt32(0);
            choice.Text = data.GetString(1);
            choice.Answer= data.GetBoolean(2);

            choices.Add(choice);
        }
        _connection.Close();
        return choices;
    }
}
