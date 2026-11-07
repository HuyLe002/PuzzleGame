using System.Drawing;
using System.Windows.Forms;

namespace PuzzleGame;

/// <summary>Xếp hình số (15-puzzle). Dùng chuột hoặc phím mũi tên.</summary>
public class SlidingPuzzleForm : Form
{
    private const int BoardPixels = 450;

    private int size = 3;
    private int[,] board;
    private Button[,] tiles;
    private int blankR, blankC;
    private int moves;
    private int seconds;
    private bool started;

    private readonly Panel boardPanel;
    private readonly Label lblInfo;
    private readonly ComboBox cboSize;
    private readonly System.Windows.Forms.Timer timer;
    private readonly Random rng = new Random();

    public SlidingPuzzleForm()
    {
        Text = "Xếp hình số";
        ClientSize = new Size(480, 580);
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
        cboSize.SelectedIndex = 0;

        var btnNew = new Button
        {
            Text = "Ván mới",
            Location = new Point(210, 15),
            Size = new Size(110, 32),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(52, 120, 220),
            ForeColor = Color.White
        };
        btnNew.FlatAppearance.BorderSize = 0;
        btnNew.Click += (s, e) => NewGame();

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

        timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += (s, e) => { seconds++; UpdateInfo(); };

        Controls.AddRange(new Control[] { lblSize, cboSize, btnNew, lblInfo, boardPanel });

        FormClosed += (s, e) => timer.Dispose();
        Shown += (s, e) => NewGame();
    }

    private void NewGame()
    {
        timer.Stop();
        started = false;
        moves = 0;
        seconds = 0;
        size = cboSize.SelectedIndex + 3;

        board = new int[size, size];
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                board[r, c] = r * size + c + 1;
        board[size - 1, size - 1] = 0;
        blankR = size - 1;
        blankC = size - 1;

        Shuffle();
        BuildTiles();
        RefreshTiles();
        UpdateInfo();
        ActiveControl = null;
    }

    // Xáo bằng các nước đi hợp lệ của ô trống -> luôn có lời giải.
    private void Shuffle()
    {
        int[] dr = { -1, 1, 0, 0 };
        int[] dc = { 0, 0, -1, 1 };
        do
        {
            int prevR = -1, prevC = -1;
            int steps = 150 * size;
            for (int i = 0; i < steps; i++)
            {
                int d = rng.Next(4);
                int nr = blankR + dr[d];
                int nc = blankC + dc[d];
                if (nr < 0 || nc < 0 || nr >= size || nc >= size) continue;
                if (nr == prevR && nc == prevC) continue; // không đi ngược lại ngay
                board[blankR, blankC] = board[nr, nc];
                board[nr, nc] = 0;
                prevR = blankR;
                prevC = blankC;
                blankR = nr;
                blankC = nc;
            }
        } while (IsSolved());
    }

    private void BuildTiles()
    {
        boardPanel.Controls.Clear();
        tiles = new Button[size, size];
        int cell = BoardPixels / size;
        float fontSize = size == 3 ? 28f : size == 4 ? 22f : 18f;

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
                    Font = new Font("Segoe UI", fontSize, FontStyle.Bold),
                    ForeColor = Color.White,
                    TabStop = false
                };
                b.FlatAppearance.BorderSize = 0;
                b.Click += (s, e) => OnTileClick(rr, cc);
                tiles[r, c] = b;
                boardPanel.Controls.Add(b);
            }
        }
    }

    private void RefreshTiles()
    {
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                var b = tiles[r, c];
                int v = board[r, c];
                if (v == 0)
                {
                    b.Visible = false;
                    continue;
                }
                b.Visible = true;
                b.Text = v.ToString();
                bool inPlace = v == r * size + c + 1;
                b.BackColor = inPlace ? Color.FromArgb(46, 160, 110) : Color.FromArgb(52, 120, 220);
            }
        }
    }

    private void OnTileClick(int r, int c)
    {
        if (Math.Abs(r - blankR) + Math.Abs(c - blankC) != 1) return;
        MoveBlankTo(r, c);
    }

    private void MoveBlankTo(int nr, int nc)
    {
        if (!started)
        {
            started = true;
            timer.Start();
        }

        board[blankR, blankC] = board[nr, nc];
        board[nr, nc] = 0;
        blankR = nr;
        blankC = nc;
        moves++;

        RefreshTiles();
        UpdateInfo();

        if (IsSolved())
        {
            timer.Stop();
            MessageBox.Show(
                $"Chúc mừng! Bạn đã hoàn thành sau {moves} bước, thời gian {FormatTime(seconds)}.",
                "Thắng rồi!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private bool IsSolved()
    {
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                int expected = (r == size - 1 && c == size - 1) ? 0 : r * size + c + 1;
                if (board[r, c] != expected) return false;
            }
        }
        return true;
    }

    private void UpdateInfo()
    {
        lblInfo.Text = $"Số bước: {moves}      Thời gian: {FormatTime(seconds)}";
    }

    private static string FormatTime(int s) => $"{s / 60:00}:{s % 60:00}";

    // Phím mũi tên: ô trống đi theo hướng ngược lại với ô được đẩy.
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (board != null && !IsSolved())
        {
            int dr = 0, dc = 0;
            switch (keyData)
            {
                case Keys.Up: dr = 1; break;    // đẩy ô phía dưới ô trống lên
                case Keys.Down: dr = -1; break;
                case Keys.Left: dc = 1; break;
                case Keys.Right: dc = -1; break;
                default: return base.ProcessCmdKey(ref msg, keyData);
            }
            int nr = blankR + dr;
            int nc = blankC + dc;
            if (nr >= 0 && nc >= 0 && nr < size && nc < size)
                MoveBlankTo(nr, nc);
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
