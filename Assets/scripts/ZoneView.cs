using UnityEngine;
using System.Collections;


public class ZoneView : MonoBehaviour {
    public Zone zoneModel;
    public GameObject queue;
    public UILabel customerNumberLabel;
    public UISprite icon;



    public void UpdateCustomerNumber()
    {
        customerNumberLabel.text = zoneModel.customers.Count.ToString();
        HSBColor red = new HSBColor(Color.red);
        HSBColor green = new HSBColor(Color.green);

        icon.color = HSBColor.ToColor( HSBColor.Lerp(green, red, (float)zoneModel.customers.Count / zoneModel.maxQueue)); 
        //icon.color = Color.Lerp(Color.green, Color.red, (float) zone.customers.Count / zone.maxQueue );
    }

	void OnClick () {
		
        GetComponent<UIPlayAnimation>().Play(true);
        //EventDelegate displayZoneScreen = new EventDelegate(God.instance.zonePanelManager,"Display");
        //EventDelegate.Parameter parameter = new EventDelegate.Parameter();
        //parameter.obj = zoneModel;
        
        //displayZoneScreen.parameters = new EventDelegate.Parameter[1]{ parameter};
        God.instance.zonePanelManager.Display(zoneModel);
        //GetComponent<UIPlayAnimation>().onFinished = 

	}
	// Use this for initialization
	void Start () {
        zoneModel = GetComponent<Zone>();
        queue = GameObject.Find("Queue");
	}
	
	// Update is called once per frame
	void Update () {

	
	}

    public void UpdateProgressIndicator(float percentageOfCompletion)
    {

    }

    void OnDrop(GameObject customer)
    {
        CustomerView customerView = customer.GetComponent<CustomerView>();
        if (customerView)
        {
            if (zoneModel.queueOpen)
            {
                customerView.CustomerDroppedInZone(zoneModel);
            }
            else
            {
                
                GameObject failIcon = Instantiate(God.instance.feedbackIconPrefab , customer.transform.position, Quaternion.identity) as GameObject;
                
                failIcon.GetComponent<FeedbackIcon>().icon = FeedbackIcon.Icons.Full;

                //returns the customer to its grid and display/plays something?
                //customer.transform.parent = queue.transform;
                //customerView.GetComponent<ExampleDragDropItem>()
            }
        }
        
    }


}
