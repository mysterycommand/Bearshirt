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
			List<int> tris = new List<int>();

			map.ForEach((int x, int y) => {
				if (map[x, y] != 1f) return;

				float t = top + (y + 1) * size;
				float r = left + (x + 1) * size;
				float b = top + y * size;
				float l = left + x * size;

				Vector3 lt = new Vector3(l, t, 0f);
				Vector3 rt = new Vector3(r, t, 0f);
				Vector3 rb = new Vector3(r, b, 0f);
				Vector3 lb = new Vector3(l, b, 0f);

				if (!verts.Contains(lt)) verts.Add(lt);
				if (!verts.Contains(rt)) verts.Add(rt);
				if (!verts.Contains(rb)) verts.Add(rb);
				if (!verts.Contains(lb)) verts.Add(lb);

				tris.Add(verts.IndexOf(lt));
				tris.Add(verts.IndexOf(rt));
				tris.Add(verts.IndexOf(rb));

				tris.Add(verts.IndexOf(lt));
				tris.Add(verts.IndexOf(rb));
				tris.Add(verts.IndexOf(lb));
			});

			// verts.ForEach((Vector3 vert) => Debug.Log(vert));
			// tris.ForEach((int tri) => Debug.Log(tri));

			mesh.vertices = verts.ToArray();
			mesh.triangles = tris.ToArray();
			mesh.RecalculateNormals();

			filter.mesh = mesh;
		}
	}
}
