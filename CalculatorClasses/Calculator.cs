using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorClasses
{
    public class Calculator
    {
        static public double Add(double a, double b)
        {
            return a + b;
        }

        static public double Sub(double a, double b)
        {
            return a - b;
        }

        static public double Mul(double a, double b)
        {
            return a * b;
        }

        static public double Div(double a, double b)
        {
            if (b == 0) throw new DivideByZeroException("Divide by zero is not possible.");
            return a / b;
        }


        static public double Add(double[] values)
        {
            return values.Sum();
        }

        static public double Mul(double[] values)
        {
            return values.Aggregate(1.0, (agg, value) => agg * value);
        }

        static public double Sub(double[] values)
        {
            double startValue = values.Length > 0 ? values[0] : 0.0;
            double[] toSubtract = RemoveFirst(values);
            return toSubtract.Aggregate(startValue, (agg, value) => agg - value);
        }

        static public double Div(double[] values)
        {
            double startValue = values.Length > 0 ? values[0] : 1.0;
            double[] divisors = RemoveFirst(values);
            return divisors.Aggregate(startValue, (agg, value) => Div(agg, value));
        }

        static private double [] RemoveFirst(double[] items)
        {
            return items.Skip(1).ToArray();
        }
    }
}
