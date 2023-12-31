﻿@inject IClimateQuestionsData questionData
@inject IMonthlyQuestionsData MonthlyQuestionData
@inject IUserData userData
@inject ISurveyData surveyData
@inject NavigationManager navManager
@inject AuthenticationStateProvider authProvider


<div class= maincomponent style = "visibility: @showQuestion" >
    < p > Please answer the following question to the best of your ability</p>
    @if (@SurveyQuestionType == 2)
    {
        < h5 > This question is in regards to your teammate<b>@teammate</ b ></ h5 >
    }
    < h5 > @TestQuestion </ h5 >



    < EditForm Model = "@surveyModel" OnValidSubmit = "@HandleSubmit" id = "MonthlyQuestionForm" >
        < InputRadioGroup @bind - Value = "@surveyModel.response" >

            < InputRadio Value = "1" > 1 </ InputRadio >
            < label > Strongly Disagree & ensp;</ label >

            < InputRadio Value = "2" > 2 </ InputRadio >
            < label > Somewhat Disagree & ensp;</ label >

            < InputRadio Value = "3" > 3 </ InputRadio >
            < label > Neither Agree nor Disagree &ensp;</ label >

            < InputRadio Value = "4" > 4 </ InputRadio >
            < label > Somewhat Agree & ensp;</ label >

            < InputRadio Value = "5" > 5 </ InputRadio >
            < label > Strongly Agree & ensp;</ label >
        </ InputRadioGroup >
        < div >
            < button type = "submit" > Submit </ button >
        </ div >
    </ EditForm >

</ div >

