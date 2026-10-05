namespace HumorQuiz;

public class StartForm : Form
{
    private readonly TextBox _nameTextBox;

    public StartForm()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;

        Text = "Юмористическая викторина";
        ClientSize = new Size(400, 210);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var rulesLabel = new Label
        {
            Text = "Викторина из 15 вопросов с подвохом.\n" +
                   "Если ошибёшься — придётся отвечать заново.",
            Location = new Point(20, 15),
            Size = new Size(360, 40),
        };

        var nameLabel = new Label
        {
            Text = "Как тебя зовут?",
            Location = new Point(20, 70),
            AutoSize = true,
        };

        _nameTextBox = new TextBox
        {
            Location = new Point(20, 92),
            Size = new Size(360, 23),
            MaxLength = 30,
        };

        var startButton = new Button
        {
            Text = "Начать",
            Location = new Point(20, 150),
            Size = new Size(120, 30),
        };
        startButton.Click += (_, _) => StartQuiz();

        var exitButton = new Button
        {
            Text = "Выход",
            Location = new Point(260, 150),
            Size = new Size(120, 30),
        };
        exitButton.Click += (_, _) => Close();

        AcceptButton = startButton;
        Controls.AddRange(new Control[] { rulesLabel, nameLabel, _nameTextBox, startButton, exitButton });
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
