using System;

namespace Nonogram_Solver
{
	public class NonogramGrid
	{
		public int Size { get; }
		
		public NonogramCellStatusEnum[,] Cells;

		public int[][] ColumnCounters { get; }
		public int[][] RowCounters { get; }

		public NonogramGrid(int[][] columnCounters, int[][] rowCounters)
		{
			if (columnCounters.Length != rowCounters.Length)
			{
				throw new ArgumentException("The number of counters for rows and columns is not the same");
			}
			
			Size = columnCounters.Length;

			ResetGrid();

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
		}

		public bool IsSolved()
		{
			var solved = true;

			foreach (var cell in Cells)
			{
				solved &= (cell != NonogramCellStatusEnum.UNSET);
			}

			return solved;
		}

		public void ResetGrid()
		{
			Cells = new NonogramCellStatusEnum[Size, Size];

			for (int r = 0; r < Size; r++)
			{
				for (int c = 0; c < Size; c++)
				{
					Cells[r, c] = NonogramCellStatusEnum.UNSET;
				}
			}
		}
	}
}
