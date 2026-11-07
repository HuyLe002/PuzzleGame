# Puzzle Game (C# WinForms, .NET 8)

Bộ 3 game giải đố:

1. **Xếp hình số** (3x3, 4x4, 5x5): bấm ô kề ô trống hoặc dùng phím mũi tên. Có đếm bước và thời gian.
2. **Câu đố Việt Nam**: 10 câu đố ngẫu nhiên, có gợi ý (trả lời đúng còn 5 điểm) và bỏ qua. Gõ không dấu vẫn được.
3. **Lights Out** (3x3, 4x4, 5x5): bấm một ô sẽ đảo ô đó và 4 ô kề, mục tiêu là tắt hết đèn. Có nút Gợi ý và Chơi lại.

## Cách chạy

1. Cài **Visual Studio 2022** (17.8 trở lên) với workload **".NET desktop development"**.
2. Giải nén, mở file `PuzzleGame.sln`.
3. Nhấn **F5** để chạy.

Nếu VS báo thiếu .NET 8 SDK, cài thêm từ https://dotnet.microsoft.com/download/dotnet/8.0

Toàn bộ giao diện được dựng bằng code (không dùng Designer) nên không có file .Designer.cs.
