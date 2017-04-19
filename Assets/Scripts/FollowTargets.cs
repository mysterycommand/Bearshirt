using System.Collections.Generic;
using UnityEngine;

namespace Bearshirt
{
	public class FollowTargets : MonoBehaviour
	{
		[HideInInspector]
		public List<Transform> targets = new List<Transform>();

		void FixedUpdate()
		{
			Vector3 currentPosition = transform.position;
			Vector3 targetPosition = GetTargetPosition();

			transform.position = Vector3.Lerp(currentPosition, targetPosition, 0.1f);
		}

		private Vector3 GetTargetPosition()
		{
			Vector3 targetPosition = Vector3.zero;

			targets.ForEach((Transform t) => {
				targetPosition += t.position;
			});
			targetPosition /= targets.Count;

			targetPosition = new Vector3(
				targetPosition.x,
				targetPosition.y,
				transform.position.z
			);

			return targetPosition;
		}
	}
}
