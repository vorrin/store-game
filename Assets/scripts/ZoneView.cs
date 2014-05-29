using UnityEngine;
using System.Collections;


public class ZoneView : MonoBehaviour {
    public Zone zone;
    public GameObject queue;
    public UILabel customerNumberLabel;
    public UISprite icon;



    public void UpdateCustomerNumber()
    {
        customerNumberLabel.text = zone.customers.Count.ToString();
        HSBColor red = new HSBColor(Color.red);
        HSBColor green = new HSBColor(Color.green);

        icon.color = HSBColor.ToColor( HSBColor.Lerp(green, red, (float)zone.customers.Count / zone.maxQueue)); 
        //icon.color = Color.Lerp(Color.green, Color.red, (float) zone.customers.Count / zone.maxQueue );
    }

	void OnClick () {
		Debug.Log ("clicKed on zone");
	}
	// Use this for initialization
	void Start () {
        zone = GetComponent<Zone>();
        queue = GameObject.Find("Queue");
	}
	
	// Update is called once per frame
	void Update () {

	
	}
    void OnDrop(GameObject customer)
    {
        CustomerView customerView = customer.GetComponent<CustomerView>();
        if (customerView)
        {
            if (zone.queueOpen)
            {
                customerView.CustomerDroppedInZone(zone);
                Debug.Log("ASDASDASDAD ASDAJAJAAJ");

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
