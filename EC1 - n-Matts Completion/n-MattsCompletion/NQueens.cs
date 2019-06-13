using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace n_MattsCompletion
{
	/// <summary>
	///		NQueens class takes two integer inputs that define the puzzle parameters. 
	///		First, it will initialize the 2D array puzzle with <code>'*'</code> characters in every cell except for the provided puzzle lines.
	///		Next, it will copy the provided puzzle lines in the missing lines in the array and start to solve the puzzle.
	///		It will iterate through every cell and place a <code>'M'</code> character into a cell that
	///		doesn't conflict with any other <code>'M' | 'E'</code> characters either vertically, horizontally, or diagonally.
	///		When the last cell has been iterated through, if every line contains a <code>'M' | 'E'</code> character without a conflict,
	///		then the unprovided lines will be printed out to the screen. 
	///		If not, then the message <code>"FUTILE!"</code> will be printed to the console window.
	/// </summary>
	#region NQueens CLASS
	class NQueens
	{
		#region CLASS UTILITIES
		// PROPERTIES
		bool _solved { get; set; }
		int _boardSize { get; set; }
		int _countOfLinesProvided { get; set; }
		char[,] _board { get; set; }
		int _countOfLinesNeeded { get; set; }

		// INDEXER
		public char this[int r, int c]
		{
			get => _board[r, c];
			set => _board[r, c] = value;
		}

		// CONSTRUCTOR
		public NQueens(int boardSize, int countOfLinesProvided)
		{
			_solved = true;
			_boardSize = boardSize;
			_countOfLinesProvided = countOfLinesProvided;
			_countOfLinesNeeded = boardSize - countOfLinesProvided;
			_board = new char[boardSize, boardSize];
			Initialize();
		}
		#endregion


		#region PUBLIC METHODS
		internal void Evaluate()
		{
			if (_board != null)
			{
				if (Assign('M', _countOfLinesProvided, 0))
					Console.WriteLine(Print());
				else
					Console.WriteLine("FUTILE!\n");
			}
			// TODO: throw error here?
		}
		#endregion

		#region PRIVATE METHODS
		protected bool Assign(char cellValue, int r, int c)
		{
			var target = Find(r, c).Select((int[] arr) => new { r = arr[0], c = arr[1] }); r = target.r; c = target.c;

			while (!_solved && c < _boardSize)
			{
				// Duplicate value found in row|column|diagonal, not safe to place, move on.
				if (Array.Exists(Array<char>.GetRow(_board, r), e => e.In('M', 'E')) ||
					Array.Exists(Array<char>.GetColumn(_board, c), e => e.In('M', 'E')) ||
					Array.Exists(Array<char>.GetDiagonals(_board, r, c, _boardSize), e => e.In('M', 'E')))
				{
					target = Find(r, ++c).Select((int[] arr) => new { r = arr[0], c = arr[1] }); r = target.r; c = target.c;
				}
				// Unique value found, safe to place here.
				else
				{
					_board[r, c] = 'M';
					_solved = Assign('M', r, c);

					// IF: completed puzzle not valid, reset current cell and try next cell
					if (!_solved)
						_board[r, c++] = '*';
				}
			}

			// IF: reached end of puzzle, ensure every line and column has a 'E'|'M'
			if (r == _boardSize && _solved)
				Validate();

			return _solved;
		}

		[DebuggerStepThrough]
		int[] Find(int r, int c)
		{
			_solved = true;
			while (r < _boardSize)
			{
				while (c < _boardSize)
				{
					if (_board[r, c] == '*')
					{
						_solved = false;
						break;
					}
					c++;
				}
				if (!_solved)
					break;
				c = 0;
				r++;
			}
			return new int[] {r, c};
		}

		internal string Print()
		{
			string s = "";
			for (int r = _countOfLinesProvided; r < _boardSize; r++)
			{
				for (int c = 0; c < _boardSize; c++)
					s += _board[r, c] + " ";
				s += "\n";
			}
			return s;
		}

		private void Initialize()
		{
			for (int r = _countOfLinesProvided; r < _boardSize; r++)
				for (int c = 0; c < _boardSize; c++)
					_board[r, c] = '*';
		}

		private bool Validate()
		{
			bool isValid = true;
			for (int r = 0; r < _boardSize; r++)
			{
				if (Array.Exists(Array<char>.GetRow(_board, r), e => e.In('M', 'E')))
				{}
				else
				{
					_solved = false;
					isValid = false;
					break;
				}	
			}
			return isValid;
		}
	}
	#endregion
	#endregion


	#region EXTENSION CLASSES
	/// <summary>
	///		Helper static utility class to pull data out of the array by row, column, diagonals, or subsections of a 2D array.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <see cref="https://stackoverflow.com/questions/27427527/how-to-get-a-complete-row-or-column-from-2d-array-in-c-sharp"/>
	[DebuggerStepThrough]
	internal static class Array<T>
	{
		public static T[] GetRow(T[,] matrix, int lockRow)
		{
			return Enumerable.Range(0, matrix.GetLength(1))
				.Select(c => matrix[lockRow, c])
				.ToArray();
		}

		public static T[] GetColumn(T[,] matrix, int lockColumn)
		{
			return Enumerable.Range(0, matrix.GetLength(0))
					.Select(r => matrix[r, lockColumn])
					.ToArray();
		}

		public static T[] GetSubsection(T[,] matrix, int lockRow, int rowWidth, int lockColumn, int colHeight)
		{
			var temp = new List<T>();
			for (int r = lockRow - (lockRow % rowWidth); r < rowWidth; r++)
				for (int c = lockColumn - (lockColumn % colHeight); c < colHeight; c++)
					temp.Add(matrix[r, c]);
			return temp.ToArray();
		}

		public static T[] GetDiagonals(T[,] matrix, int r, int c, int boardSize)
		{
			var list = new List<T>();
			bool TL = false, TR = false, BR = false, BL = false;
			for (int n = 1; !TL || !TR || !BR || !BL; n++)
			{
				if (!TL) TL = Collect(matrix, list, r, c, r-n, c-n, boardSize); // top-left
				if (!TR) TR = Collect(matrix, list, r, c, r-n, c+n, boardSize); // top-right
				if (!BR) BR = Collect(matrix, list, r, c, r+n, c+n, boardSize); // bottom-right
				if (!BL) BL = Collect(matrix, list, r, c, r+n, c-n, boardSize); // bottom-left
			}
			return list.ToArray();

			// Internal helper method
			bool Collect(T[,] m, List<T> l, int lockRow, int lockColumn, int y, int x, int size)
			{
				if (x < 0 || x >= size || y < 0 || y >= size)
					return true;

				if (Math.Abs(lockRow - y) == Math.Abs(lockColumn - x))
					l.Add(m[y, x]);
				return false;
			}
		}
	}

	[DebuggerStepThrough]
	internal static class LinqExtension
	{
		/// <summary>
		///		Helper static utility class written for me by user "IT WeiHan" on stackoverflow.com.
		///		I asked for help on how to assign 2 variables within a Linq query. And the user gave me this verbatim.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ints">Simple temp int array of length 2 only. Holding the found row and column.</param>
		/// <param name="func">Simple explicit function to place both indexes of the temp array into two variables.</param>
		/// <returns>Returns int array containing the found row and column of the element in the 2D array.</returns>
		/// <see cref="https://stackoverflow.com/questions/56301662/linq-how-to-assign-local-variables-within-linq-from-array"/>
		public static T Select<T>(this int[] ints, Func<int[], T> func)
		{
			return func(ints);
		}

		/// <summary>
		///		Simple extension method that allows me to see if a data collection contains any of the inputted values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="values"></param>
		/// <returns>True: if at least one of the values is contained in data. False: if no values are contained in data.</returns>
		/// <see cref="https://stackoverflow.com/questions/3907299/if-statements-matching-multiple-values"/>
		public static bool In<T>(this T t, params T[] values)
		{
			return values.Contains(t);
		}
	}
	#endregion
}
