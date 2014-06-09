using UnityEngine;
using System;
using System.Collections;

public class EndOfDayPanelManager : MonoBehaviour {
    public GoTween endOfDayPanelTween;

	// Use this for initialization
	void Start () {
	    endOfDayPanelTween = Go.to(transform, 0.3f, new GoTweenConfig().localPosition(Vector3.zero).startPaused());
        endOfDayPanelTween.autoRemoveOnComplete = false;
	}


    public void Display( Action<AbstractGoTween> callback = null)
    {

        endOfDayPanelTween.playForward();
        endOfDayPanelTween.setOnCompleteHandler(callback );
        God.instance.fader.GetComponent<UIPlayAnimation>().Play(true);

        //endOfDayPanelTween.setOnCompleteHandler(callback );
    }

    public void ButtonHide()
    {
        Hide();
    }

    //OVERLOAD
    public void Hide(Action<AbstractGoTween> callback )
    {
        endOfDayPanelTween.setOnCompleteHandler(callback);
        ActualHiding();
    }

    public void Hide(){
        //endOfDayPanelTween.playBackwards();
        print("caccca");
        //endOfDayPanelTween.reverse();
       // endOfDayPanelTween.setOnCompleteHandler(x => { });
        endOfDayPanelTween.setOnCompleteHandler(null);
        ActualHiding();
    //    endOfDayPanelTween.play();
    }

    public void ActualHiding()
    {
        endOfDayPanelTween.playBackwards();
        God.instance.fader.GetComponent<UIPlayAnimation>().Play(false);

    }
}
