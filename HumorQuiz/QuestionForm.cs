namespace HumorQuiz;

/// <summary>
/// Окно 2: один вопрос с вариантами ответа.
/// Для каждого следующего вопроса открывается новое окно.
/// </summary>
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

    /// <summary>true, если игрок ответил правильно и окно закрылось для перехода дальше.</summary>
    public bool AnsweredCorrectly { get; private set; }

    public QuestionForm(QuizSession session, string? previousJoke)
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        _session = session;
        Question question = session.CurrentQuestion;

        Text = $"Вопрос {session.QuestionNumber} из {session.TotalQuestions}";
        ClientSize = new Size(560, 440);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Theme.Background;
        Font = Theme.NormalFont;

        var headerLabel = new Label
        {
            Text = $"Вопрос {session.QuestionNumber} из {session.TotalQuestions}",
            Font = Theme.HeaderFont,
            ForeColor = Theme.Accent,
            Location = new Point(20, 15),
            AutoSize = true,
        };

        var progressBar = new ProgressBar
        {
            Location = new Point(20, 50),
            Size = new Size(520, 12),
            Maximum = session.TotalQuestions,
            Value = session.QuestionNumber - 1,
        };

        var jokeLabel = new Label
        {
            Text = previousJoke is null ? "" : "Верно! " + previousJoke,
            ForeColor = Theme.Success,
            Font = Theme.SmallFont,
            Location = new Point(20, 70),
            Size = new Size(520, 36),
        };

        var questionLabel = new Label
        {
            Text = question.Text,
            Font = Theme.QuestionFont,
            ForeColor = Theme.Text,
            Location = new Point(20, 110),
            Size = new Size(520, 60),
        };

        var answersPanel = new Panel
        {
            Location = new Point(20, 175),
            Size = new Size(520, 180),
        };

        // Варианты ответов выводятся в случайном порядке, чтобы не запоминать позицию.
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
                ForeColor = Theme.Text,
                Location = new Point(10, 5 + i * 43),
                Size = new Size(500, 38),
                Cursor = Cursors.Hand,
            };
            _answerButtons.Add(radio);
            answersPanel.Controls.Add(radio);
        }

        _mistakesLabel = new Label
        {
            ForeColor = Theme.Muted,
            Location = new Point(20, 385),
            AutoSize = true,
        };
        UpdateMistakesLabel();

        var answerButton = Theme.CreateButton("Ответить", new Point(390, 370), new Size(150, 45));
        answerButton.Click += (_, _) => CheckAnswer();
        AcceptButton = answerButton;

        Controls.AddRange(new Control[]
        {
            headerLabel, progressBar, jokeLabel, questionLabel, answersPanel, _mistakesLabel, answerButton,
        });

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

        // Сбрасываем выбор — игрок возвращается к вопросу и отвечает снова.
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
