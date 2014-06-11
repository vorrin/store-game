using UnityEngine;
using System.Collections;

public class ZoneFeedbackIcon : MonoBehaviour {

    
    public enum Icons
    {
        SaleFine,
        UpsellFine,
        UpsellFail,
        DeathInQueue
    }
    public ZoneFeedbackIcon.Icons zoneIcon = Icons.SaleFine;


    public void FadedAway()
    {
        Destroy(gameObject);
    }

	// Use this for initialization
	public void Start () {
        GetComponent<UISprite>().spriteName = zoneIcon.ToString();
        GetComponent<UISprite>().MakePixelPerfect();
        transform.parent = God.instance.gameScreen.transform;
        transform.localScale = Vector3.one * .5f;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
