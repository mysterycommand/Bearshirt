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

			map = GetTexture(width, height);
		}

		private Texture2D GetTexture(int width, int height)
		{
			Texture2D tex = new Texture2D(width, height);
			ForEach((int x, int y) => {
				float v = rando.Next(0, 1);
				tex.SetPixel(x, y, new Color(v, v, v));
			});
			tex.Apply();
			return tex;
		}

		private int GetSeed(int? seed)
		{
			return seed ?? Time.time.ToString().GetHashCode();
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
	}
}
