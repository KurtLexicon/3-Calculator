


using CalculatorClasses;

namespace CalculatorConsole
{
    internal class Program
    {
        static string welcomeString =
@"==============================================================
Give an expression to calculate.
The expression can contain any of these operators: + - * /
example: 4 - 6 + 3.2 * 1,5 / 2
Parantheses are not supported.
Exit the program by giving an empty input (just press ENTER).
==============================================================";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine(welcomeString);
                string strExpression = Console.ReadLine() ?? "";
                if (string.IsNullOrEmpty(strExpression)) break;
                try
                {
                    Expression expression = Expression.ParseExpression(strExpression);
                    Console.WriteLine(expression.StructuredString());
                    double result = expression.Calculate();
                    Console.WriteLine($"{expression} = {result}");
                }
                // catch (ParseException ex) { Console.WriteLine(ex.Message); }
                catch (DivideByZeroException ex) { Console.WriteLine(ex.Message); }
                catch (Exception ex) {
                    string msg = $"An unexpected error occurred:\n\n{ex.Message}";
                    Console.WriteLine(msg);
                }
                Console.WriteLine();
            }
        }

    }
}