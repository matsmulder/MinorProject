using UnityEngine;
using System.Collections;

public class SpawenItmes : MonoBehaviour {

	public Transform[] SpawnPoints;
	public GameObject[] objects;


	// Use this for initialization
	void Start () {
		int i = 0;
		int n = Random.Range (15, 35);
		while (i < n) {
			int ObjectIndex = Random.Range (0, objects.Length);
			int SpawnIndex = Random.Range(0, SpawnPoints.Length);
			SpawnPoints[SpawnIndex].transform.Rotate(new Vector3(0,0,0));
			SpawnPoints[SpawnIndex].transform.Translate(new Vector3(Random.Range(0,2),Random.Range(0,2),0));
			if(ObjectIndex == 1){
				SpawnPoints[SpawnIndex].transform.Translate(new Vector3(0,0,-1));
			};
			Instantiate (objects [ObjectIndex], SpawnPoints[SpawnIndex].position, SpawnPoints[SpawnIndex].rotation);
			i = i + 1;
		}
	}

}

