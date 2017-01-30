using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{	
	public float speed;
	public float jump;

	public Transform groundCollider;
	public LayerMask groundLayer;
	private bool grounded = false;
	private float groundRad = 0.1f;

	private Rigidbody2D rb2d;



	void Start()
	{
		rb2d = GetComponent<Rigidbody2D> ();
	}



	void Update()
	{
		if (grounded) {
			rb2d.gravityScale = 1f;
			if (Input.GetButtonDown ("Jump")) {
				rb2d.velocity = new Vector2 (rb2d.velocity.x, jump);
			}
		}
	}



	void FixedUpdate()
	{
		grounded = Physics2D.OverlapCircle (groundCollider.position, groundRad, groundLayer);
		float move_h = Input.GetAxis ("Horizontal");

		rb2d.velocity = new Vector2(move_h * speed, rb2d.velocity.y);
		if (rb2d.velocity.y < 0)
			rb2d.gravityScale = 3f;
	}
}


// https://unity3d.com/learn/tutorials/topics/2d-game-creation/2d-character-controllers?playlist=17120