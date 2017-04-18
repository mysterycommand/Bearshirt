using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class BearshirtMesh
	{
		public List<Vector3> vertices { get; private set; }
		public Dictionary<string, int> indices { get; private set; }
		public List<int> triangles { get; private set; }
		public Dictionary<string, int> triangleCount { get; private set; }

		public float top { get; private set; }
		public float left { get; private set; }

		public IntMap map { get; private set; }
		public float size { get; private set; }

		public BearshirtMesh(IntMap _map, float _size)
		{
			map = _map;
			size = _size;
		}

		public Mesh Generate()
		{
			Mesh mesh = new Mesh();

			top = -(map.height * size) / 2;
			left = -(map.width * size) / 2;

			vertices = new List<Vector3>();
			indices = new Dictionary<string, int>();
			triangles = new List<int>();
			triangleCount = new Dictionary<string, int>();

			map.ForEach((int x, int y) => {
				if (map[x, y] != 1f) return;

				float t = top + (y + 1) * size,
					r = left + (x + 1) * size,
					b = top + y * size,
					l = left + x * size;

				string lt = l + "," + t,
					rt = r + "," + t,
					rb = r + "," + b,
					lb = l + "," + b;

				if (!indices.ContainsKey(lt))
				{
					indices[lt] = vertices.Count;
					triangleCount[lt] = 0;
					vertices.Add(new Vector3(l, t, 0f));
				}

				if (!indices.ContainsKey(rt))
				{
					indices[rt] = vertices.Count;
					triangleCount[rt] = 0;
					vertices.Add(new Vector3(r, t, 0f));
				}

				if (!indices.ContainsKey(rb))
				{
					indices[rb] = vertices.Count;
					triangleCount[rb] = 0;
					vertices.Add(new Vector3(r, b, 0f));
				}

				if (!indices.ContainsKey(lb))
				{
					indices[lb] = vertices.Count;
					triangleCount[lb] = 0;
					vertices.Add(new Vector3(l, b, 0f));
				}

				triangles.Add(indices[lt]);
				triangleCount[lt]++;
				triangles.Add(indices[rt]);
				triangleCount[rt]++;
				triangles.Add(indices[rb]);
				triangleCount[rb]++;

				triangles.Add(indices[lt]);
				triangleCount[lt]++;
				triangles.Add(indices[rb]);
				triangleCount[rb]++;
				triangles.Add(indices[lb]);
				triangleCount[lb]++;
			});

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.RecalculateNormals();

			return mesh;
		}
	}
}
