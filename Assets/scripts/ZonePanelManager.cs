using UnityEngine;
using System.Collections;

public class ZonePanelManager : MonoBehaviour {

    public Zone currentZone;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Display(Zone zone)
    {
        //     gameObject.SetActiveRecursively(true);
        //DEBUG tmp hack
        GetComponent<UIPlayAnimation>().Play(true);
        currentZone = zone;
    }

    public void Hide()
    {
        GetComponent<UIPlayAnimation>().Play(false);
        GetComponent<UIPlayAnimation>().disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterReverse;
        currentZone.GetComponent<UIPlayAnimation>().Play(false);
    }

}
