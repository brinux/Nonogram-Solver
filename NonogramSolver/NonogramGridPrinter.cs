using System;

namespace Nonogram_Solver
{
	public class NonogramGridPrinter
	{
		public void PrintGrid(NonogramGrid grid)
		{
			for (int r = 0; r < grid.Size; r++)
			{
				for (int c = 0; c < grid.Size; c++)
				{
					switch (grid.Cells[r, c])
					{
						case NonogramCellStatusEnum.UNSET:
							Console.Write("░ ");
							break;
						case NonogramCellStatusEnum.FULL:
							Console.Write("█ ");
							break;
						case NonogramCellStatusEnum.EMPTY:
							Console.Write("X ");
							break;
					}
				}

				Console.WriteLine();
			}

			//Console.ReadLine();
		}
	}
}
