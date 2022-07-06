using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorClasses
{
    public class ParseException : Exception
    {
        public ParseException(string msg) : base(msg) { }
        public ParseException() : base("The expression is not in valid format, please try again.") { }
    }

    /// <summary>
    /// Creates an instance of the Expression class from a string
    /// </summary>
    internal class ExpressionParser
    {
        const string operators = "+-*/"; // Order of operators is important, this is the order
                                         // in which they will be parsed

        private class ExpressionItem
        {
            public char? op;
            public double? value;
            public ExpressionItem(char? op, double? value)
            {
                this.op = op;
                this.value = value;
            }
        }

        public static Expression ParseExpression(string str)
        {
            List<ExpressionItem> items = ParseExpressionItemList(str);
            HandleUnaryOperators(items);
            Expression expression = CreateExpression(items, operators);
            return expression;
        }

        static private List<ExpressionItem> ParseExpressionItemList(string str)
        {
            List<double?> values = str.Split(operators.ToCharArray())
                .Select(s => ParseNullableDouble(s.Trim()))
                .ToList();

            List<char> ops = str
                .Where(c => operators.Contains(c))
                .ToList();

            if (values.Last() == null)
            {
                // This would happen if str ends with an operator
                throw new ParseException();
            }

            // The very first item does not have any associated operator, so it gets op = null
            List<ExpressionItem> items = new() { new ExpressionItem(null, values[0]) };
            for (int i = 1; i < values.Count; i++)
            {
                items.Add(new ExpressionItem(ops[i - 1], values[i]));
            }
            return items;
        }

        static private void HandleUnaryOperators(List<ExpressionItem> items)
        {
            // We already know that the value of the last item is not empty.
            // An empty value on any other item means the operator on the NEXT item is in fact a unary operator,
            // so set the current value to following items value (negated if necessary),
            // and discard the following item

            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].value == null)
                {
                    items[i].value = (items[i + 1].op) switch
                    {
                        '+' => items[i + 1].value,
                        '-' => -items[i + 1].value,
                        _ => throw new ParseException() // Could happen if '*' or '/' is used as prefix / unary operator
                    };
                    items.RemoveAt(i + 1);
                }
            }
        }

        static private Expression CreateExpression(List<ExpressionItem> items, string ops)
        {
            CheckCreateExpressionParams(items);

            // If there's only one item in the list then create an expression from only this item's value
            if (items.Count == 1) return new Expression(items[0].value);

            // op = the one we're working on now, nextOps = the one's remaining
            char op = ops[0];
            string nextOps = ops[1..];

            // Split the list into chunks where every sublist starts with op = null
            List<List<ExpressionItem>> chunkList = new();
            while (items.Count > 0)
            {
                items[0].op = null;
                chunkList.Add(items.TakeWhile(item => item.op != op).ToList());
                items = items.SkipWhile(item => item.op != op).ToList();
            }

            // Create a sub expression for each sub list
            List<Expression> subExpressions = chunkList.
                Select(chunk => CreateExpression(chunk, nextOps)).
                ToList();

            // If we have only one sub expression return this as it is,
            // otherwise create a new expression containing all the sub expression
            // bound together with the current operator
            return subExpressions.Count switch
            {
                1 => subExpressions[0],
                _ => new Expression(op.ToString(), subExpressions),
            };
        }

        static private void CheckCreateExpressionParams(List<ExpressionItem> items)
        {
            if (items.Count == 0) throw new Exception("CreateExpression: Parameter items is empty");
            if (items[0].op != null) throw new Exception("CreateExpression: First op in lst should be null");
            if (items.Skip(1).Any(item => item.op == null)) throw new Exception("CreateExpression: All other op in items shall NOT be null");
        }

        static private double? ParseNullableDouble(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;

            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string strTrimmed = str.Trim().Replace(".", decimalSeparator);

            if (!double.TryParse(strTrimmed, out double value))
            {
                throw new ParseException();
            }
            return value;
        }
    }
}
