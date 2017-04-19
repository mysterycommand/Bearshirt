using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class Level : MonoBehaviour
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

			// wallGrid = new ProceduralGrid(16, 9);
			wallGrid = new ProceduralGrid(80, 45);
			wallMesh = new GridMesh(wallGrid, 1f);
			wallOutlines = new MeshOutlines(wallMesh);

			edgeGrid = new IntGrid(wallGrid.width, wallGrid.height);
			edgeMesh = new GridMesh(edgeGrid, wallMesh.size);

			GenerateLevel();
		}

		void Update()
		{
			if (Input.GetMouseButtonUp(0)) ToggleCell(Input.mousePosition);
			if (Input.GetMouseButtonUp(1)) GenerateLevel();
		}

		private void ToggleCell(Vector3 screenPosition)
		{
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
			int x = (int) ((worldPosition.x - wallMesh.left) / wallMesh.size),
				y = (int) ((worldPosition.y - wallMesh.top) / wallMesh.size);

			if (wallGrid.IsBorder(x, y)) return;

			wallGrid[x, y] = wallGrid.IsEmpty(x, y) ? 1 : 0;
			wallGrid.NoKissing();
			UpdateLevel();
		}

		private void GenerateLevel()
		{
			wallGrid.Generate();
			UpdateLevel();
		}

		private void UpdateLevel()
		{
			UpdateGrids();
			UpdateMeshes();
			AddColliders();
		}

		private void UpdateGrids()
		{
			wallGrid.ForEach((int x, int y) => {
				edgeGrid[x, y] = wallGrid.IsEdge(x, y) ? 1 : 0;
			});
		}

		private void UpdateMeshes()
		{
			walls.mesh = wallMesh.Generate();
			edges.mesh = edgeMesh.Generate();
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
