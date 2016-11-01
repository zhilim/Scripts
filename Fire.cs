using UnityEngine;
using System.Collections;

//attached to the muzzle of the SAR21
public class Fire : MonoBehaviour {
	public LayerMask WhatToHit;
	public LayerMask richochetHitWhat;
	private Transform rifleButt;
	public Transform bullet;
	private PlayerControl parentScript;
	public Transform muzzleFlash;
	public float bulletForce;
	public AudioClip miss, bodyshot, spray, headshotspray, reload;
	private AudioSource audio;
	public float fireDelay;
	private float lastBulletFiredAt = 0f;
	private Camera cam;
	private float grinStartTime;
	private float grinTime = 0.7f;
	public float reloadTime = 0;
	private float airComboStart;
	public int pierceStrength = 4;
	private float pierceForce;
	



	// Use this for initialization
	void Awake () {
		//Debug.Log("Weapon is live.");
		rifleButt = transform.GetChild(0);
		parentScript = transform.parent.GetComponent<PlayerControl>();
		audio = GetComponent<AudioSource>();
		cam = Camera.main;
		pierceForce = bulletForce;
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.time - lastBulletFiredAt > fireDelay) {
			GameMaster.PlayerReloading = false;
			transform.parent.GetComponent<Animator>().SetBool("reloading", false);
			if (Input.GetMouseButtonDown(0) && GameMaster.PlayerIsAlive && !GameMaster.upgradeMenuUp) {
				//Debug.Log("clicked at: " + Input.mousePosition);
				Shoot();
			}
			//Debug.Log("stopping reloading");

		}

		if (Time.time - grinStartTime > grinTime) {
			transform.parent.parent.GetComponent<Animator>().SetBool("shooting", false);
		}

