using System;
using System.Collections.Generic;

namespace Nonogram_Solver
{
	class NonogramSolver
	{
		public NonogramGrid Grid { get; private set; }

		private NonogramGridPrinter _printer = new NonogramGridPrinter();

		private List<int> _columnsToProcess;
		private List<int> _rowsToProcess;

		private bool[] _columnsCompleted;
		private bool[] _rowsCompleted;

		private List<NonogramCellStatusEnum[]>[] _columnsOptions;
		private List<NonogramCellStatusEnum[]>[] _rowsOptions;

		public NonogramSolver(NonogramGrid grid)
		{
			Grid = grid;
		}

		public bool Solve()
		{
			ResetSolver();

			Console.WriteLine("Calculationg options");

			for (int i = 0; i < Grid.Size; i++)
			{
				CalculatePartialRowOptions(i, 0, ProvisionsArray(NonogramCellStatusEnum.EMPTY), 0);
				CalculatePartialColumnOptions(i, 0, ProvisionsArray(NonogramCellStatusEnum.EMPTY), 0);
			}

			Console.WriteLine("Options calculated");
			Console.WriteLine("Starting solver");

			while (_columnsToProcess.Count > 0 || _rowsToProcess.Count > 0)
			{
				var next = -1;
				var currentMin = 0;
				var succeded = false;

				if (_columnsToProcess.Count > 0)
				{
					foreach (int c in _columnsToProcess)
					{
						if (next == -1 || currentMin > _columnsOptions[c].Count)
						{
							next = c;
							currentMin = _columnsOptions[c].Count;
						}
					}

					_columnsToProcess.Remove(next);
					succeded = ProcessColumn(next);
				}
				else
				{
					foreach (int c in _rowsToProcess)
					{
						if (next == -1 || currentMin > _rowsOptions[c].Count)
						{
							next = c;
							currentMin = _rowsOptions[c].Count;
						}
					}

					_rowsToProcess.Remove(next);
					succeded = ProcessRow(next);
				}

				if (succeded)
				{
					_printer.PrintGrid(Grid);
				}
			}

			return Grid.IsSolved();
		}

		private void ResetSolver()
		{
			_columnsToProcess = new List<int>();
			_rowsToProcess = new List<int>();

			_rowsCompleted = new bool[Grid.Size];
			_columnsCompleted = new bool[Grid.Size];

			_columnsOptions = new List<NonogramCellStatusEnum[]>[Grid.Size];
			_rowsOptions = new List<NonogramCellStatusEnum[]>[Grid.Size];

			for (int i = 0; i < Grid.Size; i++)
			{
				_columnsToProcess.Add(i);
				_rowsToProcess.Add(i);

				_rowsCompleted[i] = false;
				_columnsCompleted[i] = false;

				_columnsOptions[i] = new List<NonogramCellStatusEnum[]>();
				_rowsOptions[i] = new List<NonogramCellStatusEnum[]>();
			}

			Console.WriteLine("All rows and columns set to be processed");
			Console.WriteLine("All rows and columns set to not completed");
			Console.WriteLine("All rows and columns options reset");
			Console.WriteLine("Ready to solve");
		}

		private void CalculatePartialRowOptions(int r, int currentCounter, NonogramCellStatusEnum[] currentOption, int currentCell)
		{
			int countersLengthToGo = 0;
			for (int current = currentCounter; current < Grid.RowCounters[r].Length; current++)
			{
				countersLengthToGo += Grid.RowCounters[r][current];
			}
			countersLengthToGo += Grid.RowCounters[r].Length - 1 - currentCounter;

			if (currentCell <= Grid.Size - countersLengthToGo)
			{
				for (int myStart = currentCell; myStart <= Grid.Size - countersLengthToGo; myStart++)
				{
					var option = currentOption.Clone() as NonogramCellStatusEnum[];
					for (int l = 0; l < Grid.RowCounters[r][currentCounter]; l++)
					{
						option[myStart + l] = NonogramCellStatusEnum.FULL;
					}

					if (currentCounter == Grid.RowCounters[r].Length - 1)
					{
						_rowsOptions[r].Add(option);
					}
					else
					{
						CalculatePartialRowOptions(r, currentCounter + 1, option, myStart + Grid.RowCounters[r][currentCounter] + 1);
					}
				}
			}
		}

