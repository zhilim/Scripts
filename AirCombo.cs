using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AirCombo : MonoBehaviour {

	private int currentCombo = 0;
	private Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (GameMaster.AirCombo > 2) {
			//Debug.Log("Starting AirCOMBO!");
			currentCombo = GameMaster.AirCombo;
			GetComponent<Text>().text = "AIR COMBO x" + currentCombo.ToString();
			anim.SetBool("comboOn", true);
		}

		if (GameMaster.AirCombo < currentCombo) {
			if (currentCombo > 2) {
				StartCoroutine(delayedScore(currentCombo));
			}
			currentCombo = GameMaster.AirCombo;
			anim.SetBool("comboOn", false);
		}

	}

	IEnumerator delayedScore (int score) {
		yield return new WaitForSeconds(0.8f);
		GameMaster.PlayerScore += score;
	}


}
