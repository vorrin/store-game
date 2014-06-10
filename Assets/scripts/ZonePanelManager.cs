using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ZonePanelManager : MonoBehaviour {

    public Zone currentZone;
    public ZoneGrid queue;
    public UILabel staffTrainingPercent;
    public UILabel staffNumber;
    public UILabel zoneName;
    public UIButton backButton;
    public bool displayingZone = false;
    public UIButton hireButton;
    public UIButton trainButton;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Display(Zone zone)
    {
        //  God.instance.fader.SetActive(true);

        currentZone = zone;
        displayingZone = true;
        ClearZonePanel();
        zoneName.text = zone.zoneName;
        //DEBUG tmp hack
        GetComponent<UIPlayAnimation>().Play(true);
        EventDelegate asd = new EventDelegate();
        God.instance.fader.GetComponent<UIPlayAnimation>().clipName = "ZoneFaderAnim";
        God.instance.fader.GetComponent<UIPlayAnimation>().Play(true);
        PopulateZonePanel();
        if (God.instance.endOfDayPhase)
        {
            RefreshStaffButtons();
        }
    }

    public void OnDeserialized()
    {
        RefreshStaffButtons();
    }

    public void RefreshStaffButtons()
    {
        if (currentZone.staffPower  % 100 == 0) // Unity float weirdness
        {
            print("modulo as exp");
            hireButton.gameObject.SetActive(true);
            trainButton.gameObject.SetActive(false);
            if (God.instance.score.resultSpending < God.instance.hireNewStaffCost)
            {
                hireButton.isEnabled = false;
            }
            else
            {
                hireButton.isEnabled = true;
            }

        }
        else
        {
            hireButton.gameObject.SetActive(false);
            trainButton.gameObject.SetActive(true);
            if (God.instance.score.resultSpending < God.instance.trainingStepCost)
            {
                trainButton.isEnabled = false;
            }
            else
            {
                trainButton.isEnabled = true;
            }
        }
        PopulateZonePanel();
    }

    public void HireStaff()
    {
        //Some cost shall be elicited
        God god = God.instance;
        god.score.resultSpending -= god.hireNewStaffCost;
        god.RefreshStaffBuyingMenu();
        currentZone.staffNumber += 1;
        currentZone.staffPower += 20;
        currentZone.zoneViews.ForEach((zoneView) =>
        {
           zoneView.UpdateStaffNumber() ;
        });

        //currentZone.zoneViews.UpdateStaffNumber();
        RefreshStaffButtons();
    }

    public void TrainStaff()
    {
        God god = God.instance;
        god.score.resultSpending -= god.trainingStepCost;
        god.RefreshStaffBuyingMenu();

        currentZone.staffPower += 20;
        RefreshStaffButtons();
    }

    public void ReorderCustomerList( BetterList<Transform> list ){ // THIS HAPPENS IN REPOSITION, ZoneGrid.cs !!!! 
        //print(" THE LIST IS SIZED SO : " + list.size);
        if (list.size == 0) return;
        List<Customer> customers = new List<Customer>();
        foreach (Transform customerTransform in list)
        {
            customers.Add(customerTransform.GetComponent<CustomerView>().customerModel);
        }
        currentZone.customers = customers;
        //currentZone.currentlyProcessedCustomer.waiting = false;
        if (customers[0] != currentZone.currentlyProcessedCustomer) // First customer has changed.
        {
            currentZone.StartCustomerProcessing();
        }
    }

    public void PopulateZonePanel()
    {
        //Updating labels
        staffTrainingPercent.text = currentZone.staffPower.ToString() + "%";
        staffNumber.text = currentZone.staffNumber.ToString();
        //Customers adding etc
        ClearZonePanel();


        if (currentZone.customers.Count == 0) return;
        foreach (Customer customer in currentZone.customers)
        {
            GameObject currentCustomer  = NGUITools.AddChild(queue.gameObject, God.instance.customerPrefab);
            currentCustomer.RemoveComponent(typeof(StoreInformation));

            //GameObject currentCustomer = Instantiate(God.instance.customerPrefab) as GameObject;
            currentCustomer.transform.localScale = Vector3.one;
            currentCustomer.GetComponent<CustomerView>().Create(customer);
            currentCustomer.transform.parent = queue.transform;
        }
        queue.Reposition();
    }



    public void ClearZonePanel()
    {
        
        List<GameObject> customersToBeDestroyed = new List<GameObject>();
        foreach (Transform child in queue.transform)
        {
            if (!child.name.Contains("Spot"))
            {
                customersToBeDestroyed.Add(child.gameObject);
            }
        }
        foreach (GameObject customer in customersToBeDestroyed)
        {
            customer.transform.parent = null;
            Destroy(customer);
        }
    }

    

    public void RemoveCustomer(Customer processedCustomer)
    {
        if (!this.enabled)
        {
            return;
        }
        foreach (Transform customerView in queue.transform)
        {
            if (customerView.name.Contains("Spot")) continue;
            if (customerView.GetComponent<CustomerView>().customerModel == processedCustomer)
            {
                Destroy(customerView.gameObject);
                customerView.parent = null;
                queue.Reposition();
                return;
            }
            
        }
    }


    public void Hide()
    {
        displayingZone = false;
        GetComponent<UIPlayAnimation>().Play(false);
        God.instance.fader.GetComponent<UIPlayAnimation>().clipName = "FaderAnim";
        God.instance.fader.GetComponent<UIPlayAnimation>().Play(false);
        GetComponent<UIPlayAnimation>().disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterReverse;
        if (currentZone != null)
        {
            currentZone.zoneViews.ForEach(zoneView =>
            {
                zoneView.GetComponent<UIPlayAnimation>().Play(false);
            });
        };
        hireButton.gameObject.SetActive(false);
        trainButton.gameObject.SetActive(false);

    }

}
