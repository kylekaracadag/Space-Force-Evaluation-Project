namespace SpaceForceEvaluationAppLibrary.DataAccess;

public class MongoDailyQuestionsData : IDailyQuestionsData
{
    private readonly IDbConnection _db;
    private readonly IMongoCollection<DailyQuestionsModel> _questions;

    public MongoDailyQuestionsData(IDbConnection db)
    {
        _db = db;
        _questions = db.DailyQuestionsCollection;
    }

    public async Task<DailyQuestionsModel> GetDailyQuestion(string Id)
    {
        var returned = await _questions.FindAsync(q => q.Id == Id);
        return returned.FirstOrDefault();

    }
}