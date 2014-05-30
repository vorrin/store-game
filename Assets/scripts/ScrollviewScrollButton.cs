using UnityEngine;
using System.Collections;

public class ScrollviewScrollButton : MonoBehaviour {
    public UIScrollView scrollView;
    public enum Directions
    {
        Right,
        Left
    }
    public Directions direction;
    public float amountToScroll = 30f;


    public void OnClick()
    {
        float delta = 0f; ;
        switch (direction){
            case Directions.Right:
                delta = amountToScroll;
                break;
            case Directions.Left:
                delta = -amountToScroll;
                break;
        }
        scrollView.Scroll(delta);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
