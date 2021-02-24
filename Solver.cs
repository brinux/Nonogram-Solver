using System;
using System.Collections.Generic;

namespace Nonogram_Solver
{
	class Solver
	{
		private const int UNSET = 0;
		private const int FULL = 1;
		private const int EMPTY = -1;

		private int Size { get; }

		private int[][] ColumnCounters { get; }
		private int[][] RowCounters { get; }

		private int[,] Grid;

		private List<int> ColumnsToProcess;
		private List<int> RowsToProcess;

		private bool[] ColumnsCompleted;
		private bool[] RowsCompleted;

		private List<int[]>[] ColumnsOptions;
		private List<int[]>[] RowsOptions;

		public Solver(int[][] columnCounters, int[][] rowCounters)
		{
			if (columnCounters.Length != rowCounters.Length)
			{
				throw new ArgumentException("The number of counters for rows and columns is not the same");
			}

			Size = columnCounters.Length;
			
			Console.WriteLine($"Grid size set to { Size }");

			for (int i = 0; i < Size; i++)
			{
				if (columnCounters[i].Length == 0)
				{
					throw new ArgumentException($"No counters set for column { i }");
				}

				if (rowCounters[i].Length == 0)
				{
					throw new ArgumentException($"No counters set for row { i }");
				}
			}

			ColumnCounters = columnCounters;
			RowCounters = rowCounters;

			Console.WriteLine($"New grid info set. Size set to { Size }");
		}

		private void ResetGrid()
		{
			Grid = new int[Size, Size];
			for (int r = 0; r < Size; r++)
			{
				for (int c = 0; c < Size; c++)
				{
					Grid[r, c] = UNSET;
				}
			}

			Console.WriteLine("Grid set to empty");

			ColumnsToProcess = new List<int>();
			RowsToProcess = new List<int>();

			RowsCompleted = new bool[Size];
			ColumnsCompleted = new bool[Size];

			ColumnsOptions = new List<int[]>[Size];
			RowsOptions = new List<int[]>[Size];

			for (int i = 0; i < Size; i++)
			{
				ColumnsToProcess.Add(i);
				RowsToProcess.Add(i);

				RowsCompleted[i] = false;
				ColumnsCompleted[i] = false;

				ColumnsOptions[i] = new List<int[]>();
				RowsOptions[i] = new List<int[]>();
			}

			Console.WriteLine("All rows and columns set to be processed");
			Console.WriteLine("All rows and columns set to not completed");
			Console.WriteLine("All rows and columns options reset");
			Console.WriteLine("Ready to solve");
		}

		public bool IsSolved()
		{
			var solved = true;

			foreach (int cell in Grid)
			{
				solved &= (cell != UNSET);
			}

			return solved;
		}

		public int[][] Solve()
		{
			ResetGrid();

			Console.WriteLine("Calculationg options");

			for (int i = 0; i < Size; i++)
			{
				CalculatePartialRowOptions(i, 0, ProvisionsArray(EMPTY), 0);
				CalculatePartialColumnOptions(i, 0, ProvisionsArray(EMPTY), 0);
			}

			Console.WriteLine("Options calculated");
			Console.WriteLine("Starting solver");

			while (ColumnsToProcess.Count > 0 || RowsToProcess.Count > 0)
			{
				var next = -1;
				var currentMin = 0;
				var succeded = false;

				if (ColumnsToProcess.Count > 0)
				{
					foreach (int c in ColumnsToProcess)
					{
						if (next == -1 || currentMin > ColumnsOptions[c].Count)
						{
							next = c;
							currentMin = ColumnsOptions[c].Count;
						}
					}

					ColumnsToProcess.Remove(next);
					succeded = ProcessColumn(next);
				}
				else
				{
					foreach (int c in RowsToProcess)
					{
						if (next == -1 || currentMin > RowsOptions[c].Count)
						{
							next = c;
							currentMin = RowsOptions[c].Count;
						}
					}

					RowsToProcess.Remove(next);
					succeded = ProcessRow(next);
				}

				if (succeded)
				{
					PrintGrid();
				}
			}

			return Grid.Clone() as int[][];
		}

