namespace HumorQuiz;

/// <summary>
/// Окно 1: приветствие и ввод имени.
/// Также управляет переходами между окнами викторины.
/// </summary>
public class StartForm : Form
{
    private readonly TextBox _nameTextBox;

    public StartForm()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        Text = "Юмористическая викторина";
        ClientSize = new Size(520, 360);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.Background;
        Font = Theme.NormalFont;

        var titleLabel = new Label
        {
            Text = "Викторина «Смех да и только»",
            Font = Theme.TitleFont,
            ForeColor = Theme.Accent,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(20, 25),
            Size = new Size(480, 40),
        };

        var rulesLabel = new Label
        {
            Text = "Тебя ждут 15 лёгких вопросов с подвохом.\n" +
                   "Ошибёшься — придётся отвечать заново, пока не угадаешь.\n" +
                   "Думай головой, а не клавиатурой!",
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Theme.Text,
            Location = new Point(20, 80),
            Size = new Size(480, 80),
        };

        var nameLabel = new Label
        {
            Text = "Как тебя зовут?",
            ForeColor = Theme.Text,
            Location = new Point(110, 180),
            AutoSize = true,
        };

        _nameTextBox = new TextBox
        {
            Location = new Point(110, 205),
            Size = new Size(300, 30),
            MaxLength = 30,
            PlaceholderText = "Например, Капитан Очевидность",
        };

        var startButton = Theme.CreateButton("Начать викторину", new Point(160, 255), new Size(200, 45));
        startButton.Click += (_, _) => StartQuiz();

        var exitButton = new LinkLabel
        {
            Text = "Выход",
            LinkColor = Theme.Muted,
            Location = new Point(235, 315),
            AutoSize = true,
        };
        exitButton.LinkClicked += (_, _) => Close();

        AcceptButton = startButton;
        Controls.AddRange(new Control[] { titleLabel, rulesLabel, nameLabel, _nameTextBox, startButton, exitButton });
        ResumeLayout(false);
    }

    private void StartQuiz()
    {
        string name = _nameTextBox.Text.Trim();
        if (name.Length == 0)
        {
            MessageBox.Show(
                "Анонимов не принимаем! Введи хотя бы какое-нибудь имя.",
                "Кто здесь?",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            _nameTextBox.Focus();
            return;
        }

        var session = new QuizSession(name, QuestionBank.GetQuestions());
        Hide();
        OpenQuestion(session, previousJoke: null, location: Location);
    }

    /// <summary>Открывает окно с текущим вопросом сессии.</summary>
    private void OpenQuestion(QuizSession session, string? previousJoke, Point location)
    {
        var questionForm = new QuestionForm(session, previousJoke)
        {
            StartPosition = FormStartPosition.Manual,
            Location = location,
        };

        questionForm.FormClosed += (_, _) =>
        {
            if (!questionForm.AnsweredCorrectly)
            {
                // Игрок сдался и закрыл окно — возвращаемся на старт.
                ShowStart();
                return;
            }

            string joke = session.CurrentQuestion.Joke;
            if (session.IsLastQuestion)
            {
                OpenResult(session, questionForm.Location);
            }
            else
            {
                session.MoveNext();
                OpenQuestion(session, joke, questionForm.Location);
            }
        };

        questionForm.Show();
    }

    /// <summary>Открывает окно с итогами.</summary>
    private void OpenResult(QuizSession session, Point location)
    {
        var resultForm = new ResultForm(session)
        {
            StartPosition = FormStartPosition.Manual,
            Location = location,
        };

        resultForm.FormClosed += (_, _) =>
        {
            if (resultForm.PlayAgain)
            {
                var newSession = new QuizSession(session.PlayerName, QuestionBank.GetQuestions());
                OpenQuestion(newSession, previousJoke: null, location: resultForm.Location);
            }
            else
            {
                ShowStart();
            }
        };

        resultForm.Show();
    }

    private void ShowStart()
    {
        Show();
        Activate();
        _nameTextBox.Focus();
    }
}
