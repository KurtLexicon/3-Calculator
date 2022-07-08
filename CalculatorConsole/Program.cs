// See https://aka.ms/new-console-template for more information
using CalculatorClasses;

string welcomeString =
@"==============================================================
Give an expression to calculate.
The expression can contain any of these operators: + - * /
Example: 4 - 6 + 3.2 * 1,5 / 2
Parantheses are not supported.
Exit the program by giving an empty input (just press ENTER).
==============================================================";

while (true)
{
    Console.WriteLine(welcomeString);
    string strExpression = Console.ReadLine() ?? "";
    if (string.IsNullOrEmpty(strExpression)) break;

    try
    {
        Expression expression = Expression.ParseExpression(strExpression);
        double result = expression.Calculate();
        Console.WriteLine($"{expression} = {result}");
    }
    catch (ParseException ex) { Console.WriteLine(ex.Message); }
    catch (DivideByZeroException ex) { Console.WriteLine(ex.Message); }
    catch (Exception ex)
    {
        string msg = $"An unexpected error occurred:\n{ex}";
        Console.WriteLine(msg);
    }

    Console.WriteLine();
}
