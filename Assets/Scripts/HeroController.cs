using UnityEngine;

namespace Bearshirt
{
	public class HeroController : MonoBehaviour
	{
		[SerializeField]
		private Transform down;

		[SerializeField]
		private LayerMask groundMask;

		private Rigidbody2D body;

		private ContactPoint2D[] contacts;

		private Vector2 velocity;

		void Start()
		{
			body = GetComponent<Rigidbody2D>();
		}

		void OnDrawGizmos()
		{
			if (contacts == null) return;
			foreach (ContactPoint2D contact in contacts)
			{
				Gizmos.color = Color.green;
				Vector3 point = new Vector3(contact.point.x, contact.point.y, 0f);
				Gizmos.DrawCube(point, Vector2.one / 4);
			}

			Gizmos.color = Color.red;
			Gizmos.DrawCube(Vector3.zero + down.position, Vector2.one / 10);
		}

		void Update()
		{
			float h = Input.GetAxisRaw("Horizontal") * 5f;
			float v = Input.GetAxisRaw("Vertical") * 10f;
			velocity = new Vector2(h, v);
		}

		void FixedUpdate()
		{
			float cx = body.velocity.x;
			float cy = body.velocity.y;

			float tx = velocity.x;
			float ty = Mathf.Max(0f, velocity.y);

			float fx = body.mass * ((tx - cx) / Time.deltaTime);
			float fy = IsOnGround() && ty != 0 ?
				body.mass * ((ty - cy) / Time.deltaTime) :
				0f;

			Vector2 force = new Vector2(fx, fy);
			body.AddForce(force);
		}

		void OnCollisionEnter2D(Collision2D other) { contacts = other.contacts; }
		void OnCollisionStay2D(Collision2D other) { contacts = other.contacts; }
		void OnCollisionExit2D(Collision2D other) { contacts = other.contacts; }
		void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.gameObject.CompareTag("Door")) return;
			GlobalState.IsAtDoor = true;
		}
		void OnTriggerStay2D(Collider2D other)
		{
			if (!other.gameObject.CompareTag("Door")) return;
			GlobalState.IsAtDoor = true;
		}

		private bool IsOnGround()
		{
			float radius = transform.localScale.y;
			Collider2D hit = Physics2D.OverlapCircle(down.position, radius / 8, groundMask);
			return hit != null;
		}
	}
}
