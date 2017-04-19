﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class MeshOutlines
	{
		public List<List<Vector3>> edges { get; private set; }

		public GridMesh mesh { get; private set; }

		private HashSet<Vector3> edgeVertices;
		private HashSet<Vector3> checkedVertices;
		private List<Vector3> uncheckVertices = new List<Vector3>();

		public MeshOutlines(GridMesh _mesh)
		{
			mesh = _mesh;
		}

		public List<List<Vector2>> Generate()
		{
			edges = new List<List<Vector3>>();

			edgeVertices = new HashSet<Vector3>();
			checkedVertices = new HashSet<Vector3>();

			mesh.vertices
				.FindAll(new Predicate<Vector3>((Vector3 vertex) => {
					bool isEdge = mesh.vertexTriangles[vertex].Count < 6;
					if (isEdge) edgeVertices.Add(vertex);
					return isEdge;
				}))
				.ForEach((Vector3 vertex) => {
					if (checkedVertices.Contains(vertex)) return;
					checkedVertices.Add(vertex);
					edges.Add(GetEdge(vertex));
				});

			return edges.ConvertAll<List<Vector2>>((List<Vector3> vertices) => {
				return vertices.ConvertAll<Vector2>((Vector3 vertex) => {
					return new Vector2(vertex.x, vertex.y);
				});
			});
		}

		private List<Vector3> GetEdge(Vector3 vertex)
		{
			List<Vector3> edge = new List<Vector3>();
			edge.Add(vertex);
			FollowEdge(edge, vertex);
			edge.Add(vertex);

			// while (uncheckVertices.Count > 0) {
			// 	Vector3 v = uncheckVertices[0];
			// 	if (checkedVertices.Contains(v)) checkedVertices.Remove(v);
			// 	uncheckVertices.Remove(v);
			// }

			return edge;
		}

		private void FollowEdge(List<Vector3> edge, Vector3 vertex)
		{
			List<Triangle> triangles = mesh.vertexTriangles[vertex];
			List<Vector3> vertices = new List<Vector3>();

			triangles.ForEach((Triangle t) => {
				Vector3 va = mesh.vertices[t.a],
					vb = mesh.vertices[t.b],
					vc = mesh.vertices[t.c];

				if (!checkedVertices.Contains(va)) vertices.Add(va);
				if (!checkedVertices.Contains(vb))
				{
					vertices.Add(vb);
					// uncheckVertices.Add(vb);
				}
				if (!checkedVertices.Contains(vc)) vertices.Add(vc);
			});

			vertices = vertices.FindAll(new Predicate<Vector3>((Vector3 v) => {
				return mesh.vertexTriangles[v].FindAll(new Predicate<Triangle>((Triangle t) => {
					return t.Contains(mesh.vertexIndices[vertex]);
				})).Count == 1;
			}));

			if (vertices.Count == 0) return;

			Vector3 next = vertices[0];
			checkedVertices.Add(next);
			edge.Add(next);
			FollowEdge(edge, next);
		}
	}
}