@code {
    [Parameter]
    public string questionType { get; set; }



    DateTime surveyDate;
    public string teammate;
public string questionSubjectID;
public int SurveyQuestionType;
public int questionSubject;
public int questionResponse;
public int questionNumber;
public string questionCategory;
public string surveyTaker;
public string surveySubject;
public string takerTeamID;
public string takerName;
private SurveyModel surveyModel = new SurveyModel();

public string TestQuestion = "";
public string showQuestion = "";
private Random rnd = new Random();



// Function for handling when a response is submitted.
// needs to be fixed to properly save questions
private async Task HandleSubmit()
{
    questionResponse = surveyModel.response;
    surveyModel.question = TestQuestion;
    surveyModel.surveyID = questionNumber;
    surveyModel.category = questionCategory;
    surveyModel.takerID = surveyTaker;
    surveyModel.subjectID = surveySubject;
    surveyModel.teamID = takerTeamID;
    surveyModel.name = takerName;
    surveyDate = DateTime.Today;
    await surveyData.CreateSurvey(surveyModel);
    await UpdateUser();
    ShowDiv();
}

// Update the question date for the user answering the question
private async Task UpdateUser()
{
    var questionTaker = await AuthenticationStateProviderHelpers.GetUserFromAuth(authProvider, userData);

    // Update the date that the user is taking the survey
    if (questionType == "Monthly")
    {
        questionTaker.monthlyDate = surveyDate;
    }
    else if (questionType == "Monthly")
    {
        questionTaker.MonthlyDate = surveyDate;
    }
    else
    {
        questionTaker.dailyDate = surveyDate;
    }


    await userData.UpdateUser(questionTaker);
}

// Current number of climate questions, this will need to be non-static at some point
private int getMaxClimate()
{
    return 3;
    //166;
}
// Current number of personelle questions, this will need to be non-static at some point
private int getMaxMonthly()
{
    return 2;
    //236;
}
// Function to get a random number to pull a question from the list
private int GetMonthlyQuestionNumber()
{
    return rnd.Next(1, 2/*getMaxMonthly()*/);
}
// Function to get a random number to pull a question from the climate list
private int GetClimateQuestionNumber()
{
    return rnd.Next(1, 3/*getMaxClimate()*/);
}

private string GetCategory()
{
    int categoryNumber = 32;
    switch (categoryNumber)
    {
        case 1:
            return "Learning goal orientation";
        case 2:
            return "Accountability";
        case 3:
            return "Feedback Seeking";
        case 4:
            return "Psychological Saftey";
        case 5:
            return "Interpersonal Justice";
        case 6:
            return "General Risk Propensity";
        case 8:
            return "DEI";
        case 9:
            return "Honesty/Candor";
        case 10:
            return "Fairness";
        case 11:
            return "Innovation Climate";
        case 12:
            return "Innovator";
        case 13:
            return "Information Sharing";
        case 14:
            return "Intellectual Stimulation";
        case 15:
            return "Job Dedication";
        case 16:
            return "Supporting";
        case 17:
            return "OCB-O";
        case 18:
            return "Trust";
        case 19:
            return "Servant Leadership";
        case 20:
            return "Moral strivings";
        case 21:
            return "Informational Justice";
        case 22:
            return "Resource Availability";
        case 23:
            return "TP";
        case 24:
            return "OCB-I";
        case 25:
            return "Feedback from Agents";
        case 26:
            return "Communication/Cooperation";
        case 27:
            return "Supervisor Task Support";
        case 28:
            return "Moral Courage";
        case 29:
            return "Risk Attitude ";
        case 30:
            return "Managerial Behavior: Broker";
        case 31:
            return "Integrity";
        case 32:
            return "Inclusion";
        case 33:
            return "Inclusion";
        default:
            return "Inclusion";
    }
}
public async Task GenerateQuestion()
{
    // Find out which type of question is going to be displayed
    // If 1, Question will be Climate
    GetQuestionType();
    var user = await AuthenticationStateProviderHelpers.GetUserFromAuth(authProvider, userData);
    if (SurveyQuestionType == 1)
    {
        MonthlyQuestionsModel MonthlyQuestionModel = await MonthlyQuestionData.GetMonthlyQuestion(29);
        //await MonthlyQuestionData.GetMonthlyQuestionsByCategory(GetCategory());
        TestQuestion = MonthlyQuestionModel.text;
        questionNumber = MonthlyQuestionModel.QuestionID;
        questionCategory = MonthlyQuestionModel.category;
        surveyTaker = user.userID;
    }
    // Else, Question will be Personnel
    else
    {
        // Pull in a list of users
        string subjectID = await GetListOfUsers(user.teamID);
        // If the question is about the person taking the survey, remove them as a subject, and pull a new question.
        if (subjectID == user.userID)
        {
            StateHasChanged();
            GenerateQuestion();
        }
        else
        {
            UserModel subject = await userData.GetUser(subjectID);
            teammate = subject.firstName + " " + subject.lastName;
            MonthlyQuestionsModel MonthlyQuestionModel = await MonthlyQuestionData.GetMonthlyQuestion(GetMonthlyQuestionNumber());
            TestQuestion = MonthlyQuestionModel.text;
            questionNumber = MonthlyQuestionModel.QuestionID;
            questionCategory = MonthlyQuestionModel.category;
            surveyTaker = user.userID;
            surveySubject = subject.userID;
        }

    }
    takerName = user.firstName + " " + user.lastName;
    user.dailyDate = surveyDate;
    takerTeamID = user.teamID;
}

// Function to pull a list of users that are in the same "team" as the survey taker
private async Task<string> GetListOfUsers(string teamId)
{
    var results = await userData.GetUsersFromTeam(teamId);
    results.ToList();
    int questionSubject = 0;
    //rnd.Next(1, results.Count);
    // Get the user associated with int questionSubject
    System.Console.WriteLine(results.Count);
    return results[0/*questionSubject*/].userID;
}


// Function to choose the type of question
private int GetQuestionType()
{
    return SurveyQuestionType = 1;
    //rnd.Next(1, 3);
}
protected override async Task OnInitializedAsync()
{

    await GenerateQuestion();
}

// Function to hide the question once it has been taken.
private void ShowDiv()
{
    if (showQuestion == "")
    {
        showQuestion = "hidden";
    }
    else
    {
        showQuestion = "";
    }
}

}
