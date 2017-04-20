using UnityEngine;

namespace Bearshirt
{
	public class HeroController : MonoBehaviour
	{
		[SerializeField] private Transform down;
		[SerializeField] private LayerMask groundMask;
		[SerializeField] private SpriteRenderer block;

		private Rigidbody2D body;
		private ContactPoint2D[] contacts;
		public Vector2 velocity { get; private set; }

		[HideInInspector] public bool hasBlock {
			get { return block.enabled; }
			set { block.enabled = value; }
		}

		void Start()
		{
			body = GetComponent<Rigidbody2D>();
			hasBlock = false;
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
			if (other.gameObject.CompareTag("Door")) GlobalState.IsAtDoor = true;
			if (other.gameObject.CompareTag("Lava")) GlobalState.IsInLava = true;
		}
		void OnTriggerStay2D(Collider2D other)
		{
			if (other.gameObject.CompareTag("Door")) GlobalState.IsAtDoor = true;
			if (other.gameObject.CompareTag("Lava")) GlobalState.IsInLava = true;
		}

		private bool IsOnGround()
		{
			float radius = transform.localScale.y;
			Collider2D hit = Physics2D.OverlapCircle(down.position, radius / 8, groundMask);
			return hit != null;
		}
	}
}
