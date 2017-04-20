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
		[SerializeField] private GameObject hero;
		[SerializeField] private GameObject door;
		private HeroController heroController;
		private List<Transform> cameraTargets;

		private ProceduralGrid wallGrid;
		private GridMesh wallMesh;
		private MeshOutlines wallOutlines;

		private IntGrid bkgdGrid;
		private GridMesh bkgdMesh;

		private IntGrid edgeGrid;
		private GridMesh edgeMesh;

		private IntGrid lavaGrid;
		private GridMesh lavaMesh;
		private MeshOutlines lavaOutlines;


		private int lavaDepth = 5;

		void Start()
		{
			Debug.Log("Bearshirt.Level");

			heroController = hero.GetComponent<HeroController>();
			cameraTargets = Camera.main.GetComponent<FollowTargets>().targets;

			wallGrid = new ProceduralGrid();
			wallMesh = new GridMesh(wallGrid, 1f);
			wallOutlines = new MeshOutlines(wallMesh);

			bkgdGrid = new IntGrid(wallGrid.width, wallGrid.height);
			bkgdMesh = new GridMesh(bkgdGrid, wallMesh.size);

			edgeGrid = new IntGrid(wallGrid.width, wallGrid.height);
			edgeMesh = new GridMesh(edgeGrid, wallMesh.size);

			lavaGrid = new IntGrid(wallGrid.width, wallGrid.height);
			lavaMesh = new GridMesh(lavaGrid, wallMesh.size);
			lavaOutlines = new MeshOutlines(lavaMesh);

			GenerateStart();
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
				GlobalState.IsAtDoor = false;
				GenerateLevel();
			}

			if (GlobalState.Deaths == 3)
			{
				GlobalState.Deaths = 0;
				GlobalState.Levels = 0;
				GenerateLevel();
			}

			if (GlobalState.IsInLava)
			{
				GlobalState.IsInLava = false;
				ResetLevel();
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

		private void GenerateStart()
		{
			lavaDepth = 2;
			int[,] start = new int[,] {
				{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,},
				{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,},
				{1,0,1,1,1,0,1,0,0,0,1,1,1,0,1,0,1,0,1,0,1,},
				{1,0,1,0,1,0,1,0,0,0,1,0,1,0,1,0,1,0,1,0,1,},
				{1,0,1,1,1,0,1,0,0,0,1,1,1,0,1,1,1,0,1,0,1,},
				{1,0,1,0,0,0,1,0,0,0,1,0,1,0,0,1,0,0,0,0,1,},
				{1,0,1,0,0,0,1,1,1,0,1,0,1,0,0,1,0,0,1,0,1,},
				{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,},
				{1,0,0,1,1,1,0,1,1,1,0,1,1,1,0,1,1,1,0,0,1,},
				{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,},
			};

			int width = start.GetLength(1);
			int height = start.GetLength(0);
			wallGrid.SetSize(width, height);

			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					wallGrid[x, height - 1 - y] = start[y, x];
				}
			}

			cameraTargets.Add(door.transform);
			UpdateLevel();
			PlaceHero();
			PlaceDoor();
		}

		private void GenerateLevel()
		{
			++lavaDepth;
			if (cameraTargets.Contains(door.transform)) cameraTargets.Remove(door.transform);
			int width = 16 * Random.Range(1, 5);
			int height = 9 * Random.Range(1, 5) + lavaDepth;
			wallGrid.SetSize(width, height);
			wallGrid.Generate();
			UpdateLevel();
			PlaceHero();
			PlaceDoor();
		}

		private void ResetLevel()
		{
			wallGrid.Generate(false);
			UpdateLevel();
			PlaceHero();
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
				bool isPlace = wallGrid.IsEdge(x, y) && wallGrid.IsEmpty(x, up) && lavaGrid.IsEmpty(x, up);
				if (!isPlace) return;

				float px = wallMesh.left + x * wallMesh.size + (wallMesh.size / 2);
				float py = wallMesh.top + up * wallMesh.size + (wallMesh.size / 2);
				place = new Vector3(px, py, 0f);
			});

			heroController.hasBlock = false;
			hero.transform.position = place;
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
				bool isPlace = wallGrid.IsEdge(x, y) && wallGrid.IsEmpty(x, up) && lavaGrid.IsEmpty(x, up);
				if (!isPlace) return;

				float px = wallMesh.left + x * wallMesh.size + (wallMesh.size / 2);
				float py = wallMesh.top + up * wallMesh.size + (wallMesh.size / 2);
				place = new Vector3(px, py, 0f);
			});

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
			lavaGrid.ForRange(0, lavaGrid.width, 0, lavaDepth, (int x, int y) => {
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
			EdgeCollider2D[] edgeColliders = GetComponentsInChildren<EdgeCollider2D>();
			foreach (EdgeCollider2D edgeCollider in edgeColliders)
			{
				Destroy(edgeCollider);
			}
		}

		private void AddColliders()
		{
			RemoveColliders();
			List<List<Vector2>> outlines;

			outlines = wallOutlines.Generate();
			outlines.ForEach((List<Vector2> outline) => {
				EdgeCollider2D edgeCollider = walls.gameObject.AddComponent<EdgeCollider2D>();
				edgeCollider.points = outline.ToArray();
			});

			outlines = lavaOutlines.Generate();
			outlines.ForEach((List<Vector2> outline) => {
				EdgeCollider2D edgeCollider = lavas.gameObject.AddComponent<EdgeCollider2D>();
				edgeCollider.points = outline.ToArray();
				edgeCollider.isTrigger = true;
			});
		}
	}
}
