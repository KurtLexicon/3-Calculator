using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CalculatorClasses {
    public class ParseException : Exception {
        public ParseException(string msg) : base(msg) { }
        public ParseException() : base("The expression is not in valid format, please try again.") { }
    }

    /// <summary>
    /// Creates an instance of the Expression class from a string
    /// </summary>
    internal partial class ExpressionParser {
        const string operators = "+-*/"; // Order of operators is important, this is the order
                                         // in which they will be parsed

        // https://regexland.com/regex-decimal-numbers/ (slightly adjusted)
        const string numberPattern = @"^(\s*[+-]?(\d+[\,\.]?\d*|[\,\.]\d+)([eE][+-]?\d+)?\s*)";
        const string operatorPattern = @"^([+\-*/])";

        private class ExpressionItem {
            public char? op;
            public double value;
            public ExpressionItem(char? op, double value) {
                this.op = op;
                this.value = value;
            }
        }

        public static Expression ParseExpression(string str) {
            List<ExpressionItem> items = ParseItemList(str);
            return CreateExpression(items, operators);
        }

        static List<ExpressionItem> ParseItemList(string str) {
            List<ExpressionItem> items = new() {
                new ExpressionItem(null, PullValue(ref str))
            };

            while (str.Length > 0) {
                char? op = PullOperator(ref str);
                double value = PullValue(ref str);
                items.Add(new ExpressionItem(op, value));
            }

            return items;
        }

        static char PullOperator(ref string str) {
            string item = PullItem(ref str, operatorPattern);
            return item[0];
        }

        static double PullValue(ref string str) {
            string item = PullItem(ref str, numberPattern);
            return ParseDouble(item);
        }

        static string PullItem(ref string str, string pattern) {
            Match match = Regex.Match(str, pattern);
            if (!match.Success) {
                throw new ParseException();
            }

            string item = match.Groups[1].Value;
            str = str[item.Length..];
            return item.Trim();
        }

        static private Expression CreateExpression(List<ExpressionItem> items, string ops) {
            CheckCreateExpressionParams(items);

            // If there's only one item in the list then create an expression from only this item's value
            if (items.Count == 1) return new Expression(items[0].value);

            // op = the one we're working on now
            char op = ops[0];

            // Split the list into chunks where every sublist starts with op = null
            List<List<ExpressionItem>> chunkList = new();
            while (items.Count > 0) {
                items[0].op = null;
                chunkList.Add(items.TakeWhile(item => item.op != op).ToList());
                items = items.SkipWhile(item => item.op != op).ToList();
            }

            // Create a sub expression for each sub list
            List<Expression> subExpressions = chunkList
                .Select(chunk => CreateExpression(chunk, ops[1..]))
                .ToList();

            // If we have only one sub expression return this as it is,
            // otherwise create a new expression containing all the sub expression
            // bound together with the current operator
            return subExpressions.Count switch {
                1 => subExpressions[0],
                _ => new Expression(op.ToString(), subExpressions),
            };
        }

        static private void CheckCreateExpressionParams(List<ExpressionItem> items) {
            if (items.Count == 0) throw new Exception("CreateExpression: Parameter items is empty");
            if (items[0].op != null) throw new Exception("CreateExpression: First op in lst should be null");
            if (items.Skip(1).Any(item => item.op == null)) throw new Exception("CreateExpression: All other op in items shall NOT be null");
        }

        static private double ParseDouble(string str) {
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string strTrimmed = str.Trim().Replace(".", decimalSeparator);

            if (!double.TryParse(strTrimmed, out double value)) {
                throw new ParseException();
            }
            return value;
        }
    }
}
