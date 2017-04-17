using UnityEngine;

namespace Bearshirt
{
	public class BearshirtLoop : MonoBehaviour
	{
		private MeshFilter filter;

		private BearshirtMesh mesh;

		void Start()
		{
			Debug.Log("BearshirtLoop");

			filter = GetComponent<MeshFilter>();
			mesh = new BearshirtMesh(80, 45);

			filter.mesh = mesh.Generate(2f);
		}

		void Update()
		{
			if (Input.GetMouseButtonUp(0))
			{
				filter.mesh = mesh.Generate(2f);
			}
		}
	}
}
