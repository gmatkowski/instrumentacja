using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace instrumentacja
{
    class Program
    {
        static int[] TestVector;
        static ulong OpComparisonEQ;
        const int NIter = 10;
        const int maxTries = 10;
        const int tableWidth = 150;

        const int step_max = 26843545;
        const int mmultiplier_max = 10;

        const int step_avg = 50000;
        const int multiplier_avg = 10;


        static void Main(string[] args)
        {

            Console.SetWindowSize(160, 40);

            Console.WriteLine("Max:");
            PrintRow("Size", "tLMaxI", "tLMaxT", "tBMaxI", "tBMaxT");
            PrintLine();
            for (int ArraySize = step_max; ArraySize <= step_max * mmultiplier_max; ArraySize += step_max)
            {
                TestVector = new int[ArraySize];
                for (int i = 0; i < TestVector.Length; ++i)
                {
                    TestVector[i] = i;
                }

                long[] linearTim = LinearTim();
                long[] binaryTim = BinaryTim();

                string linearMaxInstr = LinearMaxInstr();
                string linearMaxTim = showTime(getTime(linearTim, "max") * 1000, "F4");

                string binaryMaxInstr = BinaryMaxInstr();
                string binaryMaxTim = showTime(getTime(binaryTim, "max") * 1000000, "F4", "μs");

                PrintRow(ArraySize.ToString(), linearMaxInstr, linearMaxTim, binaryMaxInstr, binaryMaxTim);
            }
            PrintLine();

            Console.WriteLine("Average:");
            PrintRow("Size", "tLAvgI", "tLAvgT", "tBAvgI", "tBAvgT");
            PrintLine();
            for (int ArraySize = step_avg; ArraySize <= step_avg * multiplier_avg; ArraySize += step_avg)
            {
                TestVector = new int[ArraySize];
                for (int i = 0; i < TestVector.Length; ++i) { 
                    TestVector[i] = i;
                }

                long[] linearTim = LinearTim();
                long[] binaryTim = BinaryTim();

                string linearAvgInstr = LinearAvgInstr();
                string linearAvgTim = showTime(getTime(linearTim, "average") * 1000, "F4");

                string binaryAvgInstr = BinaryAvgInstr();
                string binaryAvgTim = showTime(getTime(binaryTim, "average") * 1000000, "F4", "μs");

                PrintRow(ArraySize.ToString(), linearAvgInstr, linearAvgTim, binaryAvgInstr, binaryAvgTim);
            }
      
            PrintLine();

            Console.WriteLine("Done");

            Console.ReadKey();
        }

        //Completed
        static bool IsPresent_BinaryTim(int[] Vector, int Number)
        {
            int Left = 0, Right = Vector.Length - 1, Middle;
            while (Left <= Right)
            {
                Middle = (Left + Right) / 2;
                if (Vector[Middle] == Number) return true;
                else if (Vector[Middle] > Number) Right = Middle - 1;
                else Left = Middle + 1;
            }
            return false;
        }

        static string BinaryAvgInstr()
        {
            OpComparisonEQ = 0;
            for (int i = 0; i < TestVector.Length; ++i)
            {
                IsPresent_BinaryInstr(TestVector, i);
            }

            return ((double)OpComparisonEQ / (double)TestVector.Length).ToString("F1");
        }

        static string BinaryMaxInstr()
        {
            OpComparisonEQ = 0;
            bool Present = IsPresent_BinaryInstr(TestVector, TestVector.Length);

            return OpComparisonEQ.ToString();
        }

        static bool IsPresent_BinaryInstr(int[] Vector, int Number)
        {
            int Left = 0, Right = Vector.Length - 1, Middle;
            while (Left <= Right)
            {
                OpComparisonEQ++;
                Middle = (Left + Right) / 2;
                if (Vector[Middle] == Number) return true;
                else
                {
                    if (Vector[Middle] > Number) Right = Middle - 1;
                    else Left = Middle + 1;
                }
            }
            return false;
        }

        static long[] BinaryTim()
        {
            long IterationElapsedTime;
            long[] results = new long[maxTries];
            long StartingTime;
            long EndingTime;
            int length = TestVector.Length - 1;

            for (int n = 0; n < (maxTries); ++n)
            {
                StartingTime = Stopwatch.GetTimestamp();
                IsPresent_BinaryTim(TestVector, length);
                EndingTime = Stopwatch.GetTimestamp();

                IterationElapsedTime = EndingTime - StartingTime;

                results[n] = IterationElapsedTime;
            }

            return results;
        }


        static string LinearMaxInstr()
        {
            OpComparisonEQ = 0;
            IsPresent_LinearInstr(TestVector, TestVector.Length - 1);

            return OpComparisonEQ.ToString();
        }


        static bool IsPresent_LinearInstr(int[] Vector, int Number)
        {
            for (int i = 0; i < Vector.Length; i++)
            {
                OpComparisonEQ++;
                if (Vector[i] == Number) { 
                    return true;
                }
            }
            return false;
        }


        static bool IsPresent_LinearTim(int[] Vector, int Number)
        {
            for (int i = 0; i < Vector.Length; i++)
            {
                if (Vector[i] == Number)
                {
                    return true;
                }
            }
            return false;
        }


        static string LinearAvgInstr()
        {
            OpComparisonEQ = 0;
            for (int i = 0; i < TestVector.Length; ++i) {
                IsPresent_LinearInstr(TestVector, i);
            }

            return ((double)OpComparisonEQ / (double)TestVector.Length).ToString("F1");
        }


        static long[] LinearTim()
        {
            long IterationElapsedTime;

            long[] results = new long[maxTries];

            for (int n = 0; n < (maxTries); ++n)
            {
                long StartingTime = Stopwatch.GetTimestamp();
                IsPresent_LinearTim(TestVector, TestVector.Length - 1);
                long EndingTime = Stopwatch.GetTimestamp();

                IterationElapsedTime = EndingTime - StartingTime;
                results[n] = IterationElapsedTime;
            }

            return results;
        }

        static double getTime(long[] results, string mode)
        {
            switch (mode)
            {
                case "min":
                    return results.Min();
                case "max":
                    return results.Max();
                case "average":
                    return results.Average();
                default:
                    return 0;
            }
        }

        static double parseTime(double time)
        {
            return time * (1.0 / (NIter * Stopwatch.Frequency));
        }

        static string showTime(double time, string precision = "F4", string unit = "ms")
        {
            return parseTime(time).ToString(precision) + " " + unit;
        }

        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

    }
}
