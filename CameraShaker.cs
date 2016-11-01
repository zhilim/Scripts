using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour {

	private Camera cam;
	private float intensity;
	private Vector3 camPosActual;
	private Vector3 bgPosActual;
	public float paraScale = 0.5f;
	public float paraScaleFar = 0.8f;
	public Transform background;
	public Transform words_bg;


	// Use this for initialization
	void Awake () {
		cam = Camera.main;
		camPosActual = cam.transform.position;
		bgPosActual = background.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.T)) {
			Shake (0.1f, 0.2f);
		}
	}

	public void Shake(float intensity, float length) {
		if (!GameMaster.isSlow()) {
			this.intensity = intensity;
			InvokeRepeating("StartShake", 0, 0.01f);
			Invoke("StopShake", length);
		}
	}

	void StartShake() {
		if (intensity > 0) {
			Vector3 camPos = cam.transform.position;
			Vector3 bgPos = background.transform.position;
			Vector3 wordsPos = words_bg.transform.position;
			float offsetX = Random.value * intensity * 2 - intensity;
			float offsetY = Random.value * intensity * 2 - intensity;
			camPos.x += offsetX;
			camPos.y += offsetY;


			bgPos.x += offsetX * paraScale;
			bgPos.y += offsetY * paraScale;

			wordsPos.x += offsetX * paraScaleFar;
			wordsPos.y += offsetY * paraScaleFar;


			cam.transform.position = camPos;
			background.transform.position = bgPos;
			words_bg.transform.position = wordsPos;
		}
	}

	void StopShake() {
		CancelInvoke("StartShake");
		cam.transform.position = camPosActual;
		background.transform.position = bgPosActual;
	}
}
