using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeCanvas : MonoBehaviour {
	public Scrollbar Holder;
	public float time = 100;
	RandomMatchmaker hoi;
	int time1;

	void Start(){
		hoi = GameObject.FindGameObjectWithTag("scripts").GetComponent<RandomMatchmaker>();
	}

	void Update(){
		time1 = (hoi.getTime ())/10;
		Holder.size = time1;
	}
}
