using UnityEngine;

namespace Bearshirt
{
	public class ProceduralGrid : IntGrid
	{
		private int seed;
		private System.Random rando;

		public ProceduralGrid(int w, int h) : base(w, h)
		{
			Debug.Log("Bearshirt.ProceduralGrid");
			Generate();
		}

		public void Generate()
		{
			seed = GetSeed(null);
			rando = new System.Random(seed);

			Randomize(60);
			Smooth(4);
			NoKissing();
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

		public void NoKissing()
		{
			ForRange(1, width - 1, 1, height - 1, (int x, int y) => {
				int up = y + 1,
					rt = x + 1,
					dn = y - 1,
					lf = x - 1;

				// if it's empty up, right, down, and left, we'll call it an orphan
				bool isOrphan = (
						IsEmpty(x, up) &&
						IsEmpty(rt, y) &&
						IsEmpty(x, dn) &&
						IsEmpty(lf, y)
					);

				// if it's "checkerboarding" with another cell, we'll call it kissing
				// e.g. "up" and "right" are empty and "up-and-to-the-right" is solid
				bool isKissing = (
					(isOrphan && (
						IsSolid(rt, up) ||
						IsSolid(rt, dn) ||
						IsSolid(lf, dn) ||
						IsSolid(lf, up)
					)) ||
					(IsEmpty(x, up) && IsEmpty(rt, y) && IsSolid(rt, up)) ||
					(IsEmpty(rt, y) && IsEmpty(x, dn) && IsSolid(rt, dn)) ||
					(IsEmpty(x, dn) && IsEmpty(lf, y) && IsSolid(lf, dn)) ||
					(IsEmpty(lf, y) && IsEmpty(x, up) && IsSolid(lf, up))
				);

				if (isKissing) this[x, y] = 0;
			});
		}
	}
}
