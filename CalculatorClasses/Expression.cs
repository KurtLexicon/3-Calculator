using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorClasses
{
    public class Expression
    {
        public class ParseException : Exception { 
            public ParseException(string msg) : base(msg) { } 
        }

        double baseValue = 0;
        string? op = null;
        List<Expression> expressions = new();

        readonly static char[] termOperators = "+-".ToCharArray();
        readonly static char[] factOperators = "/*".ToCharArray();

        public static Expression ParseExpression(string str)
        {
            string[] terms = str.Split(termOperators);
            List<char> ops = str.Where(c => termOperators.Any(op => c == op)).ToList();
            if (ops.Count() + 1 != terms.Length) throw new ParseException("Unrecognized expression");

            // An empty term means the following operator is in fact a unary operator,
            // So add the operator string to the next term

            foreach(string term in terms)
            {
                if(string.IsNullOrWhiteSpace(term))
                {

                }
            }



            return null;
        }

        private Expression ParseTerm(string str)
        {
            string[] terms = str.Split(factOperators);
            Expression e = new Expression(1.0);
            throw new NotImplementedException();
        }

        public Expression(double value)
        {
            throw new NotImplementedException();
        }

        public Expression(string op, List<Expression> expressions)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public double Calculate()
        {
            throw new NotImplementedException();
        }

        private void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
