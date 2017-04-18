using System;
using System.Collections.Generic;

namespace Bearshirt
{
	public class BearshirtList2D<T> where T : new() {

		public int width { get; private set; }
		public int height { get; private set; }

		private List<T> list;

		public BearshirtList2D(int _width, int _height)
		{
			width = _width;
			height = _height;
			list = new List<T>(width * height);
			ForEach((int x, int y) => {
				list.Add(new T());
			});
		}

		public T this[int x, int y]
		{
			get
			{
				return list[y * width + x];
			}

			set
			{
				list[y * width + x] = value;
			}
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