using UnityEngine;

namespace Bearshirt
{
	public class BearshirtMap
	{
		public int width { get; private set; }
		public int height { get; private set; }
		public BearshirtList2D<int> list { get; private set; }

		private int seed;
		private System.Random rando;

		public BearshirtMap(int _width, int _height)
		{
			width = _width;
			height = _height;
			list = new BearshirtList2D<int>(width, height);

			seed = GetSeed(5);
			rando = new System.Random(seed);
			Debug.Log("BearshirtMap: " + seed);

			Generate();
		}

		public void Generate()
		{
			RandomizeList(60);
			SmoothList(4);
		}

		private int GetSeed(int? seed)
		{
			return seed ?? Time.time.ToString().GetHashCode();
		}

		private void RandomizeList(int fill)
		{
			list.ForEach((int x, int y) => {
				bool isBorder = x == 0 || x == width - 1 || y == 0 || y == height - 1,
					isSolid = rando.Next(0, 100) > fill;

				int val = isBorder || isSolid ? 1 : 0;
				list[x, y] = val;
			});
		}

		private void SmoothList(int keep)
		{
			list.ForRange(1, width - 1, 1, height - 1, (int x, int y) => {
				int n = 0;
				list.ForRange(x - 1, x + 2, y - 1, y + 2, (int c, int r) => {
					if (c == x && r == y) return;
					n += (int) list[c, r];
				});

				if (n > keep) list[x, y] = 1;
				else if (n < keep) list[x, y] = 0;
			});
		}
	}
}
