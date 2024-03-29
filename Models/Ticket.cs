﻿namespace WebAutoTest.Models;

public class Ticket
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FromIndex { get; set; }
    public int QuestionsCount { get; set; }
    public int CorrectCount { get; set; }
    public bool IsTraining { get; set; }

    public Ticket(int userId, int fromIndex, int questionsCount)
    {
        UserId = userId;
        FromIndex = fromIndex;
        QuestionsCount = questionsCount;
        CorrectCount = 0;
    }

    public Ticket()
    {

    }
}
