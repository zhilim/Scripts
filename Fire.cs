using UnityEngine;
using System.Collections;

//attached to the muzzle of the SAR21
public class Fire : MonoBehaviour {
	public LayerMask WhatToHit;
	private Transform rifleButt;
	public Transform bullet;
	private PlayerControl parentScript;
	public Transform muzzleFlash;
	public int bulletForce;


	// Use this for initialization
	void Awake () {
		Debug.Log("Weapon is live.");
		rifleButt = transform.GetChild(0);
		parentScript = transform.parent.GetComponent<PlayerControl>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			//Debug.Log("Shooting.");
			Shoot();
		}
	
	}

	private void Shoot (){
		//shoot out raycast along rifling
		Vector2 buttPos = new Vector2 (rifleButt.position.x, rifleButt.position.y);
		Vector2 muzzlePos = new Vector2 (transform.position.x, transform.position.y);
		RaycastHit2D hit = Physics2D.Raycast (muzzlePos, muzzlePos-buttPos, 500, WhatToHit);
		//shoot the bullet, show the muzzle flash
		BulletTrail(muzzlePos-buttPos, hit);
		Debug.DrawLine (muzzlePos, (muzzlePos-buttPos)*500, Color.cyan);
		if (hit.collider != null && hit.rigidbody != null) {
			//do debug lines as required (turn on gizmos during play)
			Debug.DrawLine (muzzlePos, hit.point, Color.red);
			//bullet force on enemy, consider doing this using the bullet Sort script instead
			hit.rigidbody.AddForceAtPosition((muzzlePos-buttPos)*bulletForce, hit.point, ForceMode2D.Force);
		}

	}

	void BulletTrail (Vector2 direction, RaycastHit2D hit){
		//Debug.Log("BUllet at:" + transform.position);
		//Debug.Log("Player Flipped = " + parentScript.getFlipped());
		Quaternion rot = transform.rotation;

		//gun angles get all messed up here to we have to modify and set back for the bullet
		if (parentScript.getFlipped()) {
			float correctAngle = 180.0f - rot.eulerAngles.z;
			rot = Quaternion.Euler(0,0,correctAngle);
		}

		//clone a bullet, tell the bullet if its supposed to hit the enemy
		Transform bulletClone = Instantiate (bullet, transform.position, rot) as Transform;
		bulletClone.GetComponent<Sort>().setHit(hit);

		//clone the muzzle, randomise its size, destroy after awhile
		Transform flashClone = Instantiate (muzzleFlash, transform.position, rot) as Transform;
		float flashSize = Random.Range (0.1f, 0.3f);
		flashClone.localScale = new Vector3 (flashSize, flashSize, flashSize);
		Destroy (flashClone.gameObject, 0.05f);
	
	}
}
