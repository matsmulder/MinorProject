using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicOrder : MonoBehaviour {
	public AudioClip[] soundtrack;
	AudioSource audio2 = new AudioSource();
	
	void Start() {
		audio2 = gameObject.AddComponent<AudioSource>();
		audio2.clip = soundtrack [0];
		audio2.Play ();
	}
	
	void Update(){
		if (!audio2.isPlaying) {
			if(Application.loadedLevelName.Equals("Quinoa")){
				audio2.clip = soundtrack [2];
			}
			else{
				audio2.clip = soundtrack [1];
			};
			audio2.Play ();
		}
		
	}
	
	
}
