using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class BearshirtMap
	{
		private List<int> seeds = new List<int>();
		private System.Random rando;

		BearshirtMap(int? seed)
		{
			seeds.Add(GetSeed(seed));
		}

		private int GetSeed(int? seed)
		{
			return seed ?? Time.time.ToString().GetHashCode();
		}
	}
}
