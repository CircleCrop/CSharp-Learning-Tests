internal class FizzBuzz {
    internal static void RunFizzBuzz() {
        for (int i = 1; i <= 100; i++) {
            string output = i + " - ";
            if (i % 3 == 0) {
                output += "Fizz";
            }
            if (i % 5 == 0) {
                output += "Buzz";
            }
            Console.WriteLine(output);
        }
    }
}