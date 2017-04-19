using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class BearshirtMesh
	{
		public List<Vector3> vertices { get; private set; }
		public List<int> triangles { get; private set; }
		public Dictionary<Vector3, int> vertexIndices { get; private set; }
		public Dictionary<Vector3, List<Triangle>> vertexTriangles { get; private set; }

		public float top { get; private set; }
		public float right { get { return left + (map.width * size); } }
		public float bottom { get { return top + (map.height * size); } }
		public float left { get; private set; }

		public IntGrid map { get; private set; }
		public float size { get; private set; }

		public BearshirtMesh(IntGrid _map, float _size)
		{
			map = _map;
			size = _size;

			top = -(map.height * size) / 2;
			left = -(map.width * size) / 2;
		}

		public bool IsEdge(Vector3 vertex)
		{
			return vertex.y == top ||
				vertex.x == right ||
				vertex.y == bottom ||
				vertex.x == left;
		}

		public Mesh Generate()
		{
			Mesh mesh = new Mesh();

			vertices = new List<Vector3>();
			triangles = new List<int>();

			vertexIndices = new Dictionary<Vector3, int>();
			vertexTriangles = new Dictionary<Vector3, List<Triangle>>();

			map.ForEach((int x, int y) => {
				if (map[x, y] != 1f) return;

				float t = top + (y + 1) * size,
					r = left + (x + 1) * size,
					b = top + y * size,
					l = left + x * size;

				Vector3 lt = new Vector3(l, t, 0f),
					rt = new Vector3(r, t, 0f),
					rb = new Vector3(r, b, 0f),
					lb = new Vector3(l, b, 0f);

				AddVertex(lt);
				AddVertex(rt);
				AddVertex(rb);
				AddVertex(lb);

				AddTriangle(lt, rt, rb);
				AddTriangle(lt, rb, lb);
			});

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.RecalculateNormals();

			return mesh;
		}

		private void AddVertex(Vector3 vertex)
		{
			if (vertexIndices.ContainsKey(vertex)) return;

			vertexTriangles[vertex] = new List<Triangle>();
			vertexIndices[vertex] = vertices.Count;
			vertices.Add(vertex);
		}

		private void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
		{
			int ia = vertexIndices[a],
				ib = vertexIndices[b],
				ic = vertexIndices[c];
			triangles.Add(ia);
			triangles.Add(ib);
			triangles.Add(ic);

			Triangle tri = new Triangle(ia, ib, ic);
			vertexTriangles[a].Add(tri);
			vertexTriangles[b].Add(tri);
			vertexTriangles[c].Add(tri);
		}
	}
}
