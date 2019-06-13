using System;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace n_MattsCompletion
{
	class Program
	{
		public static void Main(string[] args)
		{
			Input();
		}

		static void Input()
		{
			int r = 0;
			string lineRead;
			NQueens QUEENS = null;
			bool processing = false;

			// Accepting single and multi input puzzles.
				// Single: starts with "[n] [m]", followed by m-times provided puzzle lines, marking end of input with single "[0]".
				// Multi: starts with "[n] [m]", followed by m-times provided puzzle lines, followed by another set of "[n] [m]", repeating until an input of "[0]" read. 
			while ((lineRead = Console.ReadLine()).Trim() != null && lineRead != "0")
			{
				if (Regex.IsMatch(lineRead, @"^[0-9]+ [0-9]+$"))
				{
					if (processing)
					{
						r = 0;
						processing = false;
						QUEENS.Evaluate();
					}
					processing = true;
					string[] firstLine = lineRead.Split(" ", StringSplitOptions.RemoveEmptyEntries);
					QUEENS = new NQueens(int.Parse(firstLine[0]), int.Parse(firstLine[1]));
				}
				else
				{
					string[] givenLines = lineRead.Split(" ", StringSplitOptions.RemoveEmptyEntries);
					for (int c = 0; c < givenLines.Length; c++)
						QUEENS[r, c] = char.Parse(givenLines[c]);
					r++;
				}
			}
			r = 0;
			processing = false;
			QUEENS.Evaluate();

			if (Console.ReadLine() != null)
				Input();
			else
				Environment.Exit(1);
		}
	}
}
