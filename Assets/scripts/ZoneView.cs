﻿using UnityEngine;
using System.Collections;


public class ZoneView : MonoBehaviour {
    public Zone zoneModel;
    public GameObject queue;
    public UILabel customerNumberLabel;
    public UILabel staffNumberLabel;
    public UISprite icon;
    public UISprite progressIndicator;
    public GameObject staffBuyingIndicatorSet;
    public GameObject customersPresenceIndicatorSet;
    public GameObject feedbackIconSpawner;

    // Use this for initialization
    void Start()
    {
        if (zoneModel == null)
        {
            zoneModel = GetComponent<Zone>();
        }
        queue = GameObject.Find("Queue");
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void UpdateCustomerNumber()
    {
        customerNumberLabel.text = zoneModel.customers.Count.ToString();
        float transparency = .3f;
        HSBColor red = new HSBColor(new Color(1f,0f,0f,transparency));
        HSBColor green = new HSBColor(new Color(0f,1f,0f,transparency));

        icon.color = HSBColor.ToColor( HSBColor.Lerp(green, red, (float)zoneModel.customers.Count / zoneModel.maxQueue));
        progressIndicator.color = icon.color;

        progressIndicator.fillAmount = 0f;
        //icon.color = Color.Lerp(Color.green, Color.red, (float) zone.customers.Count / zone.maxQueue );
    }

    public void UpdateStaffNumber()
    {
        //UNUSED NOW
        staffNumberLabel.text = zoneModel.staffNumber.ToString();
    }

    public void ZoneViewStateSetup() // switches between endofday and day displays
    {

        bool theDayIsOver = God.instance.endOfDayPhase;
        customersPresenceIndicatorSet.GetComponent<UILabel>().enabled = !theDayIsOver;
        //Disabling the single elements cause else the saving system gets confused, I think.
        foreach (UISprite sprite in customersPresenceIndicatorSet.GetComponentsInChildren<UISprite>())
        {
            sprite.enabled = !theDayIsOver;
        }

        //staffBuyingIndicatorSet.GetComponent<UILabel>().enabled = theDayIsOver; // we don't show the label anymore
        foreach (UISprite sprite in staffBuyingIndicatorSet.GetComponentsInChildren<UISprite>())
        {
            sprite.enabled = theDayIsOver;
        }
        if (theDayIsOver)
        {
//            UpdateStaffNumber();
        }
        else
        {
            UpdateCustomerNumber();
        }
    }


    void OnHover(bool isHovering)
    {
        return;
        if (isHovering)
        {
            zoneModel.zoneViews.ForEach(zoneView =>
            {
                zoneView.GetComponent<UIPlayAnimation>().Play(isHovering);
            });
        }
        else
        {
            zoneModel.zoneViews.ForEach(zoneView =>
            {
                zoneView.GetComponent<UIPlayAnimation>().Play(isHovering);
            });
        }
    }


	void OnClick () {
        zoneModel.zoneViews.ForEach(zoneView =>
        {
            zoneView.GetComponent<UIPlayAnimation>().Play(true);
        });
        God.instance.zonePanelManager.Display(zoneModel);
	}
	
    public void UpdateProgressIndicator(float percentageOfCompletion)
    {
        progressIndicator.fillAmount = percentageOfCompletion;
    }

    public void FireFeedback(ZoneFeedbackIcon.Icons icon)
    {
        GameObject feedbackIcon = Instantiate(God.instance.zoneFeedbackIconPrefab, feedbackIconSpawner.transform.position, Quaternion.identity) as GameObject;
        feedbackIcon.GetComponent<ZoneFeedbackIcon>().zoneIcon = icon;
        feedbackIcon.SetParent(feedbackIconSpawner);
    }

    public void OnCustomDrop(GameObject customer)
    {
        if (customer.tag != "customer")
        {
            return;
        }

        CustomerView customerView = customer.GetComponent<CustomerView>();
        if (customerView)
        {
            if (zoneModel.queueOpen)
            {
                customerView.CustomerDroppedInZone(zoneModel);
            }
            else
            {
                GameObject fullIcon = Instantiate(God.instance.feedbackIconPrefab , customer.transform.position, Quaternion.identity) as GameObject;
                fullIcon.GetComponent<FeedbackIcon>().icon = FeedbackIcon.Icons.Full;
                AudioManager.instance.PlayAudioForIcon(FeedbackIcon.Icons.Full);
            }
        }
        
    }


}
