using System.Drawing;
using System.Windows.Forms;

namespace PuzzleGame;

/// <summary>Lights Out: bấm một ô sẽ đảo trạng thái ô đó và 4 ô kề. Mục tiêu: tắt hết đèn.</summary>
public class LightsOutForm : Form
{
    private const int BoardPixels = 450;

    private int size = 5;
    private bool[,] lights;
    private bool[,] initial;
    private bool[,] solution;    // các ô cần bấm (theo ván gốc)
    private bool[,] playerClicks; // số lần bấm (chẵn/lẻ) của người chơi
    private Button[,] cells;
    private int moves;

    private readonly Panel boardPanel;
    private readonly Label lblInfo;
    private readonly ComboBox cboSize;
    private readonly Random rng = new Random();

    public LightsOutForm()
    {
        Text = "Lights Out";
        ClientSize = new Size(480, 620);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(30, 34, 44);
        Font = new Font("Segoe UI", 10f);

        var lblSize = new Label
        {
            Text = "Kích thước:",
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(20, 22)
        };

        cboSize = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(105, 18),
            Width = 80
        };
        cboSize.Items.AddRange(new object[] { "3 x 3", "4 x 4", "5 x 5" });
        cboSize.SelectedIndex = 2;

        var btnNew = MakeButton("Ván mới", 200, 15, Color.FromArgb(52, 120, 220));
        var btnReset = MakeButton("Chơi lại", 315, 15, Color.FromArgb(120, 125, 140));
        var btnHint = MakeButton("Gợi ý", 20, 565, Color.FromArgb(220, 150, 40));

        btnNew.Click += (s, e) => NewGame();
        btnReset.Click += (s, e) => ResetGame();
        btnHint.Click += (s, e) => ShowHint();

        lblInfo = new Label
        {
            ForeColor = Color.FromArgb(200, 210, 230),
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            Location = new Point(20, 62),
            Size = new Size(440, 28)
        };

        boardPanel = new Panel
        {
            Location = new Point(15, 100),
            Size = new Size(BoardPixels, BoardPixels),
            BackColor = Color.FromArgb(45, 50, 64)
        };

        Controls.AddRange(new Control[] { lblSize, cboSize, btnNew, btnReset, btnHint, lblInfo, boardPanel });

        Shown += (s, e) => NewGame();
    }

    private static Button MakeButton(string text, int left, int top, Color color)
    {
        var b = new Button
        {
            Text = text,
            Location = new Point(left, top),
            Size = new Size(100, 32),
            FlatStyle = FlatStyle.Flat,
            BackColor = color,
            ForeColor = Color.White
        };
        b.FlatAppearance.BorderSize = 0;
        return b;
    }

    private void NewGame()
    {
        size = cboSize.SelectedIndex + 3;
        lights = new bool[size, size];
        solution = new bool[size, size];
        playerClicks = new bool[size, size];

        // Bắt đầu từ bảng tắt hết rồi bấm ngẫu nhiên -> luôn có lời giải.
        do
        {
            Array.Clear(lights, 0, lights.Length);
            Array.Clear(solution, 0, solution.Length);
            int presses = size * size;
            for (int i = 0; i < presses; i++)
            {
                int r = rng.Next(size);
                int c = rng.Next(size);
                Toggle(r, c);
                solution[r, c] = !solution[r, c];
            }
        } while (AllOff());

        initial = (bool[,])lights.Clone();
        BuildCells();
        moves = 0;
        Array.Clear(playerClicks, 0, playerClicks.Length);
        RefreshCells();
        UpdateInfo();
        ActiveControl = null;
    }

    private void ResetGame()
    {
        if (initial == null) return;
        lights = (bool[,])initial.Clone();
        Array.Clear(playerClicks, 0, playerClicks.Length);
        moves = 0;
        RefreshCells();
        UpdateInfo();
    }

    private void Toggle(int r, int c)
    {
        Flip(r, c);
        Flip(r - 1, c);
        Flip(r + 1, c);
        Flip(r, c - 1);
        Flip(r, c + 1);
    }

    private void Flip(int r, int c)
    {
        if (r < 0 || c < 0 || r >= size || c >= size) return;
        lights[r, c] = !lights[r, c];
    }

    private bool AllOff()
    {
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                if (lights[r, c]) return false;
        return true;
    }

    private void BuildCells()
    {
        boardPanel.Controls.Clear();
        cells = new Button[size, size];
        int cell = BoardPixels / size;

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                int rr = r, cc = c;
                var b = new Button
                {
                    Size = new Size(cell - 6, cell - 6),
                    Location = new Point(c * cell + 3, r * cell + 3),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                    ForeColor = Color.White,
                    TabStop = false
                };
                b.FlatAppearance.BorderSize = 0;
                b.Click += (s, e) => OnCellClick(rr, cc);
                cells[r, c] = b;
                boardPanel.Controls.Add(b);
            }
        }
    }

    private void RefreshCells()
    {
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                var b = cells[r, c];
                b.Text = "";
                b.BackColor = lights[r, c] ? Color.FromArgb(255, 210, 60) : Color.FromArgb(70, 76, 92);
            }
        }
    }

    private void OnCellClick(int r, int c)
    {
        Toggle(r, c);
        playerClicks[r, c] = !playerClicks[r, c];
        moves++;
        RefreshCells();
        UpdateInfo();

        if (AllOff())
        {
            MessageBox.Show($"Bạn đã tắt hết đèn sau {moves} lần bấm!", "Thắng rồi!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // Gợi ý: đánh dấu một ô mà lời giải còn cần bấm.
    private void ShowHint()
    {
        if (lights == null || AllOff()) return;

        // Lời giải còn lại = solution XOR playerClicks (bấm 2 lần = không bấm).
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (solution[r, c] != playerClicks[r, c])
                {
                    RefreshCells();
                    cells[r, c].Text = "?";
                    return;
                }
            }
        }
    }

    private void UpdateInfo()
    {
        int on = 0;
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                if (lights[r, c]) on++;
        lblInfo.Text = $"Số lần bấm: {moves}      Đèn đang sáng: {on}";
    }
}
