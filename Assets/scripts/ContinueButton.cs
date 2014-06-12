using UnityEngine;
using System.Collections;

public class ContinueButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (!LevelSerializer.CanResume)
        {
            GetComponent<UIButton>().isEnabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
