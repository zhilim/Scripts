using UnityEngine;
using System.Collections;

//todo: make death accurate

public class Enemy : MonoBehaviour {
	public float thrust;
	private Rigidbody2D rb;
	public Transform target, explosionAnim;
	private bool dead = false;
	public float fadePerSecond = 2.5f;
	private SpriteRenderer rd;
	private float timeOfDeath;
	private Camera cam;
	public LayerMask whatIsGround;
	public LayerMask WhatToExplode, PlayerLayer;
	private Collider2D coll;
	public float walkingSpeed;
	public float explosionRadius, explosionPower, upblast;
	private bool flipped = false;
	public AudioClip grunt1, grunt2, grunt3, grunt4, grunt5, grunt6, boom;
	private AudioSource audio;


	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource>();
		rb = GetComponent<Rigidbody2D>();
		rb.freezeRotation = true;
		rd = GetComponent<SpriteRenderer>();
		cam = Camera.main;
		coll = GetComponent<BoxCollider2D>();
		target = GameMaster.getPlayerInstance();
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Debug.Log("player is at: " + cam.WorldToScreenPoint(target.position));
		//if enemy not dead and player is alive, 
		//slowly glide towards the player
		//if in the air, dont freeze rotation, if on ground, make him walk and freeze rotation
		if(!dead && GameMaster.PlayerIsAlive){
			Vector2 targetPos = target.position;
			Vector2 myPos = transform.position;
			Vector2 diff = new Vector2 (0f,0f);
			diff.x = (targetPos.x-myPos.x)/Mathf.Abs(targetPos.x-myPos.x);
			diff.y = 0;                                                                                      
			if (!coll.IsTouchingLayers(whatIsGround)){
				//rb.freezeRotation = false;
				//rb.angularDrag = 10f;
				ForceInsideView();
				rb.AddForce(diff * thrust);
			} else {
				transform.rotation = Quaternion.Euler(0,0,0);
				foreach(Transform child in transform) {
					child.rotation = Quaternion.Euler(0,0,0);
				}
				transform.GetComponent<Animator>().SetBool("touchedDown", true);
				rb.freezeRotation = true;
				rb.AddForce(diff * walkingSpeed);
			}
			 
			if (cam.WorldToScreenPoint(transform.position).y <= 10) {
				Destroy (gameObject);
			}			
		}
	}

	void Update () {
		//if enemy is dead, fade him out
		if (dead) {
			if (Time.time - timeOfDeath > 2f){
				Color c = rd.color;
				Color newColor = new Color(c.r, c.g, c.b, c.a - (fadePerSecond * Time.deltaTime));
				rd.color = newColor;
				foreach(Transform child in transform) {
					child.GetComponent<SpriteRenderer>().color = newColor;
				}
			}
		//if enemy is not dead and if he has touched the player, explode and kill player
		}

		if(!dead && GameMaster.PlayerIsAlive) {
			if (coll.IsTouchingLayers(PlayerLayer)) {
				//Debug.Log("EXLPODING!");
				kill(false);
				explode(upblast);
				cam.transform.GetComponent<CameraShaker>().Shake(0.05f, 0.5f);
			}
			if (target != null && transform.position.x < target.position.x) {
				if (!flipped) {
					Vector3 escale = transform.localScale;
					escale.x *= -1;
					transform.localScale = escale;
					Vector3 rot = transform.eulerAngles;
					rot.z *= -1;
					transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
					flipped = true;
				}
			} else {
				if (flipped) {
					Vector3 escale = transform.localScale;
					escale.x *= -1;
					transform.localScale = escale;
					Vector3 rot = transform.eulerAngles;
					rot.z *= -1;
					transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
					flipped = false;
				}
			}

		}
	}

	//this kills the enemy
	public void kill (bool headshot) {
		if(!dead){
			timeOfDeath = Time.time;
			dead = true;
			rb.gravityScale = 3f;
			gameObject.layer = LayerMask.NameToLayer("dead");
			Destroy (gameObject, 2.3f);
			if (headshot) {
				transform.GetComponent<Animator>().SetBool("headshot", true);
			}else {
				grunt();
				transform.GetComponent<Animator>().SetBool("shot", true);
			}
		}
	}

	//force enemy into view so less awkward where they stickinig half in half out
	private void ForceInsideView(){
		Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
		if (screenPos.x < 0f || screenPos.x > (float)cam.pixelWidth || screenPos.y > (float)cam.pixelHeight){
			rb.AddForce((target.position - transform.position) * 0.5f);
		}
	}

	//explode, apply force to all within blast radius with wearoff. 
	//Also render the nice explosion and play the boom
	//tell GM to kill player also
	public void explode(float upblast){
		//target.GetComponent<Rigidbody2D>().freezeRotation = false;
		target.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
		Vector2 explosionPoint = transform.position;
		explosionPoint.y -= upblast;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPoint, explosionRadius, WhatToExplode);
		foreach (Collider2D hit in colliders) {
			Rigidbody2D r2d2 = hit.GetComponent<Rigidbody2D>();
			Vector2 hitPoint = hit.transform.position;
			Vector2 dir = hitPoint - explosionPoint;
			float wearoff = 1/(dir.magnitude+1);

			if (r2d2!=null){
				
				r2d2.AddForce(dir.normalized * explosionPower * wearoff);
			}
		}
		//Debug.Log("my explosion will hit: " + colliders[0] + colliders[1]);
		GameMaster.killPlayer();
		Transform explosion = Instantiate (explosionAnim, transform.position, transform.rotation) as Transform;
		audio.PlayOneShot(boom, 1f);
		Destroy (explosion.gameObject, 2f);

	}

	private void grunt() {
		float volume = Random.Range(0.7f, 1f);
		int grunt = Random.Range(0, 6);

		if (grunt == 1) {
			audio.PlayOneShot(grunt1, volume);
		}
		if (grunt == 2) {
			audio.PlayOneShot(grunt2, volume);
		}
		if (grunt == 3) {
			audio.PlayOneShot(grunt3, volume);
		}
		if (grunt == 4) {
			audio.PlayOneShot(grunt4, volume);
		}
		if (grunt == 5) {
			audio.PlayOneShot(grunt5, volume);
		}
		if (grunt == 6) {
			audio.PlayOneShot(grunt6, volume);
		}
	}

	public bool isDead () {
		return this.dead;
	}
}
