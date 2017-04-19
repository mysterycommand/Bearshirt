﻿namespace Bearshirt
{
	public class Triangle {

		public int a { get; private set; }
		public int b { get; private set; }
		public int c { get; private set; }

		public int[] vertexIndices;

		public Triangle(int _a, int _b, int _c)
		{
			a = _a;
			b = _b;
			c = _c;

			vertexIndices = new int[3] {
				a, b, c,
			};
		}

		public int this[int i] { get { return vertexIndices[i]; } }

	}
}