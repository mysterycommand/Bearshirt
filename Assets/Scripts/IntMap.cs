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