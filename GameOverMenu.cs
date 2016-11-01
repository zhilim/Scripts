using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour {


	// Use this for initialization
	void Start () {	
		GetComponent<Animator>().SetBool("gameOver", false);
		transform.GetChild(2).gameObject.SetActive(false);
		transform.GetChild(3).gameObject.SetActive(false);
		transform.GetChild(4).gameObject.SetActive(false);
		transform.GetChild(5).gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TryAgain () {
		GameMaster.PlayerIsAlive = true;
		GameMaster.PlayerScore = 0;
		Time.timeScale = 1f;
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}

	public void gameOver() {
		transform.GetChild(2).gameObject.SetActive(true);
		transform.GetChild(3).gameObject.SetActive(true);
		transform.GetChild(4).gameObject.SetActive(true);
		if (GameMaster.PlayerScore > GameMaster.PlayerHighScore) {
			transform.GetChild(5).gameObject.SetActive(true);
			transform.GetChild(5).GetComponent<Text>().text = "New Best: " + GameMaster.PlayerScore.ToString();
		}
		GetComponent<Animator>().SetBool("gameOver", true);
	}


}
