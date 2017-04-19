using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class BearshirtLoop : MonoBehaviour
	{
		[SerializeField]
		private MeshFilter walls;

		[SerializeField]
		private MeshFilter edges;

		private BearshirtMap map;
		private BearshirtMesh mesh;

		private List<Vector3> edgeVertices;

		void Start()
		{
			Debug.Log("BearshirtLoop");

			// map = new BearshirtMap(80, 45);
			map = new BearshirtMap(16, 9);
			mesh = new BearshirtMesh(map, 1f);

			GenerateLevel();
		}

		void Update()
		{
			if (Input.GetMouseButtonUp(0))
			{
				GenerateLevel();
			}
		}

		void OnDrawGizmos()
		{
			if (edgeVertices == null) return;
			Debug.Log("OnDrawGizmos: " + edgeVertices.Count);
			foreach (Vector3 vertex in edgeVertices)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawCube(vertex, Vector3.one / 4);
			}
		}

		private void GenerateLevel()
		{
			map.Generate();
			walls.mesh = mesh.Generate();

			IntGrid edgeMap = new IntGrid(map.width, map.height);
			map.ForEach((int x, int y) => {
				edgeMap[x, y] = map.IsEdge(x, y) ? 1 : 0;
			});
			BearshirtMesh edgeMesh = new BearshirtMesh(edgeMap, 1f);
			edges.mesh = edgeMesh.Generate();

			edgeVertices = mesh.vertices.FindAll(new Predicate<Vector3>((Vector3 vertex) => {
				return mesh.vertexTriangles[vertex].Count < 6;
			}));

			string verts = "";
			edgeVertices.ForEach((Vector3 vertex) => {
				verts += vertex.ToString() + "\n";
			});
			Debug.Log(verts);

			// GenerateColliders();
		}

		private void RemoveColliders()
		{
			EdgeCollider2D[] colliders = GetComponents<EdgeCollider2D>();
			foreach (EdgeCollider2D collider in colliders)
			{
				Destroy(collider);
			}
		}

		// private void GenerateColliders()
		// {
		// 	RemoveColliders();
		// 	List<Vector3> outline = new List<Vector3>();

		// 	// top
		// 	map.ForRange(0, map.width, map.height - 1, map.height, (int x, int y) => {
		// 		float t = mesh.top + (y + 1) * mesh.size,
		// 			l = mesh.left + x * mesh.size;

		// 		string lt = l + "," + t;
		// 		outline.Add(mesh.vertices[mesh.vertexIndices[lt]]);
		// 	});

		// 	// right
		// 	int count = outline.Count;
		// 	map.ForRange(map.width - 1, map.width, 0, map.height, (int x, int y) => {
		// 		float t = mesh.top + (y + 1) * mesh.size,
		// 			r = mesh.left + (x + 1) * mesh.size;

		// 		string rt = r + "," + t;
		// 		outline.Insert(count, mesh.vertices[mesh.vertexIndices[rt]]);
		// 	});

		// 	// bottom
		// 	count = outline.Count;
		// 	map.ForRange(0, map.width, 0, 1, (int x, int y) => {
		// 		float r = mesh.left + (x + 1) * mesh.size,
		// 			b = mesh.top + y * mesh.size;

		// 		string rb = r + "," + b;
		// 		outline.Insert(count, mesh.vertices[mesh.vertexIndices[rb]]);
		// 	});

		// 	// left
		// 	map.ForRange(0, 1, 0, map.height, (int x, int y) => {
		// 		float b = mesh.top + y * mesh.size,
		// 			l = mesh.left + x * mesh.size;

		// 		string lb = l + "," + b;
		// 		outline.Add(mesh.vertices[mesh.vertexIndices[lb]]);
		// 	});

		// 	// close
		// 	outline.Add(outline[0]);

		// 	EdgeCollider2D collider = gameObject.AddComponent<EdgeCollider2D>();
		// 	List<Vector2> points = outline.ConvertAll<Vector2>((Vector3 vert) => {
		// 		return new Vector2(vert.x, vert.y);
		// 	});
		// 	collider.points = points.ToArray();
		// }
	}
}
