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

		private ProceduralGrid wallMap;
		private GridMesh wallMesh;
		private MeshOutlines wallOutlines;

		private IntGrid edgeMap;
		private GridMesh edgeMesh;

		private List<Vector3> edgeVertices;

		void Start()
		{
			Debug.Log("BearshirtLoop");

			// wallMap = new ProceduralGrid(80, 45);
			wallMap = new ProceduralGrid(16, 9);
			wallMesh = new GridMesh(wallMap, 1f);
			wallOutlines = new MeshOutlines(wallMesh);

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
			wallMap.Generate();
			walls.mesh = wallMesh.Generate();

			edgeMap = new IntGrid(wallMap.width, wallMap.height);
			wallMap.ForEach((int x, int y) => {
				edgeMap[x, y] = wallMap.IsEdge(x, y) ? 1 : 0;
			});
			edgeMesh = new GridMesh(edgeMap, wallMesh.size);
			edges.mesh = edgeMesh.Generate();

			// edgeVertices = wallMesh.vertices.FindAll(new Predicate<Vector3>((Vector3 vertex) => {
			// 	return wallMesh.vertexTriangles[vertex].Count < 6;
			// }));
			AddColliders();
		}

		private void RemoveColliders()
		{
			EdgeCollider2D[] colliders = GetComponents<EdgeCollider2D>();
			foreach (EdgeCollider2D collider in colliders)
			{
				Destroy(collider);
			}
		}

		private void AddColliders()
		{
			RemoveColliders();

			List<List<Vector2>> outlines = wallOutlines.Generate();
			outlines.ForEach((List<Vector2> outline) => {
				EdgeCollider2D collider = gameObject.AddComponent<EdgeCollider2D>();
				collider.points = outline.ToArray();
			});
		}
	}
}
