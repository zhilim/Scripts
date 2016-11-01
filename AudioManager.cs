using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public static AudioManager am;
	private AudioSource audioPlayer;
	public AudioClip shot, bloodspray, headShotSpray, revive, explosion, land, hurt, gameOver, newBest, flyby, reload, slowDown, speedUp;


	// Use this for initialization
	void Start () {
		am.audioPlayer = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void Play(AudioClip clip, float delay, float volume) {
		am.audioPlayer.volume = volume;
		am.audioPlayer.clip = clip;
		am.audioPlayer.PlayDelayed(delay);
	}
}
