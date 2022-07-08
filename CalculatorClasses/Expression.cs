using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CalculatorClasses
{
    /// <summary>
    /// An expression can be
    /// Either:
    /// - A single numerical value
    /// Or:
    /// - An operator together with a list of sub-expressions
    /// </summary>
    public class Expression
    {
        public class ParseException : Exception
        {
            public ParseException(string msg) : base(msg) { }
            public ParseException() : base("The expression is not in valid format, please try again.") { }
        }

        internal const string opAdd = "+";
        internal const string opSub = "-";
        internal const string opMul = "*";
        internal const string opDiv = "/";

        private readonly double baseValue = 0;
        private readonly string? op = null;
        private readonly List<Expression> expressions = new();

        internal Expression(string? op, List<Expression> expressions)
        {
            this.op = op;
            this.expressions = expressions;
        }

        internal Expression(double? value)
        {
            if (value != null)
                this.baseValue = value.Value;
        }

        public static Expression Parse(string str)
        {
            return ExpressionParser.ParseExpression(str);
        }

        public override string ToString()
        {
            return op == null
                ? baseValue.ToString()
                : String.Join($" {op} ", expressions.Select(e => e.ToString()));
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
