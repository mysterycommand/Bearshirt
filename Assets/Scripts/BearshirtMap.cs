using System;
using UnityEngine;

namespace Bearshirt
{
	public class BearshirtMap
	{
		public int width { get; private set; }
		public int height { get; private set; }
		private int seed;
		private System.Random rando;

		private Texture2D map;

		public BearshirtMap(int _width, int _height)
		{
			width = _width;
			height = _height;

			seed = GetSeed(null);
			rando = new System.Random(seed);
			Debug.Log("BearshirtMap: " + seed);

			GenerateMap();
		}

		public void GenerateMap()
		{
			map = GetTexture(width, height);
			ForRange(1, width - 1, 1, height - 1, (int x, int y) => {
				int n = 0;
				ForRange(x - 1, x + 2, y - 1, y + 2, (int c, int r) => {
					if (c == x && r == y) return;
					n += (int) this[c, r];
				});

				if (n > 4) map.SetPixel(x, y, Color.white);
				else if (n < 4) map.SetPixel(x, y, Color.black);
			});
			map.Apply();
		}

		public float this[int x, int y]
		{
			get
			{
				return map.GetPixel(x, y).r;
			}
		}

		public void ForRange(
			int fx = 0, int tx = 0, int fy = 0, int ty = 0,
			Action<int, int> action = null)
		{
			if (action == null)
			{
				action = (int x, int y) => {};
			}

			for (int x = fx; x < tx; ++x)
			{
				for (int y = fy; y < ty; ++y)
				{
					action(x, y);
				}
			}
		}

		public void ForEach(Action<int, int> action = null)
		{
			ForRange(0, width, 0, height, action);
		}

		private Texture2D GetTexture(int width, int height)
		{
			Texture2D tex = new Texture2D(width, height);
			ForEach((int x, int y) => {
				bool isBorder = x == 0 || x == width - 1 || y == 0 || y == height - 1;
				bool isSolid = rando.Next(0, 255) / 255f > 0.6f;
				Color c = isBorder || isSolid ? Color.white : Color.black;
				tex.SetPixel(x, y, c);
			});
			tex.Apply();
			return tex;
		}

		private int GetSeed(int? seed)
		{
			return seed ?? Time.time.ToString().GetHashCode();
		}
	}
}
