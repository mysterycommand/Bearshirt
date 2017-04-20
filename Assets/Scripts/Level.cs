using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class Level : MonoBehaviour
	{
		[SerializeField] private MeshFilter bkgds;
		[SerializeField] private MeshFilter walls;
		[SerializeField] private MeshFilter edges;
		[SerializeField] private MeshFilter lavas;
		[SerializeField] private GameObject Hero;
		[SerializeField] private GameObject Door;

		private GameObject hero;
		private HeroController heroController;
		private GameObject door;

		private IntGrid bkgdGrid;
		private GridMesh bkgdMesh;

		private ProceduralGrid wallGrid;
		private GridMesh wallMesh;
		private MeshOutlines wallOutlines;

		private IntGrid edgeGrid;
		private GridMesh edgeMesh;

		private IntGrid lavaGrid;
		private GridMesh lavaMesh;

		void Start()
		{
			Debug.Log("Bearshirt.Level");

			wallGrid = new ProceduralGrid();
			// wallGrid = new ProceduralGrid(80, 45);
			wallMesh = new GridMesh(wallGrid, 1f);
			wallOutlines = new MeshOutlines(wallMesh);

			bkgdGrid = new IntGrid(wallGrid.width, wallGrid.height);
			bkgdMesh = new GridMesh(bkgdGrid, wallMesh.size);

			edgeGrid = new IntGrid(wallGrid.width, wallGrid.height);
			edgeMesh = new GridMesh(edgeGrid, wallMesh.size);

			lavaGrid = new IntGrid(wallGrid.width, wallGrid.height);
			lavaMesh = new GridMesh(lavaGrid, wallMesh.size);

			GenerateLevel();
		}

		void Update()
		{
			if (Input.GetButtonDown("Jump") && heroController.velocity != Vector2.zero)
			{
				float px = hero.transform.position.x + heroController.velocity.normalized.x;
				float py = hero.transform.position.y + heroController.velocity.normalized.y;
				Vector3 position = new Vector3(px, py, 0f);
				ToggleCell(position);
			}

			if (GlobalState.IsAtDoor)
			{
				GenerateLevel();
				GlobalState.IsAtDoor = false;
			}
		}

		private void ToggleCell(Vector3 worldPosition)
		{
			int x = (int) ((worldPosition.x - wallMesh.left) / wallMesh.size),
				y = (int) ((worldPosition.y - wallMesh.top) / wallMesh.size);

			if (wallGrid.IsBorder(x, y)) return;

			if (wallGrid.IsEmpty(x, y))
			{
				if (heroController.hasBlock)
				{
					heroController.hasBlock = false;
					wallGrid[x, y] = 1;
				}
			}
			else
			{
				if (!heroController.hasBlock)
				{
					heroController.hasBlock = true;
					wallGrid[x, y] = 0;
				}
			}

			wallGrid.NoKissing();
			UpdateLevel();
		}

		private void GenerateLevel()
		{
			int width = 16 * Random.Range(1, 5);
			int height = 9 * Random.Range(1, 5);
			wallGrid.SetSize(width, height);
			wallGrid.Generate();
			UpdateLevel();
			PlaceHero();
			PlaceDoor();
		}

		private void UpdateLevel()
		{
			UpdateGrids();
			UpdateMeshes();
			AddColliders();
		}

		private void PlaceHero()
		{
			Vector3 place = Vector3.zero;
			wallGrid.ForEach((int x, int y) => {
				if (place != Vector3.zero) return;

				int up = (int) Mathf.Min(y + 1, wallGrid.height - 1);
				bool isPlace = wallGrid.IsEdge(x, y) && wallGrid.IsEmpty(x, up);
				if (!isPlace) return;

				float px = wallMesh.left + x * wallMesh.size + (wallMesh.size / 2);
				float py = wallMesh.top + up * wallMesh.size + (wallMesh.size / 2);
				place = new Vector3(px, py, 0f);
			});

			if (hero == null)
			{
				hero = Instantiate(Hero);
				heroController = hero.GetComponent<HeroController>();
			}
			hero.transform.position = place;
			Camera.main.GetComponent<FollowTargets>().targets.Add(hero.transform);
		}

		private void PlaceDoor()
		{
			Vector3 place = Vector3.zero;
			wallGrid.ForEach((int x, int y) => {
				if (place != Vector3.zero) return;

				// dirty hack to start from the top right and work backwards
				x = wallGrid.width - 1 - x;
				y = wallGrid.height - 1 - y;

				int up = (int) Mathf.Min(y + 1, wallGrid.height - 1);
				bool isPlace = wallGrid.IsEdge(x, y) && wallGrid.IsEmpty(x, up);
				if (!isPlace) return;

				float px = wallMesh.left + x * wallMesh.size + (wallMesh.size / 2);
				float py = wallMesh.top + up * wallMesh.size + (wallMesh.size / 2);
				place = new Vector3(px, py, 0f);
			});

			if (door == null) door = Instantiate(Door);
			door.transform.position = place;
		}

		private void UpdateGrids()
		{
			bkgdGrid.SetSize(wallGrid.width, wallGrid.height);
			bkgdGrid.ForEach((int x, int y) => {
				bkgdGrid[x, y] = 1;
			});

			edgeGrid.SetSize(wallGrid.width, wallGrid.height);
			edgeGrid.ForEach((int x, int y) => {
				edgeGrid[x, y] = wallGrid.IsEdge(x, y) ? 1 : 0;
			});

			lavaGrid.SetSize(wallGrid.width, wallGrid.height);
			lavaGrid.ForRange(0, lavaGrid.width, 0, 5, (int x, int y) => {
				lavaGrid[x, y] = wallGrid.IsEmpty(x, y) ? 1 : 0;
			});
		}

		private void UpdateMeshes()
		{
			bkgds.mesh = bkgdMesh.Generate();
			walls.mesh = wallMesh.Generate();
			edges.mesh = edgeMesh.Generate();
			lavas.mesh = lavaMesh.Generate();
		}

		private void RemoveColliders()
		{
			EdgeCollider2D[] edgeColliders = GetComponents<EdgeCollider2D>();
			foreach (EdgeCollider2D edgeCollider in edgeColliders)
			{
				Destroy(edgeCollider);
			}
		}

		private void AddColliders()
		{
			RemoveColliders();

			List<List<Vector2>> outlines = wallOutlines.Generate();
			outlines.ForEach((List<Vector2> outline) => {
				EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
				edgeCollider.points = outline.ToArray();
			});
		}
	}
}
