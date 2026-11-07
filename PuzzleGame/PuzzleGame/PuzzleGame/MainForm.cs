using System.Drawing;
using System.Windows.Forms;

namespace PuzzleGame;

public class MainForm : Form
{
    public MainForm()
    {
        Text = "Puzzle Game - Giải đố";
        ClientSize = new Size(420, 430);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(30, 34, 44);
        Font = new Font("Segoe UI", 11f);

        var lblTitle = new Label
        {
            Text = "PUZZLE GAME",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 24f, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 25),
            Size = new Size(420, 55)
        };

        var lblSub = new Label
        {
            Text = "Chọn một trò chơi giải đố",
            ForeColor = Color.FromArgb(170, 180, 200),
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(0, 82),
            Size = new Size(420, 28)
        };

        var btnSliding = MakeButton("1. Xếp hình số", 130, Color.FromArgb(52, 120, 220));
        var btnRiddle = MakeButton("2. Câu đố Việt Nam", 200, Color.FromArgb(46, 160, 110));
        var btnLights = MakeButton("3. Lights Out", 270, Color.FromArgb(220, 150, 40));
        var btnExit = MakeButton("Thoát", 350, Color.FromArgb(120, 125, 140));

        btnSliding.Click += (s, e) => { using var f = new SlidingPuzzleForm(); f.ShowDialog(this); };
        btnRiddle.Click += (s, e) => { using var f = new RiddleForm(); f.ShowDialog(this); };
        btnLights.Click += (s, e) => { using var f = new LightsOutForm(); f.ShowDialog(this); };
        btnExit.Click += (s, e) => Close();

        Controls.AddRange(new Control[] { lblTitle, lblSub, btnSliding, btnRiddle, btnLights, btnExit });
    }

    private static Button MakeButton(string text, int top, Color color)
    {
        var b = new Button
        {
            Text = text,
            Location = new Point(70, top),
            Size = new Size(280, 54),
            FlatStyle = FlatStyle.Flat,
            BackColor = color,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        b.FlatAppearance.BorderSize = 0;
        return b;
    }
}
