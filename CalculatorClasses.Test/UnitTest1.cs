
namespace CalculatorClassesTest
{
    public class ExpressionTest
    {
        [Theory]
        [InlineData("1+0+")]
        [InlineData("1+*2")]
        [InlineData("1+++2")]
        public void TestParseFailure(string failingExpression)
        {
            // Action
            Expression act() => Expression.Parse(failingExpression);
            // Assert
            ParseException exception = Assert.Throws<ParseException>(act);
            Assert.Equal("The expression is not in valid format, please try again.", exception.Message);
        }

        [Theory]
        [InlineData("1++2", "1 + 2")]
        [InlineData("1+-2", "1 + -2")]
        [InlineData("1-+2", "1 - 2")]
        [InlineData("1--2", "1 - -2")]
        [InlineData("1.2", "1,2")]
        [InlineData(".2", "0,2")]
        [InlineData("1.", "1")]
        [InlineData("1,2", "1,2")]
        [InlineData(",2", "0,2")]
        [InlineData("1,", "1")]
        [InlineData("1+1", "1 + 1")]
        [InlineData(" 1 + 1 ", "1 + 1")]
        [InlineData("1+2.5+2,5", "1 + 2,5 + 2,5")]
        [InlineData("1+.5+2,", "1 + 0,5 + 2")]
        [InlineData("-1/3-4* 7*3/2/-3*8", "-1 / 3 - 4 * 7 * 3 / 2 / -3 * 8")]
        [InlineData("1+2e-2", "1 + 0,02")]
        [InlineData("1+2e+2", "1 + 200")]
        [InlineData("1+2e99", "1 + 2E+99")]
        [InlineData("1+2E-2", "1 + 0,02")]
        [InlineData("1+2E+2", "1 + 200")]
        [InlineData("1+2E99", "1 + 2E+99")]
        public void TestParsexEpression(string str, string expected)
        {
            // Action
            Expression exp = Expression.Parse(str);
            string result = exp.ToString();

            // Assert
            Assert.Equal(expected, result);
        }
    }


    public class CalculatorTest
    {
        [Fact]
        public void DivideByZero()
        {
            // Act
            Action action = () => Calculator.Div(1, 0);

            // Assert
            DivideByZeroException ex = Assert.Throws<DivideByZeroException>(action);
            Assert.Equal("Divide by zero is not possible.", ex.Message);
        }

        [Fact]
        public void DivideByZeroArray()
        {
            //Arrange
            double[] ar = { 2, 0, 2, 1 };
            string expectedText = "Divide by zero is not possible.";

            //Act
            Action action = () => Calculator.Div(ar);
            DivideByZeroException ex = Assert.Throws<DivideByZeroException>(action);

            //Assert
            Assert.Equal(expectedText, ex.Message);
        }

        [Theory]
        [InlineData(2, 3, 5)]
        [InlineData(2, -3, -1)]
        public void TestAdd(double a, double b, double expected)
        {
            double result = Calculator.Add(a, b);
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData(new double[] { 1, 2, 3 }, 6)]
        [InlineData(new double[] { 1, -3, 1 }, -1)]
        public void TestAddArray(double[] ar, double expected)
        {
            double result = Calculator.Add(ar);
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData(9, 6, 3)]
        [InlineData(6, 9, -3)]
        [InlineData(6, -3, 9)]
        public void TestSub(double a, double b, double expected)
        {
            double result = Calculator.Sub(a, b);
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData(new double[] { 10, 1, 2 }, 7)]
        [InlineData(new double[] { -5, -5, -5 }, 5)]
        public void TestSubArray(double[] ar, double expected)
        {
            double result = Calculator.Sub(ar);
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData(2, 3, 6)]
        [InlineData(2, -3, -6)]
        public void TestMul(double a, double b, double expected)
        {
            double result = Calculator.Mul(a, b);
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData(new double[] { 4, 2, 3 }, 24)]
        [InlineData(new double[] { 4, -3, 1 }, -12)]
        public void TestMulArray(double[] ar, double expected)
        {
            double result = Calculator.Mul(ar);
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData(6, 3, 2)]
        [InlineData(2, -4, -0.5)]
        public void TestDiv(double a, double b, double expected)
        {
            double result = Calculator.Div(a, b);
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData(new double[] { 12, 2, 3 }, 2)]
        [InlineData(new double[] { -60, -6, -2 }, -5)]
        public void TestDivArray(double[] ar, double expected)
        {
            double result = Calculator.Div(ar);
            Assert.Equal(result, expected);
        }
    }
}
