using UnityEngine;
using System.Collections;

public class ZoneView : MonoBehaviour {


	void OnPress (bool isDown) {
			if (!isDown) {
			Debug.Log("I GOT  CLICKED ON HERE!");
				}
		}

	void OnClick () {
		Debug.Log ("clicKed on zone");
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	
	}
}
