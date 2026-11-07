using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace PuzzleGame;

/// <summary>Đố vui bằng tiếng Việt: nhập đáp án, có gợi ý và bỏ qua. Gõ không dấu cũng được.</summary>
public class RiddleForm : Form
{
    private class Riddle
    {
        public string Question;
        public string[] Answers;
        public Riddle(string question, params string[] answers)
        {
            Question = question;
            Answers = answers;
        }
    }

    private static readonly string[] Prefixes = { "cai ", "con ", "qua ", "trai ", "chiec ", "loi ", "bi " };

    private readonly List<Riddle> riddles;
    private int index;
    private int score;
    private int correctCount;
    private bool usedHint;

    private readonly Label lblProgress;
    private readonly Label lblQuestion;
    private readonly Label lblFeedback;
    private readonly TextBox txtAnswer;
    private readonly Button btnSubmit;
    private readonly Button btnHint;
    private readonly Button btnSkip;

    public RiddleForm()
    {
        Text = "Câu đố Việt Nam";
        ClientSize = new Size(560, 420);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(30, 34, 44);
        Font = new Font("Segoe UI", 11f);

        var all = new List<Riddle>
        {
            new Riddle("Cái gì càng lau càng ướt?", "khăn", "khăn lau", "khăn tắm"),
            new Riddle("Cái gì của bạn nhưng người khác lại dùng nhiều hơn bạn?", "tên", "họ tên"),
            new Riddle("Cái gì tăng lên mỗi năm nhưng không bao giờ giảm?", "tuổi", "tuổi tác", "số tuổi"),
            new Riddle("Cái gì đập thì sống, không đập thì chết?", "tim", "trái tim", "quả tim"),
            new Riddle("Cái gì luôn ở phía trước bạn nhưng bạn không thể nhìn thấy?", "tương lai"),
            new Riddle("Con gì buổi sáng đi bốn chân, buổi trưa đi hai chân, buổi tối đi ba chân?", "con người", "người"),
            new Riddle("Cái gì có nhiều răng nhưng không bao giờ ăn?", "lược", "cưa", "bánh răng"),
            new Riddle("Thứ gì có thể bị phá vỡ mà không cần chạm vào?", "lời hứa", "hứa"),
            new Riddle("Cái gì bạn có thể bắt được nhưng không thể ném đi?", "cảm lạnh", "cảm", "cảm cúm"),
            new Riddle("Cái gì có thể đi vòng quanh thế giới mà vẫn nằm yên ở một góc?", "tem", "con tem", "cái tem"),
        };

        var rng = new Random();
        riddles = all.OrderBy(_ => rng.Next()).ToList();

        lblProgress = new Label
        {
            ForeColor = Color.FromArgb(170, 180, 200),
            Location = new Point(20, 15),
            Size = new Size(520, 26)
        };

        lblQuestion = new Label
        {
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 16f, FontStyle.Bold),
            Location = new Point(20, 55),
            Size = new Size(520, 120),
            TextAlign = ContentAlignment.MiddleCenter
        };

        txtAnswer = new TextBox
        {
            Location = new Point(60, 195),
            Size = new Size(440, 32),
            Font = new Font("Segoe UI", 14f)
        };

        btnSubmit = MakeButton("Trả lời", 60, 245, 140, Color.FromArgb(46, 160, 110));
        btnHint = MakeButton("Gợi ý", 215, 245, 130, Color.FromArgb(220, 150, 40));
        btnSkip = MakeButton("Bỏ qua", 360, 245, 140, Color.FromArgb(120, 125, 140));

        lblFeedback = new Label
        {
            ForeColor = Color.FromArgb(255, 210, 60),
            Location = new Point(20, 305),
            Size = new Size(520, 70),
            TextAlign = ContentAlignment.MiddleCenter
        };

        btnSubmit.Click += (s, e) => Submit();
        btnHint.Click += (s, e) => ShowHint();
        btnSkip.Click += (s, e) => Skip();
        AcceptButton = btnSubmit;

        Controls.AddRange(new Control[] { lblProgress, lblQuestion, txtAnswer, btnSubmit, btnHint, btnSkip, lblFeedback });

        ShowQuestion();
    }

    private static Button MakeButton(string text, int left, int top, int width, Color color)
    {
        var b = new Button
        {
            Text = text,
            Location = new Point(left, top),
            Size = new Size(width, 40),
            FlatStyle = FlatStyle.Flat,
            BackColor = color,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11f, FontStyle.Bold)
        };
        b.FlatAppearance.BorderSize = 0;
        return b;
    }

    private void ShowQuestion()
    {
        usedHint = false;
        var r = riddles[index];
        lblProgress.Text = $"Câu {index + 1}/{riddles.Count}      Điểm: {score}";
        lblQuestion.Text = r.Question;
        lblFeedback.Text = "";
        txtAnswer.Text = "";
        txtAnswer.Focus();
    }

    private void Submit()
    {
        string input = Normalize(txtAnswer.Text);
        if (input.Length == 0)
        {
            lblFeedback.Text = "Hãy nhập đáp án nhé!";
            return;
        }

        var r = riddles[index];
        bool ok = r.Answers.Any(a => Normalize(a) == input);

        if (ok)
        {
            int gained = usedHint ? 5 : 10;
            score += gained;
            correctCount++;
            MessageBox.Show($"Chính xác! +{gained} điểm", "Đúng rồi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Next();
        }
        else
        {
            lblFeedback.Text = "Chưa đúng, thử lại nhé!";
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }
    }

    private void ShowHint()
    {
        var answer = riddles[index].Answers[0];
        int letters = answer.Count(ch => !char.IsWhiteSpace(ch));
        int words = answer.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        usedHint = true;
        lblFeedback.Text = $"Gợi ý: đáp án có {words} từ, {letters} chữ cái, bắt đầu bằng \"{answer[0]}\". (Trả lời đúng được 5 điểm)";
        txtAnswer.Focus();
    }

    private void Skip()
    {
        var answer = riddles[index].Answers[0];
        MessageBox.Show($"Đáp án là: {answer}", "Bỏ qua", MessageBoxButtons.OK, MessageBoxIcon.Information);
        Next();
    }

    private void Next()
    {
        index++;
        if (index >= riddles.Count)
        {
            MessageBox.Show(
                $"Hoàn thành! Bạn trả lời đúng {correctCount}/{riddles.Count} câu, tổng điểm {score}/{riddles.Count * 10}.",
                "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
            return;
        }
        ShowQuestion();
    }

    // Chuẩn hóa: bỏ dấu, chữ thường, bỏ các từ đứng đầu như "cái", "con"...
    private static string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";

        string s = text.Trim().ToLowerInvariant().Replace('đ', 'd');
        string decomposed = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (char ch in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        s = sb.ToString().Normalize(NormalizationForm.FormC);
        s = string.Join(" ", s.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        foreach (var p in Prefixes)
        {
            if (s.StartsWith(p) && s.Length > p.Length)
            {
                s = s.Substring(p.Length);
                break;
            }
        }
        return s;
    }
}
