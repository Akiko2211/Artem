namespace HumorQuiz;

public class Question
{
    public string Text { get; }
    public string[] Answers { get; }
    public int CorrectIndex { get; }

    public string Joke { get; }

    public Question(string text, string[] answers, int correctIndex, string joke)
    {
        Text = text;
        Answers = answers;
        CorrectIndex = correctIndex;
        Joke = joke;
    }

    public string CorrectAnswer => Answers[CorrectIndex];
}
