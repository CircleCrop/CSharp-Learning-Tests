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

    public async Task ProcessMultipleWritesAsync() {
        IList<FileStream> sourceStreams = new List<FileStream>();

        try {
            string folder = Directory.CreateDirectory("tempfolder").Name;
            IList<Task> writeTaskList = new List<Task>();

            for (int index = 1; index <= 10; ++index) {
                string fileName = $"file-{index:00}.txt";
                string filePath = $"{folder}/{fileName}";

                string text = $"In file {index}{Environment.NewLine}";
                byte[] encodedText = Encoding.Unicode.GetBytes(text);

                var sourceStream =
                    new FileStream(
                        filePath,
                        FileMode.Create, FileAccess.Write, FileShare.None,
                        bufferSize: 4096, useAsync: true);

                Task writeTask = sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                sourceStreams.Add(sourceStream);

                writeTaskList.Add(writeTask);
            }

            await Task.WhenAll(writeTaskList);
        } finally {
            foreach (FileStream sourceStream in sourceStreams) {
                sourceStream.Close();
            }
        }
    }
}

