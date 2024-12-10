using System.Text;

class Program {
    private const int ProgressBarMargin = 7;
    static string GetProgressBar(int percentage) {
        StringBuilder processBar = new StringBuilder();
        int percentageCount = (int)((percentage / 100.0f) * (Console.WindowWidth - ProgressBarMargin));
        int remainCount = Console.WindowWidth - percentageCount - ProgressBarMargin;
        processBar.Append('\r');
        processBar.Append(string.Concat(Enumerable.Repeat('=', percentageCount)));
        processBar.Append('>');
        processBar.Append(string.Concat(Enumerable.Repeat('-', remainCount)));
        processBar.Append($"[{percentage,3}%]");
        return processBar.ToString();
    }

    static async Task Main(string[] args) {
        for (int i = 0; i <= 100; i++) {
            Console.Write(GetProgressBar(i));
            await Task.Delay(20);
        }
        Console.WriteLine("\nDone!");
    }
}