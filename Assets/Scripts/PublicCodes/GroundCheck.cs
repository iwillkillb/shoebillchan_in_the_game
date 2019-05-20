using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour {

	//Ground check...
	[System.NonSerialized] public bool grounded;
	public float detectGroundDistance = 2f;
	public LayerMask whatIsGround;
	Transform groundCheck;

	void Awake () {
		groundCheck = transform.FindChild ("Ground Check");
	}

	void FixedUpdate () {
		//grounded = Physics2D.Raycast (transform.position, Vector2.down, detectGroundDistance, whatIsGround);
		grounded = Physics2D.OverlapCircle (groundCheck.position, 0.1f, whatIsGround);
	}
}
