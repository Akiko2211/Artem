namespace HumorQuiz;

/// <summary>Общие цвета, шрифты и стиль кнопок для всех окон.</summary>
public static class Theme
{
    public static readonly Color Background = Color.FromArgb(255, 250, 235);
    public static readonly Color Text = Color.FromArgb(40, 40, 40);
    public static readonly Color Accent = Color.FromArgb(230, 100, 30);
    public static readonly Color Success = Color.FromArgb(40, 140, 70);
    public static readonly Color Muted = Color.FromArgb(120, 120, 120);

    public static readonly Font TitleFont = new("Segoe UI", 18f, FontStyle.Bold);
    public static readonly Font HeaderFont = new("Segoe UI", 14f, FontStyle.Bold);
    public static readonly Font QuestionFont = new("Segoe UI", 13f, FontStyle.Bold);
    public static readonly Font NormalFont = new("Segoe UI", 11f);
    public static readonly Font SmallFont = new("Segoe UI", 9.5f, FontStyle.Italic);

    public static Button CreateButton(string text, Point location, Size size)
    {
        var button = new Button
        {
            Text = text,
            Location = location,
            Size = size,
            BackColor = Accent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            Cursor = Cursors.Hand,
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }
}
