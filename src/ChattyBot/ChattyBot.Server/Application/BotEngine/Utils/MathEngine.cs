using System.Data;
using System.Globalization;

namespace ChattyBot.Server.Application.BotEngine.Utils
{
    public static class MathEngine
    {
        public static string Compute(string expression)
        {
            try
            {
                using var dt = new DataTable();
                var result = dt.Compute(expression, "");

                double checkVal = Convert.ToDouble(result);
                if (double.IsInfinity(checkVal) || double.IsNaN(checkVal))
                {
                    return "DIV_ZERO";
                }

                decimal val = Convert.ToDecimal(result);
                decimal roundedVal = Math.Round(val, 4, MidpointRounding.AwayFromZero);
                return roundedVal.ToString("0.####", CultureInfo.InvariantCulture);
            }
            catch (DivideByZeroException)
            {
                return "DIV_ZERO";
            }
            catch (Exception)
            {
                return "MATH_ERR";
            }
        }
    }
}
