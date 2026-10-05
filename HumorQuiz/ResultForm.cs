namespace HumorQuiz;

/// <summary>Окно 3: итоги викторины.</summary>
public class ResultForm : Form
{
    /// <summary>true, если игрок нажал «Сыграть ещё раз».</summary>
    public bool PlayAgain { get; private set; }

    public ResultForm(QuizSession session)
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        Text = "Итоги викторины";
        ClientSize = new Size(560, 400);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Theme.Background;
        Font = Theme.NormalFont;

        var (title, comment) = GetRank(session.Mistakes);

        var congratsLabel = new Label
        {
            Text = $"Поздравляем, {session.PlayerName}!",
            Font = Theme.TitleFont,
            ForeColor = Theme.Accent,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(20, 25),
            Size = new Size(520, 45),
        };

        var statsLabel = new Label
        {
            Text = $"Ты ответил(а) на все {session.TotalQuestions} вопросов.\n" +
                   $"Неправильных попыток: {session.Mistakes}",
            ForeColor = Theme.Text,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(20, 85),
            Size = new Size(520, 55),
        };

        var rankLabel = new Label
        {
            Text = $"Твоё звание: «{title}»",
            Font = Theme.HeaderFont,
            ForeColor = Theme.Success,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(20, 155),
            Size = new Size(520, 40),
        };

        var commentLabel = new Label
        {
            Text = comment,
            ForeColor = Theme.Text,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(20, 200),
            Size = new Size(520, 70),
        };

        var againButton = Theme.CreateButton("Сыграть ещё раз", new Point(95, 300), new Size(180, 45));
        againButton.Click += (_, _) =>
        {
            PlayAgain = true;
            Close();
        };

        var exitButton = Theme.CreateButton("Выход", new Point(285, 300), new Size(180, 45));
        exitButton.BackColor = Theme.Muted;
        exitButton.Click += (_, _) => Application.Exit();

        AcceptButton = againButton;
        Controls.AddRange(new Control[] { congratsLabel, statsLabel, rankLabel, commentLabel, againButton, exitButton });
        ResumeLayout(false);
    }

    private static (string Title, string Comment) GetRank(int mistakes) => mistakes switch
    {
        0 => ("Гений юмора",
              "Ни одной ошибки! Ты либо очень умный, либо уже проходил эту викторину."),
        <= 3 => ("Мастер подвоха",
                 "Почти идеально! Пара ошибок — это просто для разминки."),
        <= 7 => ("Весёлый эрудит",
                 "Неплохо! Подвохи иногда побеждали, но ты не сдавался."),
        <= 12 => ("Стажёр-юморист",
                  "Ты дошёл до конца, а это главное. Шутки тоже требуют тренировки!"),
        _ => ("Кот на клавиатуре",
              "Кажется, ответы выбирались методом тыка. Но упорство — достойно уважения!"),
    };
}
