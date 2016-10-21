using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

	public float enemySpawnDelay;
	private float lastSpawnTime;
	public Transform enemy;
	private Camera cam;
	public Transform spawnPoint;



	// Use this for initialization
	void Start () {
		lastSpawnTime = Time.time;
		cam = Camera.main;
		
	}
	
	//spawn enemy randomly from top of camera
	void Update () {

		if (Time.time - lastSpawnTime > enemySpawnDelay) {
			Vector3 screenSpawnPoint = cam.WorldToScreenPoint(spawnPoint.transform.position);
			screenSpawnPoint.x = Random.Range(0.0f, (float)cam.pixelWidth);
			Vector3 spawnPointRandom = cam.ScreenToWorldPoint(screenSpawnPoint);
			Instantiate (enemy, spawnPointRandom, spawnPoint.rotation);
			lastSpawnTime = Time.time;
		}

	}
}
