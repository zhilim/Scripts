using UnityEngine;
using System.Collections;

//attached to the bullet
public class Sort : MonoBehaviour {

	private Renderer MyRenderer;
	public string SortingLayer = "weapon";
	public int SortingOrder = 10;
	public int moveSpeed = 100;
	private RaycastHit2D hit;
	public int hitArea;
	public Transform BloodSpray;
	

	// Use this for initialization
	void Start () {
		//Debug.Log(gameObject.name);
		MyRenderer = GetComponent<Renderer>();
		MyRenderer.sortingLayerName = SortingLayer;
		MyRenderer.sortingOrder = SortingOrder;
		Debug.Log("Bullet Angle rotation: " + transform.rotation.eulerAngles);
	}
	
	// If bullet hits enemy, destroy bullet at point of impact and bleed the enemy
	void Update () {
		if (hit.collider == null) {
			transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
		} else {
			Vector2 trailPos = transform.position;
			Vector2 distance = hit.point - trailPos;
			if (distance.x > hitArea || distance.x < -hitArea){
				transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
			}else{
				Bleed();
				Destroy(gameObject);
			}
		}
		
		Destroy (gameObject, 0.5f);
	}

	public void setHit (RaycastHit2D hit) {
		this.hit = hit;
		
	}

	//bleed function should be placed on enemy prefab in the future.
	private void Bleed () {
		if (hit.collider != null) {
			Quaternion rot = Quaternion.Euler(-transform.rotation.eulerAngles.z, 90, 0);	
			Transform blood = Instantiate (BloodSpray, hit.transform.position, rot) as Transform;
			Destroy (blood.gameObject, 5f);
			
		}
	}
}
