using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameMenu : MonoBehaviour {
	

	// Use this for initialization
	void Start () {
		
		GetComponent<Animator>().SetBool("gameOver", false);
		transform.GetChild(1).GetComponent<Text>().text = "BEST: " + GameMaster.PlayerHighScore.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameMaster.PlayerIsAlive) {
			GetComponent<Animator>().SetBool("gameOver", true);
		} else {
			GetComponent<Animator>().SetBool("gameOver", false);
			if (GameMaster.PlayerScore > GameMaster.PlayerHighScore) {
				transform.GetChild(1).GetComponent<Text>().text = "NEW BEST: " + GameMaster.PlayerScore.ToString();
			}
		}

	}

}
