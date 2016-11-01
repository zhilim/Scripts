using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountScore : MonoBehaviour {
	private Animator scoreCounter;
	[SerializeField] Text scoreCount;
	private int currentScore = 0;
	private bool scoreIncreased = false;
	private bool scoreIncreasedAgain = true;


	// Use this for initialization
	void Start () {

		scoreCounter = GetComponent<Animator>();
		StartCoroutine(startScoring());
		Debug.Log("Score text at: " + transform.GetChild(0).position);

	}
	
	// Update is called once per frame
	void Update () {

		if (GameMaster.PlayerScore > currentScore) {
			//Debug.Log("Score Increased!");
			currentScore = GameMaster.PlayerScore;
			scoreCount.text = currentScore.ToString();
			scoreIncreased = !scoreIncreased;
			scoreIncreasedAgain = !scoreIncreasedAgain;
			scoreCounter.SetBool("scoreIncreasedAgain", scoreIncreasedAgain); 
			scoreCounter.SetBool("scoreIncreased", scoreIncreased);
		}

		if(!GameMaster.PlayerIsAlive){
			gameObject.SetActive(false);
			scoreCounter.SetBool("startScore", false);
		}else {
			gameObject.SetActive(true);
		}
	}

	IEnumerator startScoring() {
		
		yield return new WaitForSeconds(3);
		gameObject.SetActive(true);
		scoreCounter.SetBool("startScore", true);
	}

}
