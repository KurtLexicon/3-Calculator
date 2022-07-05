using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CalculatorClasses
{
    public class Expression
    {
        public class ParseException : Exception
        {
            public ParseException(string msg) : base(msg) { }
            public ParseException() : base("Unrecognized expression") { }

        }

        readonly double baseValue = 0;
        readonly string? op = null;
        readonly List<Expression> expressions = new();


        const string opAdd = "+";
        const string opSub = "-";
        const string opMul = "*";
        const string opDiv = "/";
        const string operators = "+-*/"; // Order of operators is important, this is the order they will be parsed

        private Expression(string? op, List<Expression> expressions)
        {
            this.op = op;
            this.expressions = expressions;
        }

        private class ExpressionItem
        {
            public char? op;
            public double? value;
            public ExpressionItem SetOpNull() { op = null; return this; }
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
            List<double?> values = str.Split(operators)
                .Select(s => s.Trim())
                .ToList()
                .Select(s => ParseNullableDouble(s))
                .ToList();

            List<char> ops = str
                .Where(c => operators.Any(op => c == op))
                .ToList();

            if (ops.Count + 1 != values.Count)
            {
                // Can't see this happening?
                throw new ParseException();
            }

            if (values.Last() == null)
            {
                // This would happen if str ends with an operator
                throw new ParseException();
            }

            // The very first term does not have any associated operator, so it gets op = null

            List<ExpressionItem> items = new() { new ExpressionItem(null, values[0]) };
            for (int i = 1; i < values.Count; i++)
            {
                items.Add(new ExpressionItem(ops[i - 1], values[i]));
            }

            return items;
        }

        static private void HandleUnaryOperators(List<ExpressionItem> items)
        {
            // We already have checked that the value of the last item is not empty.
            // An empty value on any other item means the operator on the NEXT item is in fact a unary operator,
            // so set the current value to next items value (negated if necessary), and discard the following item

            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].value == null)
                {
                    items[i].value = (items[i + 1].op) switch
                    {
                        '+' => items[i + 1].value,
                        '-' => -items[i + 1].value,
                        _ => throw new ParseException() // Could happen if '*' or '/' is used as prefix
                    };
                    items[i].value = items[i + 1].op + items[i + 1].value;
                    items.RemoveAt(i + 1);
                }
            }
        }

        static private Expression CreateExpression(List<ExpressionItem> items, string ops)
        {
            // Check parameters

            if (items.Count == 0) throw new Exception("CreateExpression: Parameter items is empty");
            if (items[0].op != null) throw new Exception("CreateExpression: First op in lst should be null");
            if (items.Skip(1).Any(item => item.op == null)) throw new Exception("CreateExpression: All other op in items shall NOT be null");

            // If no operators left to handle then just return first value

            if (items.Count == 1) return new Expression(items[0].value);

            // op = the one we're working w now, nextOps = the one's remaining

            char op = ops[0];
            string nextOps = ops[1..];

            // Replace all op operators with null

            items = items.Select(item => (op == item.op) ? item : item.SetOpNull()).ToList();

            // Split the list into chunks where every sublist starts with op = null

            List<List<ExpressionItem>> chunkList = new();
            while (items.Count > 0)
            {
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



        static private double? ParseNullableDouble(string str)
        {
            if (string.IsNullOrWhiteSpace(str.Trim())) return null;

            if (!double.TryParse(str.Trim(), out double value))
            {
                throw new ParseException();
            }
            return value;
        }

        private Expression(double? value)
        {
            if(value != null)
                this.baseValue = value.Value;
        }

        public override string ToString()
        {
            return op == null
                ? baseValue.ToString()
                : String.Join($" {op} ", expressions.Select(e => e.ToString()));
        }

        public string StructuredString()
        {
            return op == null
                ? baseValue.ToString()
                : $"{op}({expressions.Select(e => e.StructuredString())})";
        }

        public double Calculate()
        {
            double[] subValues = this.expressions.Select(e => e.Calculate()).ToArray();
            return op switch
            {
                opAdd => Calculator.Add(subValues),
                opSub => Calculator.Sub(subValues),
                opMul => Calculator.Mul(subValues),
                opDiv => Calculator.Div(subValues),
                _ => baseValue
            };
        }
    }
}
