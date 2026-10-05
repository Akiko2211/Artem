namespace HumorQuiz;

public class QuestionForm : Form
{
    private static readonly string[] WrongMessages =
    {
        "Мимо! Даже кот, гуляющий по клавиатуре, попал бы точнее.",
        "Неправильно! Но попытка засчитана... в копилку ошибок.",
        "Нет-нет-нет! Мозг, проснись, у нас тут викторина!",
        "Ошибочка вышла. Подумай ещё — вопрос с подвохом!",
        "Это был смелый ответ. Смелый, но неправильный.",
        "Не угадал! Говорят, вторая попытка всегда удачнее.",
        "Увы! Правильный ответ где-то рядом, но не здесь.",
    };

    private readonly QuizSession _session;
    private readonly List<RadioButton> _answerButtons = new();
    private readonly Label _mistakesLabel;

    public bool AnsweredCorrectly { get; private set; }

    public QuestionForm(QuizSession session, string? previousJoke)
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        _session = session;
        Question question = session.CurrentQuestion;

        Text = $"Вопрос {session.QuestionNumber} из {session.TotalQuestions}";
        ClientSize = new Size(420, 290);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var jokeLabel = new Label
        {
            Text = previousJoke is null ? "" : "Верно! " + previousJoke,
            Location = new Point(20, 10),
            Size = new Size(380, 30),
        };

        var questionLabel = new Label
        {
            Text = question.Text,
            Location = new Point(20, 45),
            Size = new Size(380, 40),
        };

        int[] order = Enumerable.Range(0, question.Answers.Length)
            .OrderBy(_ => Random.Shared.Next())
            .ToArray();

        for (int i = 0; i < order.Length; i++)
        {
            int answerIndex = order[i];
            var radio = new RadioButton
            {
                Text = question.Answers[answerIndex],
                Tag = answerIndex,
                Location = new Point(20, 90 + i * 30),
                Size = new Size(380, 25),
            };
            _answerButtons.Add(radio);
            Controls.Add(radio);
        }

        _mistakesLabel = new Label
        {
            Location = new Point(20, 245),
            AutoSize = true,
        };
        UpdateMistakesLabel();

        var answerButton = new Button
        {
            Text = "Ответить",
            Location = new Point(300, 240),
            Size = new Size(100, 30),
        };
        answerButton.Click += (_, _) => CheckAnswer();
        AcceptButton = answerButton;

        Controls.AddRange(new Control[] { jokeLabel, questionLabel, _mistakesLabel, answerButton });

        FormClosing += OnFormClosing;
        ResumeLayout(false);
    }

    private void CheckAnswer()
    {
        RadioButton? selected = _answerButtons.FirstOrDefault(r => r.Checked);
        if (selected is null)
        {
            MessageBox.Show(
                "Сначала выбери вариант ответа. Телепатию мы пока не поддерживаем.",
                "Ничего не выбрано",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        if ((int)selected.Tag! == _session.CurrentQuestion.CorrectIndex)
        {
            AnsweredCorrectly = true;
            Close();
            return;
        }

        _session.RegisterMistake();
        UpdateMistakesLabel();

        string message = WrongMessages[Random.Shared.Next(WrongMessages.Length)];
        MessageBox.Show(
            message + "\n\nВыбери ответ заново.",
            "Неправильный ответ!",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);

        selected.Checked = false;
    }

    private void UpdateMistakesLabel()
    {
        _mistakesLabel.Text = $"Ошибок за игру: {_session.Mistakes}";
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        if (AnsweredCorrectly || e.CloseReason != CloseReason.UserClosing)
            return;

        DialogResult answer = MessageBox.Show(
            "Уже сдаёшься? Весь прогресс будет потерян!",
            "Побег с викторины",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (answer == DialogResult.No)
            e.Cancel = true;
    }
}
