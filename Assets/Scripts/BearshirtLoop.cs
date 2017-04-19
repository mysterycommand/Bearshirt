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

		private ProceduralGrid wallGrid;
		private GridMesh wallMesh;
		private MeshOutlines wallOutlines;

		private IntGrid edgeGrid;
		private GridMesh edgeMesh;

		private List<Vector3> edgeVertices;

		void Start()
		{
			Debug.Log("BearshirtLoop");

			wallGrid = new ProceduralGrid(80, 45);
			wallMesh = new GridMesh(wallGrid, 1f);
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

		private void GenerateLevel()
		{
			wallGrid.Generate();
			walls.mesh = wallMesh.Generate();

			edgeGrid = new IntGrid(wallGrid.width, wallGrid.height);
			wallGrid.ForEach((int x, int y) => {
				edgeGrid[x, y] = wallGrid.IsEdge(x, y) ? 1 : 0;
			});
			edgeMesh = new GridMesh(edgeGrid, wallMesh.size);
			edges.mesh = edgeMesh.Generate();

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
