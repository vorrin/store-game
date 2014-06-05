using UnityEngine;
using System.Collections;

public class CustomerPanelManager : MonoBehaviour {
    public UISprite avatarWindow;
    public UILabel scenarioLabel;
    public Customer currentCustomer;
    public UILabel timerLabel;
    public UILabel totalTimeLabel;
    public UILabel currentZoneLabel;

    void Start()
    {
        //gameObject.SetActiveRecursively(false);           
    }

    public void Display(Customer customer)
    {
   //     gameObject.SetActiveRecursively(true);
        //DEBUG tmp hack
        GetComponent<UIPlayAnimation>().Play(true);
        //if (!God.instance.zonePanelManager.displayingZone)
        //{

        if (!God.instance.zonePanelManager.displayingZone)
        {
            God.instance.zonePanelManager.backButton.enabled = false;
        }
        God.instance.fader.GetComponent<UIPlayAnimation>().clipName = "FaderAnim";
        God.instance.fader.GetComponent<UIPlayAnimation>().Play(true);
        //}

        currentCustomer = customer;
        avatarWindow.spriteName = customer.avatarName + "_PROFILE" ;
        scenarioLabel.text = customer.scenario;
        GetComponentInChildren<UpsellButton>().setUpselling(customer.attemptingUpsell);
        if (currentCustomer.currentZone == null)
        {
            currentZoneLabel.text = "Queue";
        }
        else
        {
            currentZoneLabel.text = currentCustomer.currentZone.zoneName;
        }
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
        if (!God.instance.zonePanelManager.displayingZone)
        {
            God.instance.zonePanelManager.backButton.enabled = true;
            God.instance.fader.GetComponent<UIPlayAnimation>().Play(false);
        }

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
