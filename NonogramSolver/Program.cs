using System;

namespace Nonogram_Solver
{
	class Program
	{
		static void Main(string[] args)
		{
			var grid = new NonogramGrid(
				new int[][] {
					new int[] { 1, 1, 1, 1 },
					new int[] { 2, 1, 1, 1, 1 },
					new int[] { 2, 1, 1, 1 },
					new int[] { 2, 2, 1 },
					new int[] { 2, 5 },
					new int[] { 2, 3, 4, 1 },
					new int[] { 4, 1, 1, 2, 1 },
					new int[] { 2, 6 },
					new int[] { 2, 1 },
					new int[] { 1, 3, 2 },
					new int[] { 2, 2, 2 },
					new int[] { 1, 2, 1, 2 },
					new int[] { 1, 2, 2, 1, 2 },
					new int[] { 2, 1, 1 },
					new int[] { 1, 1, 2, 2, 1 }
				},
				new int[][] {
					new int[] { 1 },
					new int[] { 1, 3, 1 },
					new int[] { 2, 3, 1, 1 },
					new int[] { 1, 2, 1 },
					new int[] { 1, 1 },
					new int[] { 2, 1, 2 },
					new int[] { 2, 3 },
					new int[] { 12 },
					new int[] { 4, 1, 6 },
					new int[] { 3 },
					new int[] { 3, 2, 1, 3 },
					new int[] { 5, 1 },
					new int[] { 4, 6 },
					new int[] { 1, 5 },
					new int[] { 7, 1 }
				}
				/*new int[][] {
					new int[] { 4, 1 },
					new int[] { 1, 3, 1 },
					new int[] { 1, 5 },
					new int[] { 7 },
					new int[] { 1, 1, 2 },
					new int[] { 2, 1, 3 },
					new int[] { 2, 3, 3 },
					new int[] { 1, 6, 1 },
					new int[] { 1, 6 },
					new int[] { 2, 2, 3 }
				},
				new int[][] {
					new int[] { 4 },
					new int[] { 2, 1 },
					new int[] { 4, 1, 2 },
					new int[] { 1, 2, 4 },
					new int[] { 1, 2, 4 },
					new int[] { 9 },
					new int[] { 3, 2 },
					new int[] { 9 },
					new int[] { 5, 1 },
					new int[] { 2, 3, 1 }
				}*/
			);

			var solver = new NonogramSolver(grid);
			var solved = solver.Solve();

			if (solved)
			{
				Console.WriteLine("Grid solved!");
			}
			else
			{
				Console.WriteLine("Gird not solved");
			}
		}
	}
}
