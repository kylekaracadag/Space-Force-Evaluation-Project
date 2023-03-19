﻿namespace SpaceForceEvaluationAppLibrary.Models;

public class InjectedQuestionsModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public int QuestionID { get; set; }
    public string text { get; set; }
    public string category { get; set; }
    public float questionWeight { get; set; }
    public int questionPriority { get; set; }
    public List<string> sendTo { get; set; }
}

