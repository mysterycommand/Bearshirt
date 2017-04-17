using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class BearshirtLoop : MonoBehaviour
	{
		private MeshFilter filter;

		private BearshirtMap map;
		private BearshirtMesh mesh;

		void Start()
		{
			Debug.Log("BearshirtLoop");

			filter = GetComponent<MeshFilter>();

			map = new BearshirtMap(80, 45);
			mesh = new BearshirtMesh(map, 2f);
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
			filter.mesh = mesh.Generate();
			GenerateColliders();
		}

		private void RemoveColliders()
		{
			EdgeCollider2D[] colliders = GetComponents<EdgeCollider2D>();
			foreach (EdgeCollider2D collider in colliders)
			{
				Destroy(collider);
			}
		}

		private void GenerateColliders()
		{
			RemoveColliders();
			List<Vector3> outline = new List<Vector3>();

			// top
			map.ForRange(0, map.width, map.height - 1, map.height, (int x, int y) => {
				float t = mesh.top + (y + 1) * mesh.size,
					l = mesh.left + x * mesh.size;

				string lt = l + "," + t;
				outline.Add(mesh.verts[mesh.indices[lt]]);
			});

			// right
			map.ForRange(map.width - 1, map.width, 0, map.height, (int x, int y) => {
				float t = mesh.top + (y + 1) * mesh.size,
					r = mesh.left + (x + 1) * mesh.size;

				string rt = r + "," + t;
				outline.Add(mesh.verts[mesh.indices[rt]]);
			});

			// bottom
			int count = outline.Count;
			map.ForRange(0, map.width, 0, 1, (int x, int y) => {
				float r = mesh.left + (x + 1) * mesh.size,
					b = mesh.top + y * mesh.size;

				string rb = r + "," + b;
				outline.Insert(count, mesh.verts[mesh.indices[rb]]);
			});

			// left
			count = outline.Count;
			map.ForRange(0, 1, 0, map.height, (int x, int y) => {
				float b = mesh.top + y * mesh.size,
					l = mesh.left + x * mesh.size;

				string lb = l + "," + b;
				outline.Insert(count, mesh.verts[mesh.indices[lb]]);
			});

			EdgeCollider2D collider = gameObject.AddComponent<EdgeCollider2D>();
			List<Vector2> points = outline.ConvertAll<Vector2>((Vector3 vert) => {
				return new Vector2(vert.x, vert.y);
			});
			collider.points = points.ToArray();
		}
	}
}
