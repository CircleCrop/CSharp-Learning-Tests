// Snake Game

using System.Text;

internal class SnakeGame {
    public static class Symbols {
        // Emoji
        public const string Wall = "🧱";
        public const string Head = "🐍";
        public const string Body = "🟩"; //绿色方块
        public const string Apple = "🍎";
        public const string Blank = "　"; //全角空格
        public const string heart = "❤️";
        public const string blackHeart = "🖤";
    }

    public const int snakeFullHealth = 1;
    public static int snakeHealth = snakeFullHealth;
    public static int snakeStartLength = 10;
    private static int _snakeSpeed = 500; //ms
    public static int snakeSpeed {
        get {
            return _snakeSpeed;
        }
        set {
            if (snakeSpeed >= 60) {
                _snakeSpeed = value;
            }
        }
    }

    public const int windowWidth = 20;
    public const int windowHeight = 16;
    public static int time = 0;
    public static int score = 0;
    public static char[,] map = new char[windowHeight, windowWidth]; // 0=blank, 1=snakebody, 2=snakehead, 3=apple, 4=wall
    public static string? message;
    public static Boolean gameOver = false;
    public static string direction = "right";
    public static Timer? timer;
    public static List<(int x, int y)> snakeLocation = new List<(int x, int y)>();
    public static (int x, int y) appleLocation;

    public static void RunSnakeGame(string[] args) {
        InitializingGameWindow();

        timer = new Timer(UpdateTime, null, 0, 1000);

        Task ComputeTask = Task.Run(() => {
            while (!gameOver) {
                Thread.Sleep(snakeSpeed); // 控制渲染间隔
                UpdateMap();
                RenderFrame();

            }
        });

        // 监听方向键/WASD，直到 gameOver=True
        while (!gameOver) {
            if (Console.KeyAvailable) {
                var key = Console.ReadKey(intercept: true).Key;
                switch (key) {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        ControlDirection("up");
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        ControlDirection("down");
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        ControlDirection("left");
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        ControlDirection("right");
                        break;
                }
            }
        }
        if (gameOver) {
            message = "游戏结束！";
            RenderFrame();
            Thread.Sleep(100);
            Console.ReadLine();
        }

    }

    public static void UpdateMap() {
        // 控制蛇的方向：每次根据当前方向更新蛇头的位置，并将其插入 List 的开头位置。然后移除 List 的最后一个元素，以模拟前进的效果。
        var lastSnakeHead = snakeLocation[snakeLocation.Count - 1];
        switch (direction) {
            case "up":
                snakeLocation.Add((lastSnakeHead.x - 1, lastSnakeHead.y));
                break;
            case "down":
                snakeLocation.Add((lastSnakeHead.x + 1, lastSnakeHead.y));

                break;
            case "left":
                snakeLocation.Add((lastSnakeHead.x, lastSnakeHead.y - 1));

                break;
            case "right":
                snakeLocation.Add((lastSnakeHead.x, lastSnakeHead.y + 1));
                break;
        }
        var newSnakeHead = snakeLocation[snakeLocation.Count - 1];
        if (newSnakeHead == appleLocation) {
            score += 1;
            snakeSpeed -= 20;
            RandomApple();
        } else {
            snakeLocation.RemoveAt(0); //移除最后一个
        }
        UpdateOnMap();
        /*foreach (var snake in snakeLocation) {
            Console.WriteLine(snake.ToString());
        }*/
        // 位置更新完成，下面开始判定是否撞墙
        var positionSet = new HashSet<(int x, int y)>();
        bool duplicatePosition = false;
        foreach (var position in snakeLocation) {
            // 如果添加失败，说明有重复位置
            if (!positionSet.Add(position)) {
                duplicatePosition = true;
                break;
            }
        }
        if (newSnakeHead.x == -1 || newSnakeHead.x == windowHeight || newSnakeHead.y == -1 || newSnakeHead.y == windowWidth || duplicatePosition) {
            message = "游戏结束！";
            RenderFrame();
            gameOver = true;
            Console.ReadLine();
        } else {
            message = "继续！";
        }
    }


    public static void ControlDirection(string newDirection) {
        if ((newDirection == "up" && direction != "down") ||
            (newDirection == "down" && direction != "up") ||
            (newDirection == "left" && direction != "right") ||
            (newDirection == "right" && direction != "left")) {
            direction = newDirection;
        }

    }

    public static void RenderFrame() {
        StringBuilder Frame = new StringBuilder();
        // 构造生命值区域
        string lifeArea = "生命: " + string.Concat(Enumerable.Repeat(Symbols.heart, snakeHealth))
            + string.Concat(Enumerable.Repeat(Symbols.blackHeart, snakeFullHealth - snakeHealth));
        string timeArea = $"时间: {(time / 60):D2}:{(time % 60):D2}";
        string scoreArea = "得分: " + score.ToString();
        string lengthArea = "长度: " + snakeLocation.Count();
        // 构造信息栏
        Frame.AppendLine($"{lifeArea}　{timeArea}　{scoreArea}　{lengthArea}");
        Frame.AppendLine();
        Frame.AppendLine("💬: " + message);
        Frame.AppendLine();
        // 顶部围墙
        Frame.AppendLine(string.Concat(Enumerable.Repeat(Symbols.Wall, windowWidth + 2)));
        // 构造地图
        for (int i = 0; i < map.GetLength(0); i++) {
            Frame.Append(Symbols.Wall);
            for (int j = 0; j < map.GetLength(1); j++) {
                switch (map[i, j]) { // 0=blank, 1=snakebody, 2=snakehead, 3=apple, 4=wall
                    case (char)0:
                        Frame.Append(Symbols.Blank);
                        break;
                    case (char)1:
                        Frame.Append(Symbols.Body);
                        break;
                    case (char)2:
                        Frame.Append(Symbols.Head);
                        break;
                    case (char)3:
                        Frame.Append(Symbols.Apple);
                        break;
                    case (char)4:
                        Frame.Append(Symbols.Wall);
                        break;
                }

            }
            Frame.AppendLine(Symbols.Wall); // 下一行
        }
        // 底部围墙
        Frame.AppendLine(string.Concat(Enumerable.Repeat(Symbols.Wall, windowWidth + 2)));
        Console.Clear(); // 清屏
        // 渲染到控制台
        Console.WriteLine(Frame.ToString());
    }

    internal static void InitializingGameWindow() {
        // 初始化
        Console.OutputEncoding = Encoding.UTF8;
        message = "游戏开始！";
        for (int i = 0; i < snakeStartLength; i++) { // 初始化蛇的位置
            snakeLocation.Add((0, i));
        }
        RandomApple();
        UpdateOnMap();
        RenderFrame();
        message = "";
    }

    internal static void UpdateOnMap() {
        for (int a = 0; a < map.GetLength(0); a++) {
            for (int b = 0; b < map.GetLength(1); b++) {
                map[a, b] = (char)0;
            }
        }
        foreach (var xy in snakeLocation) {
            map[xy.x, xy.y] = (char)1;
        }
        var head = snakeLocation.Last();
        map[head.x, head.y] = (char)2;
        map[appleLocation.x, appleLocation.y] = (char)3;
    }

    internal static void UpdateTime(object? state) {
        time += 1;
    }
    internal static void RandomApple() {
        Random random = new Random();
        appleLocation = (random.Next(0, windowHeight), random.Next(0, windowWidth));
        foreach (var x in snakeLocation) {
            if (appleLocation == x) {
                RandomApple();
            }
        }
    }
}