using UnityEngine;
using System.Collections;

public class CustomerPanelManager : MonoBehaviour {
    public UISprite avatarWindow;
    public UILabel scenarioLabel;


    public void Display(string avatarImageName, string scenarioText)
    {
        //DEBUG tmp hack
        avatarWindow.spriteName = avatarImageName + "1" ;
        scenarioLabel.text = scenarioText;
        GetComponentInChildren<UIScrollView>().ResetPosition();
        scenarioLabel.ResizeCollider();
        GetComponent<UIPlayAnimation>().Play(true);

    }

    public void Hide()
    {
        GetComponent<UIPlayAnimation>().Play(false);

    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
