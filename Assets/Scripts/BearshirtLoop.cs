// using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class BearshirtLoop : MonoBehaviour
	{
		void Start()
		{
			Debug.Log("BearshirtLoop");
			GenerateMesh();
		}

		void Update()
		{
			if (Input.GetMouseButtonUp(0))
			{
				GenerateMesh();
			}
		}

		private void GenerateMesh()
		{
			BearshirtMap map = new BearshirtMap(160, 90);

			MeshFilter filter = GetComponent<MeshFilter>();
			Mesh mesh = new Mesh();

			float size = 0.1f,
				left = -(map.width * size) / 2,
				top = -(map.height * size) / 2;

			List<Vector3> verts = new List<Vector3>();
			Dictionary<string, int> indices = new Dictionary<string, int>();
			List<int> tris = new List<int>();

			// Debug.Log("ForEach start: " + Environment.TickCount);
			map.ForEach((int x, int y) => {
				if (map[x, y] != 1f) return;

				float t = top + (y + 1) * size;
				float r = left + (x + 1) * size;
				float b = top + y * size;
				float l = left + x * size;

				string lt = l + "," + t;
				string rt = r + "," + t;
				string rb = r + "," + b;
				string lb = l + "," + b;

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
			// Debug.Log("ForEach end: " + Environment.TickCount);

			// verts.ForEach((Vector3 vert) => Debug.Log(vert));
			// tris.ForEach((int tri) => Debug.Log(tri));

			mesh.vertices = verts.ToArray();
			mesh.triangles = tris.ToArray();
			mesh.RecalculateNormals();

			filter.mesh = mesh;
		}
	}
}
