using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Armory : MonoBehaviour {
	public GameObject goMenu;
	private float timeScaleOriginal;
	public string[] itemNames;
	public int[] prices;
	public int currentWeapon;
	private Animator anim;


	// Use this for initialization
	void Start () {
		anim = transform.GetChild(1).GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void openArmory() {
		timeScaleOriginal = Time.timeScale;
		Time.timeScale = 0f;
		goMenu.SetActive(false);
		gameObject.SetActive(true);

		populateStats(GameMaster.currentlyEquipped);
		transform.GetChild(3).GetComponent<Text>().text = "CREDITS: " + GameMaster.credits;
	}

	public void closeArmory() {
		Time.timeScale = timeScaleOriginal;
		goMenu.SetActive(true);
		gameObject.SetActive(false);
		
	}

	public void nextWeapon() {
		StartCoroutine(right());
	}

	public void previousWeapon() {
		StartCoroutine(left());
	}

	private void populateStats(int index) {
		currentWeapon = index;
		transform.GetChild(1).GetChild(index).gameObject.SetActive(true);
		string price = prices[index].ToString();
		string name = itemNames[index];
		if (PlayerPrefs.HasKey(GameMaster.gm.nameList[index])){
			price = "Owned";
			if (index != GameMaster.currentlyEquipped) {
				//equip button
				transform.GetChild(13).gameObject.SetActive(true);
			} else {
				transform.GetChild(13).gameObject.SetActive(false);
				name += " (EQUIPPED)";
			}
			//buy button
			transform.GetChild(9).gameObject.SetActive(false);
		}else{
			transform.GetChild(9).gameObject.SetActive(true);
			transform.GetChild(9).GetChild(0).GetComponent<Text>().text = "BUY (" + price + ")"; 
		}
		transform.GetChild(4).GetComponent<Text>().text = name;
		transform.GetChild(8).GetComponent<Text>().text = price;

		Fire pfab = GameMaster.gm.prefabList[index].GetChild(3).GetChild(2).GetComponent<Fire>();
		transform.GetChild(5).GetComponent<Text>().text = "FIRE POWER: " + pfab.bulletForce.ToString();
		transform.GetChild(6).GetComponent<Text>().text = "PIERCING STRENGTH: " + pfab.pierceStrength.ToString();
		transform.GetChild(7).GetComponent<Text>().text = "RELOAD TIME: " + pfab.reloadTime.ToString();

		if (index >= itemNames.Length-1) {
			//right button
			transform.GetChild(11).gameObject.SetActive(false);
		} else {
			transform.GetChild(11).gameObject.SetActive(true);
		}

		if (index <= 0) {
			//left button
			transform.GetChild(12).gameObject.SetActive(false);
		} else {
			transform.GetChild(12).gameObject.SetActive(true);
		}

	}

	private IEnumerator right() {
		transform.GetChild(11).GetComponent<Button>().interactable = false;
		anim.SetBool("goingAway", true);
		Time.timeScale = 1f;
		yield return new WaitForSeconds(0.2f);
		Time.timeScale = 0f;
		transform.GetChild(1).GetChild(currentWeapon).gameObject.SetActive(false);
		populateStats(currentWeapon+1);
		anim.SetBool("goingAway", false);
		Time.timeScale = 1f;
		yield return new WaitForSeconds(0.2f);
		Time.timeScale = 0f;
		transform.GetChild(11).GetComponent<Button>().interactable = true;
		Debug.Log("current Weapon index: " + currentWeapon);
		
	}

	private IEnumerator left() {
		transform.GetChild(12).GetComponent<Button>().interactable = false;
		anim.SetBool("goingAway", true);
		Time.timeScale = 1f;
		yield return new WaitForSeconds(0.2f);
		Time.timeScale = 0f;
		transform.GetChild(1).GetChild(currentWeapon).gameObject.SetActive(false);
		populateStats(currentWeapon-1);
		anim.SetBool("goingAway", false);
		Time.timeScale = 1f;
		yield return new WaitForSeconds(0.2f);
		Time.timeScale = 0f;
		transform.GetChild(12).GetComponent<Button>().interactable = true;
		Debug.Log("current Weapon index: " + currentWeapon);
		
	}

	public void Buy(){

	}


}
