using UnityEngine;
using System.Collections;

//script is actually attached to the SAR 21
public class PlayerControl : MonoBehaviour {

	private Camera cam;
	private Vector3 mousePos, gunPosScreen;
	private float angle;
	private Transform player;
	private bool playerFlipped;


	// Use this for initialization
	void Start () {
		//grab the main camera
		cam = Camera.main;
		//Debug.Log("Camera Width: " + cam.pixelWidth);
		//Debug.Log("Camera Height: " + cam.pixelHeight);
		//grab the player
		player = transform.parent;
		//note that player facing right
		playerFlipped = false;
	}
	
	// Update is called once per frame
	void Update () {

		//trigo the angle 
		mousePos = Input.mousePosition;
		gunPosScreen = cam.WorldToScreenPoint(transform.position);
		float x = mousePos.x - gunPosScreen.x;
		float y = mousePos.y - gunPosScreen.y;
		angle = Mathf.Atan2(y,x) * Mathf.Rad2Deg;
		//Debug.Log("angle: " + angle);

		//flip player as required
		if (!playerFlipped) {
			if (angle > 90.0f || angle < -90.0f) {	
				flip();
			}
		}
		else {
			if (angle < 90.0f && angle >= 0) {
				flip();
			}
			if (angle > -90.0f && angle < 0) {
				flip();
			}
		}
		
		//modify angle for each quadrant
		if (playerFlipped) {
			angle = 180.0f-angle;
		}

		//rotate the gun
		transform.rotation = Quaternion.Euler(0,0,angle);
		
		if (Input.GetMouseButtonDown(0)) {
			//Debug.Log("Left Clicked at: " + Input.mousePosition);
			//Debug.Log("Gun centered at: " + cam.WorldToScreenPoint(transform.position));
		}
	}

	private void flip(){
		
		Vector3 pscale = player.localScale;
		pscale.x *= -1;
		player.localScale = pscale;
		playerFlipped = !playerFlipped;
	}

	public bool getFlipped(){
		return playerFlipped;
	}
}
