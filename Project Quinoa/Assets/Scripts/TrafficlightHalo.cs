using UnityEngine;
using System.Collections;

public class TrafficlightHalo : MonoBehaviour {
	float timeLeft;
	
	
	// Use this for initialization
	void Start () {
		timeLeft = 25.0f;
		Behaviour h = (Behaviour)GetComponent("Halo");
		h.enabled = false;
	}
	
	void Update()
	{
		timeLeft -= Time.deltaTime;
		
		if (timeLeft < 0.0f) {
			timeLeft = 25.0f;
		};
		if (timeLeft > 15.0f) {
			greenon ();
		};
		if (timeLeft > 10.0f && timeLeft < 15.0f) {
			orangeon();
			
		};
		if (timeLeft < 10.0f) {
			redon();
		};
		
	}
	
	void greenon(){
		Behaviour h = (Behaviour)GetComponent("Halo");
		if (gameObject.transform.name.Equals ("Cone_groen")) {
			h.enabled = true;
		}
		;
		if (gameObject.transform.name.Equals ("Cone_geel")) {
			h.enabled = false;
		}
		;
		if (gameObject.transform.name.Equals ("Cone_rood")) {
			h.enabled = false;
		}
	}
	
	void orangeon(){
		Behaviour h = (Behaviour)GetComponent("Halo");
		if (gameObject.transform.name.Equals ("Cone_groen")) {
			h.enabled = false;
		}
		;
		if (gameObject.transform.name.Equals ("Cone_geel")) {
			h.enabled = true;
		}
		;
		if (gameObject.transform.name.Equals ("Cone_rood")) {
			h.enabled = false;
		}
	}
	
	void redon(){
		Behaviour h = (Behaviour)GetComponent("Halo");
		if (gameObject.transform.name.Equals ("Cone_groen")) {
			h.enabled = false;
		}
		;
		if (gameObject.transform.name.Equals ("Cone_geel")) {
			h.enabled = false;
		}
		;
		if (gameObject.transform.name.Equals ("Cone_rood")) {
			h.enabled = true;
		}
	}
	
}