		private void CalculatePartialRowOptions(int r, int currentCounter, int[] currentOption, int currentCell)
		{
			int countersLengthToGo = 0;
			for (int current = currentCounter; current < RowCounters[r].Length; current++)
			{
				countersLengthToGo += RowCounters[r][current];
			}
			countersLengthToGo += RowCounters[r].Length - 1 - currentCounter;

			if (currentCell <= Size - countersLengthToGo)
			{
				for (int myStart = currentCell; myStart <= Size - countersLengthToGo; myStart++)
				{
					var option = currentOption.Clone() as int[];
					for (int l = 0; l < RowCounters[r][currentCounter]; l++)
					{
						option[myStart + l] = FULL;
					}

					if (currentCounter == RowCounters[r].Length - 1)
					{
						RowsOptions[r].Add(option);
					}
					else
					{
						CalculatePartialRowOptions(r, currentCounter + 1, option, myStart + RowCounters[r][currentCounter] + 1);
					}
				}
			}
		}

		private void CalculatePartialColumnOptions(int c, int currentCounter, int[] currentOption, int currentCell)
		{
			int countersLengthToGo = 0;
			for (int current = currentCounter; current < ColumnCounters[c].Length; current++)
			{
				countersLengthToGo += ColumnCounters[c][current];
			}
			countersLengthToGo += ColumnCounters[c].Length - 1 - currentCounter;

			if (currentCell <= Size - countersLengthToGo)
			{
				for (int myStart = currentCell; myStart <= Size - countersLengthToGo; myStart++)
				{
					var option = currentOption.Clone() as int[];
					for (int l = 0; l < ColumnCounters[c][currentCounter]; l++)
					{
						option[myStart + l] = FULL;
					}

					if (currentCounter == ColumnCounters[c].Length - 1)
					{
						ColumnsOptions[c].Add(option);
					}
					else
					{
						CalculatePartialColumnOptions(c, currentCounter + 1, option, myStart + ColumnCounters[c][currentCounter] + 1);
					}
				}
			}
		}

		private bool ProcessRow(int r)
		{
			bool succeded = false;

			// Clean options
			var rowOptionsToRemove = new List<int[]>();
			foreach (var option in RowsOptions[r])
			{
				for (int i = 0; i < Size; i++)
				{
					if (Grid[r, i] != UNSET && Grid[r, i] != option[i])
					{
						rowOptionsToRemove.Add(option);

						break;
					}
				}
			}
			RowsOptions[r].RemoveAll(o => rowOptionsToRemove.Contains(o));

			// Detect common elements
			for (int c = 0; c < Size; c++)
			{
				int value = UNSET;

				foreach (var option in RowsOptions[r])
				{
					if (value == UNSET)
					{
						value = option[c];
					}
					else if (value != option[c])
					{
						value = UNSET;

						break;
					}
				}

				if (value != UNSET && Grid[r, c] == UNSET)
				{
					Grid[r, c] = value;

					Console.WriteLine($"Cell [{ r }, { c }] set to { value }");

					if (!ColumnsToProcess.Contains(c))
					{
						ColumnsToProcess.Add(c);
					}

					succeded = true;
				}
			}

			return succeded;
		}

		private bool ProcessColumn(int c)
		{
			bool succeded = false;

			// Clean options
			var columnOptionsToRemove = new List<int[]>();
			foreach (var option in ColumnsOptions[c])
			{
				for (int i = 0; i < Size; i++)
				{
					if (Grid[i, c] != UNSET && Grid[i, c] != option[i])
					{
						columnOptionsToRemove.Add(option);

						break;
					}
				}
			}
			ColumnsOptions[c].RemoveAll(o => columnOptionsToRemove.Contains(o));

			// Detect common elements
			for (int r = 0; r < Size; r++)
			{
				int value = UNSET;

				foreach (var option in ColumnsOptions[c])
				{
					if (value == UNSET)
					{
						value = option[r];
					}
					else if (value != option[r])
					{
						value = UNSET;

						break;
					}
				}

				if (value != UNSET && Grid[r, c] == UNSET)
				{
					Grid[r, c] = value;

					Console.WriteLine($"Cell [{ r }, { c }] set to { value }");

					if (!RowsToProcess.Contains(r))
					{
						RowsToProcess.Add(r);
					}

					succeded = true;
				}
			}

			return succeded;
		}

		private int[] ProvisionsArray(int value)
		{
			int[] array = new int[Size];
			for (int i = 0; i < Size; i++)
			{
				array[i] = value;
			}

			return array;
		}

		private void PrintGrid()
		{
			for (int r = 0; r < Size; r++)
			{
				for (int c = 0; c < Size; c++)
				{
					switch (Grid[r, c])
					{
						case UNSET:
							Console.Write("░ ");
							break;
						case FULL:
							Console.Write("█ ");
							break;
						case EMPTY:
							Console.Write("X ");
							break;
					}
				}

				Console.WriteLine();
			}

			Console.ReadLine();
		}
	}
}
