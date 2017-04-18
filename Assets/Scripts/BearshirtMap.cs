using UnityEngine;

namespace Bearshirt
{
	public class BearshirtMap : IntMap
	{
		private int seed;
		private System.Random rando;

		public BearshirtMap(int w, int h) : base(w, h)
		{
			Debug.Log("BearshirtMap: " + seed);
			Generate();
		}

		public void Generate()
		{
			// seed = GetSeed(null);
			seed = GetSeed(5);
			rando = new System.Random(seed);

			Randomize(60);
			Smooth(4);
		}

		private int GetSeed(int? seed)
		{
			return seed ?? Time.time.ToString().GetHashCode();
		}

		private void Randomize(int fill)
		{
			ForEach((int x, int y) => {
				bool isBorder = IsBorder(x, y),
					isSolid = rando.Next(0, 100) > fill;

				int val = isBorder || isSolid ? 1 : 0;
				this[x, y] = val;
			});
		}

		private void Smooth(int keep)
		{
			ForRange(1, width - 1, 1, height - 1, (int x, int y) => {
				int n = 0;
				ForRange(x - 1, x + 2, y - 1, y + 2, (int c, int r) => {
					if (c == x && r == y) return;
					n += this[c, r];
				});

				if (n > keep) this[x, y] = 1;
				else if (n < keep) this[x, y] = 0;
			});
		}
	}
}
