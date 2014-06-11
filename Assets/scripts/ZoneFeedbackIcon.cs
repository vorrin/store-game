using UnityEngine;
using System.Collections;

public class ZoneFeedbackIcon : MonoBehaviour {

    public Vector3 scale = Vector3.one * .5f;
    public enum Icons
    {
        Null,
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
        if (zoneIcon == Icons.Null)
        {
            Destroy(gameObject);
            return;
        }
        GetComponent<UISprite>().spriteName = zoneIcon.ToString();
        GetComponent<UISprite>().MakePixelPerfect();
        transform.parent = God.instance.gameScreen.transform;
        transform.localScale = scale;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
