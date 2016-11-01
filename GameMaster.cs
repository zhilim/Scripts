using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {

	private float slowMoInitatedAt, slowMoDuration;
	public bool slowMotionEnabled = false;
	private bool slowMotionNow = false;
	public static bool PlayerIsAlive = true;
	[SerializeField] public Transform Player;
	private Transform playerInstance;
	public static int PlayerScore = 0;
	[SerializeField] public Transform Warnings;
	[SerializeField] public Transform EM;
	private Animator words_bg;
	public static GameMaster gm;
	public static bool PlayerReloading = false;
	public static int PlayerHighScore = 0;
	public Transform respawnLights;
	private AudioSource audio;
	public AudioClip revive, flyby, lose, newHigh, slowDown, Speedup;
	public GameOverMenu gom;
	public static int AirCombo = 0;
	public static bool AirComboNow = false;
	public static int multiKillShot = 0;
	public static bool multiKillNow = false;
	public static bool upgradeMenuUp = false;
	public Transform[] prefabList;
	public string[] nameList = {"sar21", "beretta", "acog", "ak47", "colonial", "corpsecutter",
	 "m16", "p38", "pewpew", "bugkiller", "repulse", "ar15", "brother", "widowmaker"};

	public static int currentlyEquipped;

	public static int sar21 = 0;
	public static int beretta = 1;
	public static int acog = 2;
	public static int ak47 = 3;
	public static int colonial = 4;
	public static int corpsecutter = 5;
	public static int m16 = 6;
	public static int p38 = 7;
	public static int pewpew = 8;
	public static int bugkiller = 9;
	public static int repulse = 10;
	public static int ar15 = 11;
	public static int brother = 12;
	public static int widowmaker = 13;

	public static int credits = 0;


	// Use this for initialization
	void Awake () {
		if (PlayerPrefs.HasKey("currentlyEquipped")) {
			Debug.Log("Player has currently qeuipped and it is: " + PlayerPrefs.GetInt("currentlyEquipped"));
			GameMaster.currentlyEquipped = PlayerPrefs.GetInt("currentlyEquipped");
		} else {
			GameMaster.currentlyEquipped = 1;
			PlayerPrefs.SetInt("currentlyEquipped", GameMaster.currentlyEquipped);			
		}
		PlayerPrefs.SetInt(nameList[GameMaster.currentlyEquipped], 1);

		audio = GetComponent<AudioSource>();
		gm = GameObject.FindGameObjectWithTag("Master").GetComponent<GameMaster>();
		gm.Player = prefabList[GameMaster.currentlyEquipped];
		gm.words_bg = gm.Warnings.GetComponent<Animator>();
		Vector3 playerPos = new Vector3 (0f, 10f, 0f);
		gm.playerInstance = Instantiate (gm.Player, playerPos, Quaternion.Euler(0,0,0)) as Transform;
		gm.StartCoroutine(gm.reSpawn(playerPos));
		Instantiate (gm.EM, playerPos, Quaternion.Euler(0,0,0));
		audio.PlayOneShot(flyby, 0.5f);
		if (PlayerPrefs.HasKey("HighScore")) {
			GameMaster.PlayerHighScore = PlayerPrefs.GetInt("HighScore");
			GameMaster.PlayerHighScore = 0;
		}
		//gm.StartCoroutine(gm.showMessage("doNotMiss", 20));
		
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if (Time.time - gm.slowMoInitatedAt > gm.slowMoDuration && Time.timeScale != 1f && GameMaster.PlayerIsAlive && gm.slowMotionNow){
			GameMaster.speedUp();
		}
		
	}

	public static void goSlow (float slowMoDuration, float howSlow, float afterHowLong){
		if (gm.slowMotionEnabled) {
			if (!gm.slowMotionNow) {
				gm.StartCoroutine(gm.slowAfter(slowMoDuration, howSlow, afterHowLong));
				gm.slowMotionNow = true;
			} else {
				gm.slowMoInitatedAt = Time.time;
			}
		} 
		
	}

	public IEnumerator slowAfter (float duration, float howSlow, float delay) {
		yield return new WaitForSeconds(delay);
		if (gm.slowMotionNow) {
			gm.slowMoInitatedAt = Time.time;
			gm.slowMoDuration = duration;
			gm.audio.PlayOneShot(gm.slowDown, 0.5f);
			Time.timeScale = howSlow;
		}

	}

	public static void speedUp() { 
		if (gm.slowMotionNow) {
			gm.slowMotionNow = false;
			Time.timeScale = 1f;
			//gm.audio.PlayOneShot(gm.Speedup, 0.5f);
		}
	}

	public static void killPlayer () {
		GameMaster.PlayerIsAlive = false;
		gm.gom.gameOver();
		Destroy(gm.playerInstance.gameObject, 3f);
		if (GameMaster.PlayerScore > GameMaster.PlayerHighScore) {
			PlayerPrefs.SetInt("HighScore", GameMaster.PlayerScore);
			gm.audio.clip = gm.newHigh;
			gm.audio.PlayDelayed(1.5f);
		} else {
			gm.audio.clip = gm.lose;
			gm.audio.PlayDelayed(1.5f);
		}
		GameMaster.goSlow(5f, 0.3f, 0.3f);
		
		//gm.StartCoroutine(gm.showMessage("youDied", 0));
	}

	public IEnumerator reSpawn (Vector3 pos) {
		//gm.playerInstance.GetComponent<Rigidbody2D>().freezeRotation = true;
		gm.playerInstance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
		gm.playerInstance.GetComponent<Rigidbody2D>().gravityScale = 0f;
		Transform lights = Instantiate (respawnLights, pos, Quaternion.Euler(0,0,0)) as Transform;
		audio.PlayOneShot(revive, 1f);
		yield return new WaitForSeconds(1f);
		GameMaster.PlayerIsAlive = true;
		Destroy (lights.gameObject, 4f);
		gm.playerInstance.GetComponent<Rigidbody2D>().gravityScale = 3f;

	}


	public IEnumerator showMessage(string message, int afterHowLong) {
		yield return new WaitForSeconds(afterHowLong);
		gm.words_bg.SetBool("goingDown", true);
		yield return new WaitForSeconds(1);
		foreach (Transform warning in gm.Warnings){
			warning.gameObject.SetActive(false);
		}
		gm.Warnings.Find(message).gameObject.SetActive(true);
		gm.words_bg.SetBool("goingDown", false);
	}

	public static Transform getPlayerInstance() {
		return gm.playerInstance;
	}

	public static bool isSlow() {
		return gm.slowMotionNow;
	}

}
