namespace HumorQuiz;

/// <summary>Состояние одной игры: игрок, вопросы, текущий вопрос и ошибки.</summary>
public class QuizSession
{
    public string PlayerName { get; }
    public List<Question> Questions { get; }
    public int CurrentIndex { get; private set; }
    public int Mistakes { get; private set; }

    public QuizSession(string playerName, List<Question> questions)
    {
        PlayerName = playerName;
        Questions = questions;
    }

    public Question CurrentQuestion => Questions[CurrentIndex];
    public int QuestionNumber => CurrentIndex + 1;
    public int TotalQuestions => Questions.Count;
    public bool IsLastQuestion => CurrentIndex == Questions.Count - 1;

    public void RegisterMistake() => Mistakes++;

    public void MoveNext() => CurrentIndex++;
}
