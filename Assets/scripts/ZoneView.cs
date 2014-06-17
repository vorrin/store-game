using UnityEngine;
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

        staffBuyingIndicatorSet.GetComponent<UILabel>().enabled = theDayIsOver;
        foreach (UISprite sprite in staffBuyingIndicatorSet.GetComponentsInChildren<UISprite>())
        {
            sprite.enabled = theDayIsOver;
        }
        if (theDayIsOver)
        {
            UpdateStaffNumber();
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


    public void OnCustomHover(bool isOver)
    {
        print("custover is " + isOver);
        if (God.instance.customerDragging && isOver)
        {
            foreach (GameObject zone in GameObject.FindGameObjectsWithTag("zone"))
            {
                
                if (zone.GetComponent<ZoneView>().zoneModel != zoneModel)
                {

                    zone.GetComponent<ZoneView>().zoneModel.zoneViews.ForEach(zoneView =>
                        zoneView.GetComponent<UIPlayAnimation>().Play(false)
                        );
                }
            }
        }
        
        
            zoneModel.zoneViews.ForEach(zoneView =>
            {
                zoneView.GetComponent<UIPlayAnimation>().Play(isOver);
            });
        
            
        

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

    public void FireFeedback(ZoneFeedbackIcon.Icons icon)
    {
        GameObject feedbackIcon = Instantiate(God.instance.zoneFeedbackIconPrefab, feedbackIconSpawner.transform.position, Quaternion.identity) as GameObject;
        feedbackIcon.GetComponent<ZoneFeedbackIcon>().zoneIcon = icon;
        feedbackIcon.SetParent(feedbackIconSpawner);
    }

    public void OnCustomDrop(GameObject customer)
    {
        print("dropping ");

        //if (!God.instance.customerDragging)
        //{
        //    return;
        //    Debug.Log("NODRAGING " );

        //}
        if (customer.tag != "customer")
        {
            return;
        }

        Debug.Log("TAT STAT OF THINGS INI " + customer.GetComponent<DragDropCustomer>().dragging);
        print("SOMETHING DROPPED ON ZONE");

    //    customer.collider.enabled = false;
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
