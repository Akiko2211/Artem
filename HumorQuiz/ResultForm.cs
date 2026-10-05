namespace HumorQuiz;

public class ResultForm : Form
{
    public bool PlayAgain { get; private set; }

    public ResultForm(QuizSession session)
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        Text = "Итоги викторины";
        ClientSize = new Size(400, 210);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var (title, comment) = GetRank(session.Mistakes);

        var resultLabel = new Label
        {
            Text = $"Поздравляем, {session.PlayerName}!\n\n" +
                   $"Ты ответил(а) на все {session.TotalQuestions} вопросов.\n" +
                   $"Неправильных попыток: {session.Mistakes}\n\n" +
                   $"Твоё звание: «{title}»\n" +
                   comment,
            Location = new Point(20, 15),
            Size = new Size(360, 125),
        };

        var againButton = new Button
        {
            Text = "Сыграть ещё раз",
            Location = new Point(20, 160),
            Size = new Size(120, 30),
        };
        againButton.Click += (_, _) =>
        {
            PlayAgain = true;
            Close();
        };

        var exitButton = new Button
        {
            Text = "Выход",
            Location = new Point(260, 160),
            Size = new Size(120, 30),
        };
        exitButton.Click += (_, _) => Application.Exit();

        AcceptButton = againButton;
        Controls.AddRange(new Control[] { resultLabel, againButton, exitButton });
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