		if (Time.time - airComboStart > 0.7f) {
			GameMaster.AirComboNow = false;
			GameMaster.AirCombo = 0;

		}
	
	}

	private void Shoot (){
		Grin();
		//shoot out raycast along rifling
		Vector2 buttPos = new Vector2 (rifleButt.position.x, rifleButt.position.y);
		Vector2 muzzlePos = new Vector2 (transform.position.x, transform.position.y);
		RaycastHit2D hit = Physics2D.Raycast (muzzlePos, muzzlePos-buttPos, 500, WhatToHit);
		RaycastHit2D[] hits = Physics2D.RaycastAll (muzzlePos, muzzlePos-buttPos, 500, richochetHitWhat);
		if (hits.Length > 1) {
			GameMaster.multiKillShot = hits.Length;
		}
		//shoot the bullet, show the muzzle flash
		BulletTrail(muzzlePos-buttPos, hit, true);
		Debug.DrawLine (muzzlePos, (muzzlePos-buttPos)*500, Color.cyan);
		if (hit.collider != null && hit.rigidbody != null) {
			//do debug lines as required (turn on gizmos during play)
			audio.PlayOneShot(bodyshot, 0.7f);
			Debug.DrawLine (muzzlePos, hit.point, Color.red);
			//bullet force on enemy, unfreeze the enemy so he can fly
			hit.rigidbody.freezeRotation = false;
			hit.rigidbody.angularDrag = 1f;
			hit.rigidbody.AddForceAtPosition((muzzlePos-buttPos)*bulletForce, hit.point, ForceMode2D.Force);
			//play hit audio
			
			//kill the enemy
			bool headshot = false;
			if (hit.collider.isTrigger) {
				headshot = true;
				
				if (!hit.transform.GetComponent<Enemy>().isDead()) {
					audio.PlayOneShot(headshotspray, 0.7f);
					GameMaster.goSlow(0.5f, 0.3f, 0.5f);
				} else {
					audio.PlayOneShot(spray, 0.3f);
				}

			}else {
				audio.PlayOneShot(spray, 0.3f);
			}

			if (!hit.transform.GetComponent<Enemy>().isDead()) {
				GameMaster.PlayerScore += 1;
				hit.transform.GetComponent<Enemy>().kill(headshot);
				cam.transform.GetComponent<CameraShaker>().Shake(0.03f, 0.2f);
				//Debug.Log("hit: " + hit.transform.position.y + ", soldier: " + transform.parent.parent.position.y);
				if (hit.transform.position.y > transform.parent.parent.position.y) {
					//Debug.Log("Adding Air Combo Score");
					//GameMaster.PlayerScore += 1;
					if (GameMaster.AirComboNow) {
						//Debug.Log("extending Air Combo time");
						airComboStart = Time.time;
						GameMaster.AirCombo += 1;
					} else if (!GameMaster.AirComboNow) {
						//Debug.Log("Starting aircombo time");
						GameMaster.AirComboNow = true;
						airComboStart = Time.time;
						GameMaster.AirCombo += 1;
					}
				} else {
					//Debug.Log("cancellling aircombo..");
					GameMaster.AirComboNow = false;
					GameMaster.AirCombo = 1;
				}
			} else {
				if (hit.transform.position.y > transform.parent.parent.position.y) {
					//Debug.Log("Adding Air Combo Score");

					GameMaster.PlayerScore += 1;
					if (GameMaster.AirComboNow) {
						//Debug.Log("extending Air Combo time");
						airComboStart = Time.time;
						GameMaster.AirCombo += 1;
					} else if (!GameMaster.AirComboNow) {
						//Debug.Log("Starting aircombo time");
						GameMaster.AirComboNow = true;
						airComboStart = Time.time;
						GameMaster.AirCombo += 1;
					}
				} else {
					//Debug.Log("cancellling aircombo..");
					GameMaster.AirComboNow = false;
					GameMaster.AirCombo = 1;
				}
			}
			//Debug.Log("GM aircombo records show: " + GameMaster.AirCombo.ToString());
			//free fire
			fireDelay = 0f;

			if (pierceStrength > 1) {
				pierce(1, muzzlePos-buttPos, hit.point);
			}
			
		}else{
			//if miss, slow mo over, player reloads
			GameMaster.speedUp();
			audio.PlayOneShot(miss, 0.7f);
			fireDelay = reloadTime;
			audio.PlayOneShot(reload, 0.7f);
			//Debug.Log("Setting gamemaster player reloading");
			GameMaster.PlayerReloading = true;
			transform.parent.GetComponent<Animator>().SetBool("reloading", true);

		}
		//for firedelay
		lastBulletFiredAt = Time.time;

	}

	private void pierce (float pierceCount, Vector2 direction, Vector2 piercePoint) {
		if (pierceCount >= pierceStrength) {
			//Debug.Log("not piercing");
			pierceForce = bulletForce;
			return;
		}
		//Debug.Log("piercing");
		pierceForce *= 0.5f;
		RaycastHit2D hit = Physics2D.Raycast (piercePoint, direction, 500, richochetHitWhat);
		BulletTrail(direction, hit, false);
		if (hit.collider != null && hit.rigidbody != null) {

			//bullet force on enemy, unfreeze the enemy so he can fly
			hit.rigidbody.freezeRotation = false;
			hit.rigidbody.angularDrag = 1f;
			hit.rigidbody.AddForceAtPosition((direction)*pierceForce, hit.point, ForceMode2D.Force);
		
			//kill the enemy
			bool headshot = false;
			if (hit.collider.isTrigger) {
				headshot = true;
				
				if (!hit.transform.GetComponent<Enemy>().isDead()) {
					audio.PlayOneShot(headshotspray, 0.7f);
					GameMaster.goSlow(0.5f, 0.3f, 0.5f);
				} else {
					audio.PlayOneShot(spray, 0.3f);
				}

			}else {
				audio.PlayOneShot(spray, 0.3f);
			}

			if (!hit.transform.GetComponent<Enemy>().isDead()) {
				GameMaster.PlayerScore += 1;
				hit.transform.GetComponent<Enemy>().kill(headshot);
				if (hit.transform.position.y > transform.parent.parent.position.y) {
					//GameMaster.PlayerScore += 1;
					if (GameMaster.AirComboNow) {
						airComboStart = Time.time;
						GameMaster.AirCombo += 1;
					} else if (!GameMaster.AirComboNow) {
						GameMaster.AirComboNow = true;
						airComboStart = Time.time;
						GameMaster.AirCombo += 1;
					}
				} else {
					GameMaster.AirComboNow = false;
					GameMaster.AirCombo = 1;
				}
			} else {
				if (hit.transform.position.y > transform.parent.parent.position.y) {

					GameMaster.PlayerScore += 1;
					if (GameMaster.AirComboNow) {
						airComboStart = Time.time;
						GameMaster.AirCombo += 1;
					} else if (!GameMaster.AirComboNow) {
						GameMaster.AirComboNow = true;
						airComboStart = Time.time;
						GameMaster.AirCombo += 1;
					}
				} else {
					GameMaster.AirComboNow = false;
					GameMaster.AirCombo = 1;
				}
			}
			pierce(pierceCount+1, direction, hit.point);
		}

		

	}

	private void BulletTrail (Vector2 direction, RaycastHit2D hit, bool flash){
		//Debug.Log("BUllet at:" + transform.position);
		//Debug.Log("Player Flipped = " + parentScript.getFlipped());
		Quaternion rot = transform.rotation;

		//gun angles get all messed up here to we have to modify and set back for the bullet
		if (parentScript.getFlipped()) {
			float correctAngle = 180.0f - rot.eulerAngles.z;
			rot = Quaternion.Euler(0,0,-correctAngle);
		}else {
			rot = Quaternion.Euler(0,0,-rot.eulerAngles.z);
		}

		//clone a bullet, tell the bullet if its supposed to hit the enemy
		Transform bulletClone = Instantiate (bullet, transform.position, rot) as Transform;
		bulletClone.GetComponent<Sort>().setHit(hit);

		if (flash) {
		//clone the muzzle, randomise its size, destroy after awhile
			Transform flashClone = Instantiate (muzzleFlash, transform.position, rot) as Transform;
			float flashSize = Random.Range (0.1f, 0.3f);
			flashClone.localScale = new Vector3 (flashSize, flashSize, flashSize);
			Destroy (flashClone.gameObject, 0.05f);
		}
	
	}

	private void Grin () {
		transform.parent.parent.GetComponent<Animator>().SetBool("shooting", true);
		grinStartTime = Time.time;
	}


}
