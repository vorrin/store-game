using UnityEngine;
using System.Collections;

public class FeedbackIcon : MonoBehaviour {

    public enum Icons
    {
        Fail,
        Full,
        BestOption,
        SecondBestOption
    }

    public Icons icon;

    public void FadedAway()
    {
        Destroy(gameObject);
    }

	// Use this for initialization
	void Start () {
        GetComponent<UISprite>().spriteName = icon.ToString();
        transform.parent = God.instance.gameScreen.transform;
        transform.localScale = Vector3.one;
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
