using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class BearshirtMesh
	{
		public List<Vector3> verts { get; private set; }
		public Dictionary<string, int> indices { get; private set; }
		public List<int> tris { get; private set; }

		public float top { get; private set; }
		public float left { get; private set; }

		public BearshirtMap map { get; private set; }
		public float size { get; private set; }

		public BearshirtMesh(BearshirtMap _map, float _size)
		{
			map = _map;
			size = _size;
		}

		public Mesh Generate()
		{
			map.Generate();
			Mesh mesh = new Mesh();

			top = -(map.height * size) / 2;
			left = -(map.width * size) / 2;

			verts = new List<Vector3>();
			indices = new Dictionary<string, int>();
			tris = new List<int>();

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
					indices[lt] = verts.Count;
					verts.Add(new Vector3(l, t, 0f));
				}

				if (!indices.ContainsKey(rt))
				{
					indices[rt] = verts.Count;
					verts.Add(new Vector3(r, t, 0f));
				}

				if (!indices.ContainsKey(rb))
				{
					indices[rb] = verts.Count;
					verts.Add(new Vector3(r, b, 0f));
				}

				if (!indices.ContainsKey(lb))
				{
					indices[lb] = verts.Count;
					verts.Add(new Vector3(l, b, 0f));
				}

				tris.Add(indices[lt]);
				tris.Add(indices[rt]);
				tris.Add(indices[rb]);

				tris.Add(indices[lt]);
				tris.Add(indices[rb]);
				tris.Add(indices[lb]);
			});

			mesh.vertices = verts.ToArray();
			mesh.triangles = tris.ToArray();
			mesh.RecalculateNormals();

			return mesh;
		}
	}
}
