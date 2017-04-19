using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class MeshOutlines
	{
		public List<List<Vector3>> outlines { get; private set; }

		public GridMesh mesh { get; private set; }

		private HashSet<Vector3> checkedVertices;

		public MeshOutlines(GridMesh _mesh)
		{
			mesh = _mesh;
		}

		public List<List<Vector2>> Generate()
		{
			outlines = new List<List<Vector3>>();

			checkedVertices = new HashSet<Vector3>();

			mesh.vertices
				.FindAll(new Predicate<Vector3>((Vector3 vertex) => {
					bool isOutline = mesh.vertexTriangles[vertex].Count < 6;
					return isOutline;
				}))
				.ForEach((Vector3 vertex) => {
					if (checkedVertices.Contains(vertex)) return;
					outlines.Add(GetOutline(vertex));
				});

			return outlines.ConvertAll<List<Vector2>>((List<Vector3> vertices) => {
				return vertices.ConvertAll<Vector2>((Vector3 vertex) => {
					return new Vector2(vertex.x, vertex.y);
				});
			});
		}

		private List<Vector3> GetOutline(Vector3 vertex)
		{
			List<Vector3> outline = new List<Vector3>();
			checkedVertices.Add(vertex);
			outline.Add(vertex);
			Trace(outline, vertex);
			outline.Add(vertex);
			return outline;
		}

		private void Trace(List<Vector3> outline, Vector3 vertex)
		{
			List<Triangle> triangles = mesh.vertexTriangles[vertex];
			List<Vector3> vertices = new List<Vector3>();

			triangles.ForEach((Triangle t) => {
				for (int i = 0; i < 3; ++i)
				{
					Vector3 v = mesh.vertices[t[i]];
					if (!checkedVertices.Contains(v)) vertices.Add(v);
				}
			});

			vertices = vertices.FindAll(new Predicate<Vector3>((Vector3 v) => {
				return mesh.vertexTriangles[v].FindAll(new Predicate<Triangle>((Triangle t) => {
					return t.Contains(mesh.vertexIndices[vertex]);
				})).Count == 1;
			}));

			if (vertices.Count == 0) return;

			Vector3 next = vertices[0];
			checkedVertices.Add(next);
			outline.Add(next);
			Trace(outline, next);
		}
	}
}
