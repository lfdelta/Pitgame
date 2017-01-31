using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMove : MonoBehaviour
{
	public float speed = 5f;
	public float grav = 3f;

	private bool hit;
	private Transform trans;
	private Vector3 pos;
	private Rigidbody2D rb2d;



	void Start ()
	{
		hit = false;
		trans = GetComponent<Transform> ();
		rb2d = GetComponent<Rigidbody2D> ();
	}



	void FixedUpdate ()
	{
		if (!hit) {
			pos = trans.position;
			Vector3 move = new Vector3 (-pos.x, 0, 0); // sinusoidal horizontal force
			rb2d.AddForce (move * speed);
		}
	}



	void OnCollisionEnter2D(Collision2D collision)
	{
		//Debug.Log(collision.gameObject.tag);

		//if (collision.gameObject.tag == "Weapon") {
			Hit ();
		//}
	}



	public void Hit ()
	{
		Debug.Log("Hit!");

		hit = true;
		rb2d.gravityScale = grav;
		rb2d.sharedMaterial = null;
	}
}
