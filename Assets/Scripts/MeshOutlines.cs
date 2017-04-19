using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class MeshOutlines
	{
		public List<List<Vector3>> outlines { get; private set; }

		public GridMesh mesh { get; private set; }

		private Dictionary<Vector3, int> vertexOutlineCount;

		public MeshOutlines(GridMesh _mesh)
		{
			mesh = _mesh;
		}

		public List<List<Vector2>> Generate()
		{
			outlines = new List<List<Vector3>>();

			vertexOutlineCount = new Dictionary<Vector3, int>();

			mesh.vertices
				.FindAll(new Predicate<Vector3>((Vector3 vertex) => {
					bool isOutline = mesh.vertexTriangles[vertex].Count < 6;
					if (isOutline) vertexOutlineCount[vertex] = GetOutlineCount(vertex);
					return isOutline;
				}))
				.ForEach((Vector3 vertex) => {
					if (vertexOutlineCount[vertex] == 0) return;
					--vertexOutlineCount[vertex];
					outlines.Add(GetOutline(vertex));
				});

			return outlines.ConvertAll<List<Vector2>>((List<Vector3> vertices) => {
				return vertices.ConvertAll<Vector2>((Vector3 vertex) => {
					return new Vector2(vertex.x, vertex.y);
				});
			});
		}

		private int GetOutlineCount(Vector3 vertex)
		{
			// bool isOuterBound = (
			// 	vertex.y == mesh.top ||
			// 	vertex.x == mesh.right ||
			// 	vertex.y == mesh.bottom ||
			// 	vertex.x == mesh.left
			// );
			// if (isOuterBound) return 1;

			// int x = (int) ((vertex.x - mesh.left) / mesh.size);
			// int y = (int) ((vertex.y - mesh.top) / mesh.size);
			// IntGrid grid = mesh.grid;

			// bool isKissing = (
			// 	((grid.IsEdge(x, y) && grid.IsEdge(x - 1, y - 1)) &&
			// 	(grid.IsEmpty(x - 1, y) && grid.IsEmpty(x, y - 1))) ||
			// 	((grid.IsEdge(x - 1, y) && grid.IsEdge(x, y - 1)) &&
			// 	(grid.IsEmpty(x, y) && grid.IsEmpty(x - 1, y - 1)))
			// );
			// if (isKissing) return 2;

			return 1;
		}

		private List<Vector3> GetOutline(Vector3 vertex)
		{
			List<Vector3> outline = new List<Vector3>();
			outline.Add(vertex);
			Follow(outline, vertex);
			outline.Add(vertex);
			return outline;
		}

		private void Follow(List<Vector3> outline, Vector3 vertex)
		{
			List<Triangle> triangles = mesh.vertexTriangles[vertex];
			List<Vector3> vertices = new List<Vector3>();

			triangles.ForEach((Triangle t) => {
				for (int i = 0; i < 3; ++i)
				{
					Vector3 v = mesh.vertices[t[i]];
					bool isOutline = vertexOutlineCount.ContainsKey(v);
					bool isChecked = isOutline && vertexOutlineCount[v] == 0;
					if (!isChecked) vertices.Add(v);
				}
			});

			vertices = vertices.FindAll(new Predicate<Vector3>((Vector3 v) => {
				return mesh.vertexTriangles[v].FindAll(new Predicate<Triangle>((Triangle t) => {
					return t.Contains(mesh.vertexIndices[vertex]);
				})).Count == 1;
			}));

			if (vertices.Count == 0) return;

			Vector3 next = vertices[0];
			--vertexOutlineCount[next];
			outline.Add(next);
			Follow(outline, next);
		}
	}
}
