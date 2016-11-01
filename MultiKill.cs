using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiKill : MonoBehaviour {
	private Animator anim;
	private bool processing = false;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameMaster.multiKillShot > 0 && !processing) {
			string message = "";
			if (GameMaster.multiKillShot == 2) {
				message = "DOUBLE KILL";
			}
			if (GameMaster.multiKillShot == 3) {
				message = "TRIPLE KILL";
			}
			if (GameMaster.multiKillShot == 4) {
				message = "GENOCIDE";
			}
			if (GameMaster.multiKillShot == 5) {
				message = "SAVAGE AF";
			}

			GetComponent<Text>().text = message;
			anim.SetBool("multikilling", true);
			StartCoroutine(delayedScore(GameMaster.multiKillShot));
		} else {
			anim.SetBool("multikilling", false);
		}
	}

	IEnumerator delayedScore (int score) {
		processing = true;
		yield return new WaitForSeconds(0.5f);
		GameMaster.multiKillShot = 0;
		yield return new WaitForSeconds(0.3f);
		GameMaster.PlayerScore += score;
		processing = false;
	}
}
