using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

	public float enemySpawnDelay;
	private float lastSpawnTime;
	public Transform enemy;
	private Camera cam;
	private int top = 0;
	private int left = 1;
	private int right = 2;
	private int waveCounter;
	private float gravity = 0.3f;
	private float thrust = 0.3f;
	private float walkingSpeed = 7f;



	// Use this for initialization
	void Start () {
		lastSpawnTime = Time.time;
		cam = Camera.main;
		waveCounter = 0;
		enemySpawnDelay = 3;
		
	}
	
	//spawn enemy randomly from top of camera
	void Update () {


		if (Time.time - lastSpawnTime > enemySpawnDelay) {
			waveCounter += 1;
			//Debug.Log("Wave " + waveCounter.ToString() + " Incoming.");
			//Debug.Log("EnemyRespawn time this round: " + enemySpawnDelay.ToString());
			//Debug.Log("gravity now: " + gravity.ToString()); 
			for(int i = 0; i < Random.Range(1,2); i++){
				StartCoroutine(SpawnProcedural(0.5f, 3, top)); 
			}
			for(int i = 0; i < Random.Range(1,1); i++){
				StartCoroutine(SpawnProcedural(0.5f, 2, left));  
			}
			for(int i = 0; i < Random.Range(1,1); i++){
				StartCoroutine(SpawnProcedural(0.5f, 2, right));  
			}
			lastSpawnTime = Time.time;
			if (enemySpawnDelay >= 1.2f) {
				enemySpawnDelay -= 0.05f;
			}
			if (gravity < 3f) {
				gravity += 0.05f;
			} 
			if (thrust < 15f) {
				thrust += 0.27f;
			}
			if (walkingSpeed < 21f) {
				walkingSpeed += 0.27f;
			} 
			
		}

	}

	private void Spawn(int where){
		Vector3 pos = new Vector3 (0,0,0);
		Vector3 screenSpawnPoint = cam.WorldToScreenPoint(pos);
		if (where == top){
			screenSpawnPoint.y = (float)cam.pixelHeight + 50f; 
			screenSpawnPoint.x = Random.Range(0.0f, (float)cam.pixelWidth);
			
		}
		if (where == left){
			screenSpawnPoint.x = -50f;
			screenSpawnPoint.y = Random.Range(0f, (float)cam.pixelHeight);
		}
		if (where == right){
			screenSpawnPoint.x = (float)cam.pixelWidth + 50f;
			screenSpawnPoint.y = Random.Range(0f, (float)cam.pixelHeight);
		}

		Vector3 spawnPointRandom = cam.ScreenToWorldPoint(screenSpawnPoint);
		Transform en = Instantiate (enemy, spawnPointRandom, Quaternion.Euler(0,0,Random.Range(-15f, 89f))) as Transform;
		en.SetParent(GameObject.Find("Enemies").transform);
		en.GetComponent<Rigidbody2D>().gravityScale = gravity;
		en.GetComponent<Enemy>().thrust = thrust;
		en.GetComponent<Enemy>().walkingSpeed = walkingSpeed;
	}

	private IEnumerator SpawnProcedural(float tacticalDistance, int maxWaveSize, int where) {
		
		for(int i = 0; i < Random.Range(1,maxWaveSize); i++){
			Spawn(where);
			yield return new WaitForSeconds(tacticalDistance); 
		}
	}
}
