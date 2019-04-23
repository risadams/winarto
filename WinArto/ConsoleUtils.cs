using System;
using System.Globalization;

namespace WinArto
{
    public static class ConsoleUtils
    {
        /// <summary>
        ///     Writes a single line "caution tape" style highly visible text.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="culture">The culture.</param>
        public static void WriteLineCautionTape(string value, ConsoleColor foreground = ConsoleColor.Black, ConsoleColor background = ConsoleColor.Yellow, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (culture == null)
            {
                culture = CultureInfo.InvariantCulture;
            }

            //cut off text if it will take more than one line.
            if (value.Length > 79)
            {
                value = value.Substring(0, 79);
            }

            //Pad right to fill line with background color
            if (value.Length < 79)
            {
                for (var i = value.Length; i < 79; i++)
                {
                    value = value + " ";
                }
            }

            WriteLineColor("///////////////////////////////////////////////////////////////////////////////", background);
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
            Console.WriteLine(string.Format(culture, value));
            Console.ResetColor();
            WriteLineColor("///////////////////////////////////////////////////////////////////////////////", background);
        }

        /// <summary>
        ///     Writes the line to the console using the specified color.
        ///     Reverts to the default color when complete.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="color">The color.</param>
        public static void WriteLineColor(string value, ConsoleColor color)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ResetColor();
        }
    }
}