		private void CalculatePartialColumnOptions(int c, int currentCounter, NonogramCellStatusEnum[] currentOption, int currentCell)
		{
			int countersLengthToGo = 0;
			for (int current = currentCounter; current < Grid.ColumnCounters[c].Length; current++)
			{
				countersLengthToGo += Grid.ColumnCounters[c][current];
			}
			countersLengthToGo += Grid.ColumnCounters[c].Length - 1 - currentCounter;

			if (currentCell <= Grid.Size - countersLengthToGo)
			{
				for (int myStart = currentCell; myStart <= Grid.Size - countersLengthToGo; myStart++)
				{
					var option = currentOption.Clone() as NonogramCellStatusEnum[];
					for (int l = 0; l < Grid.ColumnCounters[c][currentCounter]; l++)
					{
						option[myStart + l] = NonogramCellStatusEnum.FULL;
					}

					if (currentCounter == Grid.ColumnCounters[c].Length - 1)
					{
						_columnsOptions[c].Add(option);
					}
					else
					{
						CalculatePartialColumnOptions(c, currentCounter + 1, option, myStart + Grid.ColumnCounters[c][currentCounter] + 1);
					}
				}
			}
		}

		private bool ProcessRow(int r)
		{
			bool succeded = false;

			// Clean options
			var rowOptionsToRemove = new List<NonogramCellStatusEnum[]>();
			foreach (var option in _rowsOptions[r])
			{
				for (int i = 0; i < Grid.Size; i++)
				{
					if (Grid.Cells[r, i] != NonogramCellStatusEnum.UNSET && Grid.Cells[r, i] != option[i])
					{
						rowOptionsToRemove.Add(option);

						break;
					}
				}
			}
			_rowsOptions[r].RemoveAll(o => rowOptionsToRemove.Contains(o));

			// Detect common elements
			for (int c = 0; c < Grid.Size; c++)
			{
				var value = NonogramCellStatusEnum.UNSET;

				foreach (var option in _rowsOptions[r])
				{
					if (value == NonogramCellStatusEnum.UNSET)
					{
						value = option[c];
					}
					else if (value != option[c])
					{
						value = NonogramCellStatusEnum.UNSET;

						break;
					}
				}

				if (value != NonogramCellStatusEnum.UNSET && Grid.Cells[r, c] == NonogramCellStatusEnum.UNSET)
				{
					Grid.Cells[r, c] = value;

					Console.WriteLine($"Cell [{ r }, { c }] set to { value }");

					if (!_columnsToProcess.Contains(c))
					{
						_columnsToProcess.Add(c);
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
			var columnOptionsToRemove = new List<NonogramCellStatusEnum[]>();
			foreach (var option in _columnsOptions[c])
			{
				for (int i = 0; i < Grid.Size; i++)
				{
					if (Grid.Cells[i, c] != NonogramCellStatusEnum.UNSET && Grid.Cells[i, c] != option[i])
					{
						columnOptionsToRemove.Add(option);

						break;
					}
				}
			}
			_columnsOptions[c].RemoveAll(o => columnOptionsToRemove.Contains(o));

			// Detect common elements
			for (int r = 0; r < Grid.Size; r++)
			{
				var value = NonogramCellStatusEnum.UNSET;

				foreach (var option in _columnsOptions[c])
				{
					if (value == NonogramCellStatusEnum.UNSET)
					{
						value = option[r];
					}
					else if (value != option[r])
					{
						value = NonogramCellStatusEnum.UNSET;

						break;
					}
				}

				if (value != NonogramCellStatusEnum.UNSET && Grid.Cells[r, c] == NonogramCellStatusEnum.UNSET)
				{
					Grid.Cells[r, c] = value;

					Console.WriteLine($"Cell [{ r }, { c }] set to { value }");

					if (!_rowsToProcess.Contains(r))
					{
						_rowsToProcess.Add(r);
					}

					succeded = true;
				}
			}

			return succeded;
		}

		private NonogramCellStatusEnum[] ProvisionsArray(NonogramCellStatusEnum value)
		{
			var array = new NonogramCellStatusEnum[Grid.Size];
			for (int i = 0; i < Grid.Size; i++)
			{
				array[i] = value;
			}

			return array;
		}
	}
}
