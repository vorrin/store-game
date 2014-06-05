using UnityEngine;
using System.Collections;

public class CustomerPanelManager : MonoBehaviour {
    public UISprite avatarWindow;
    public UILabel scenarioLabel;
    public Customer currentCustomer;
    public UILabel timerLabel;
    public UILabel totalTimeLabel;

    void Start()
    {
        //gameObject.SetActiveRecursively(false);           
    }

    public void Display(Customer customer)
    {
   //     gameObject.SetActiveRecursively(true);
        //DEBUG tmp hack
        GetComponent<UIPlayAnimation>().Play(true);
        God.instance.fader.clipName = "FaderAnim";
        God.instance.fader.Play(true);

        currentCustomer = customer;
        avatarWindow.spriteName = customer.avatarName + "1" ;
        scenarioLabel.text = customer.scenario;
        totalTimeLabel.text = FormatSecondsIntoTimerDisplay(customer.initialTimeAvailable);
        GetComponentInChildren<UIScrollView>().ResetPosition();
        scenarioLabel.ResizeCollider();
    }

    public string FormatSecondsIntoTimerDisplay(float totalSeconds)
    {
        int minutes = (int)Mathf.Floor(totalSeconds / 60);
        int seconds = (int)totalSeconds % 60;
        return minutes + ":" + seconds.ToString("00");
    }

    public void Hide()
    {
        GetComponent<UIPlayAnimation>().Play(false);
        God.instance.fader.Play(false);

        GetComponent<UIPlayAnimation>().disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterReverse;
    }


	// Update is called once per frame
	void Update () {
        if (currentCustomer == null)
        {
            return;
        }

        timerLabel.text = FormatSecondsIntoTimerDisplay(currentCustomer.currentTimeAvailable);

	}
}
