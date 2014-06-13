using UnityEngine;
using System.Collections;

public class ContinueButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
        RefreshActive();
	}

    public void RefreshActive()
    {
        if (!LevelSerializer.CanResume)
        {
            GetComponent<UIButton>().isEnabled = false;
        }
    }

    void OnClick()
    {
        if (God.instance.gameStarted)
        {
            God.instance.mainMenuContainer.GetComponent<UITweener>().PlayForward();
        }
        else
        {
            God.instance.LoadState();
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
