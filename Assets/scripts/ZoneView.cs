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
        staffNumberLabel.text = zoneModel.staffNumber.ToString();
    }

    public void ZoneViewStateSetup() // switches between endofday and day displays
    {
        bool theDayIsOver = God.instance.endOfDayPhase;
        customersPresenceIndicatorSet.SetActive(!theDayIsOver);
        staffBuyingIndicatorSet.SetActive(theDayIsOver);
        if (theDayIsOver)
        {
            UpdateStaffNumber();
        }
        else
        {
            UpdateCustomerNumber();
        }
    }

	void OnClick () {
        zoneModel.zoneViews.ForEach(zoneView =>
        {
            zoneView.GetComponent<UIPlayAnimation>().Play(true);
        });

        //EventDelegate displayZoneScreen = new EventDelegate(God.instance.zonePanelManager,"Display");
        //EventDelegate.Parameter parameter = new EventDelegate.Parameter();
        //parameter.obj = zoneModel;
        
        //displayZoneScreen.parameters = new EventDelegate.Parameter[1]{ parameter};
        God.instance.zonePanelManager.Display(zoneModel);
        //GetComponent<UIPlayAnimation>().onFinished = 

	}
	
    public void UpdateProgressIndicator(float percentageOfCompletion)
    {
        progressIndicator.fillAmount = percentageOfCompletion;
    }

    void OnDrop(GameObject customer)
    {
        print("SOMETHING DROPPED ON ZONE");
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
            }
        }
        
    }


}
