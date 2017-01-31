using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
	public Transform grappleOrigin;
	public float reach;
	public float pullForce;
	public LayerMask grappleLayer;
	public LayerMask raycastLayer;

	public GameObject grappleRender;

	private Collider2D grappledObj;
	private Transform grappleTrans;
	private GameObject grappleParent;
	private Rigidbody2D grappleRb2d;
	private float grappleMass;
	private bool touchingGrappled;

	public Transform arm;
	private float armRotOffset;
	private Vector3 mouseV3;
	private Vector2 mouseV2;
	private Vector2 gorigV2;

	public LineRenderer grappleRange;

	private Transform trans;
	private Rigidbody2D rb2d;



	void Start()
	{
		trans = GetComponent<Transform> ();
		rb2d = GetComponent<Rigidbody2D> ();
		armRotOffset = Mathf.Atan2 (grappleOrigin.position.y - arm.position.y,
			grappleOrigin.position.x - arm.position.x);

		//
		grappleRange.enabled = false;
	}



	void Update()
	{
		mouseV3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseV2 = new Vector2(mouseV3.x, mouseV3.y);
		gorigV2 = new Vector2 (grappleOrigin.position.x, grappleOrigin.position.y);

		// rotate the player and arm to track the mouse or grappled object
		if (grappledObj != null) {
			pointArmTo (new Vector2(grappleTrans.position.x, grappleTrans.position.y));
		} else {
			pointArmTo (mouseV2);
		}

		// hold mouse to grapple
		if (Input.GetMouseButton(0)) {
			if (grappledObj != null) {
				grappleRender = (GameObject) Instantiate(grappleRender, transform.position, transform.rotation) as GameObject;
				//grappleLine.enabled = true;
				Pull ();
			} else {
				attemptGrapple ();
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			grappledObj = null;
			//grappleLine.enabled = false;
			grappleRange.enabled = false;
			touchingGrappled = false;
		}
	}



	void pointArmTo(Vector2 pointAt) {
		Vector3 playerScale = trans.localScale;
		playerScale.x = Mathf.Sign(pointAt.x - gorigV2.x) * Mathf.Abs(playerScale.x);
		trans.localScale = playerScale;

		float dir = Mathf.Sign (playerScale.x);
		float theta = dir * Mathf.Atan2 (pointAt.y - arm.position.y, dir * (pointAt.x - arm.position.x));
		float totalRot = (theta - (dir * armRotOffset)) * Mathf.Rad2Deg;
		arm.rotation = Quaternion.Euler (new Vector3 (0, 0, totalRot));
	}



	void attemptGrapple() {
		Vector2 grappleV2 = mouseV2 - gorigV2;

		// pulls object if it's within reach and there is nothing in the way
		if (grappleV2.magnitude < reach) {
			RaycastHit2D hitObj = Physics2D.Raycast (mouseV2, Vector2.zero, 0f, grappleLayer);
			RaycastHit2D pathObj = Physics2D.Raycast (gorigV2, grappleV2, reach, raycastLayer);
			if (hitObj.collider != null && hitObj.collider == pathObj.collider) {
				// initialize variables
				grappledObj = hitObj.collider;
				grappleTrans = grappledObj.transform;
				grappleParent = grappledObj.gameObject.transform.parent.gameObject;
				grappleRb2d = grappleParent.GetComponent<Rigidbody2D> ();
				grappleMass = grappleParent.GetComponent<PullMass> ().mass;

				Pull ();
			}
		} else {
			grappleRange.enabled = true;
			refreshCircle ();
		}
	}



	void Pull()
	{
		// vector between player and grappled object
		Vector2 rVec = new Vector2 (grappleTrans.position.x - grappleOrigin.position.x,
									grappleTrans.position.y - grappleOrigin.position.y);
		rVec.Normalize ();

		// apply forces
		if (!touchingGrappled) {
			rb2d.AddForce (rVec * pullForce);
			grappleRb2d.AddForce (-rVec * pullForce / grappleMass);
			Debug.Log (rVec.magnitude);
		}
			
		// hit object
		if (grappleParent.tag == "CanBeHit") {
			grappleParent.SendMessage ("Hit");
		}

		// draw the grapple line
		//grappleLine.SetPositions(new Vector3[] {grappleOrigin.position, grappleTrans.position});
	}



	void OnCollisionEnter2D(Collision2D col) {
		Debug.Log (col.collider.name + " " + touchingGrappled);
	}



	void refreshCircle() {
		Vector3 center = grappleOrigin.position;
		// grappleRange.numPositions = some function of reach
		float dTheta = 2 * Mathf.PI / (grappleRange.numPositions - 1);

		for (int i = 0; i < grappleRange.numPositions; i++) {
			Vector3 ipos = new Vector3 (reach * Mathf.Cos (i * dTheta), reach * Mathf.Sin (i * dTheta), 1);
			grappleRange.SetPosition (i, center + ipos);
		}
	}
}

// make sure that if the path is interrupted while grappling, something happens
// if the grappled object is touching the player, grappling should not move the system
// why is momentum only maintained in vertical direction after releasing grapple?