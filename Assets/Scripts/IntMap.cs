using System;

namespace Bearshirt
{
	public class IntMap {

		public int width { get; private set; }
		public int height { get; private set; }

		private int[,] cells;

		public IntMap(int w, int h)
		{
			width = w;
			height = h;
			cells = new int[width, height];
			ForEach((int x, int y) => {
				cells[x, y] = 0;
			});
		}

		public int this[int x, int y]
		{
			get { return cells[x, y]; }
			set { cells[x, y] = value; }
		}

		public bool IsEmpty(int x, int y)
		{
			return this[x, y] == 0;
		}

		public bool IsSolid(int x, int y)
		{
			return this[x, y] == 1;
		}

		public bool IsBorder(int x, int y)
		{
			return x == 0 || x == width - 1 || y == 0 || y == height - 1;
		}

		public bool IsEdge(int x, int y)
		{
			return IsSolid(x, y) &&
			(
				IsBorder(x, y) ||
				IsEmpty(x - 1, y) ||
				IsEmpty(x, y - 1) ||
				IsEmpty(x + 1, y) ||
				IsEmpty(x, y + 1)
			);
		}

		public void ForRange(
			int fromX = 0, int toX = 0,
			int fromY = 0, int toY = 0,
			Action<int, int> action = null)
		{
			if (action == null)
			{
				action = (int x, int y) => {};
			}

			for (int x = fromX; x < toX; ++x)
			{
				for (int y = fromY; y < toY; ++y)
				{
					action(x, y);
				}
			}
		}

		public void ForEach(Action<int, int> action = null)
		{
			ForRange(0, width, 0, height, action);
		}
	}
}