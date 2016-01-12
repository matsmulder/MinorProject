using UnityEngine;
using System.Collections;

public class MusicObject : MonoBehaviour {
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
}