using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

/// AManager is a singleton.
/// To avoid having to manually link an instance to every class that needs it, it has a static property called
/// instance, so other objects that need to access it can just call:
///        AManager.instance.DoSomeThing();
///
public class God : MonoBehaviour {
	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.

	
	private static God s_Instance = null;

    public Zone entrance;
	public  List<GameObject> zones = new List<GameObject>();
    public List<Customer> customers = new List<Customer>();
    public GameObject customerPrefab;
    public GameObject gameScreen;
    public GameObject entranceStart;
    public GameObject entranceEnd;

	
	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static God instance {
		get {
			if (s_Instance == null) {
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (God)) as God;
			}
			
			// If it is still null, create a new instance
			if (s_Instance == null) {
				GameObject obj = new GameObject("AManager");
				s_Instance = obj.AddComponent(typeof (God)) as God;
				Debug.Log ("Could not locate an AManager object.  AManager was Generated Automaticly.");
			}
			
			return s_Instance;
		}
	}
	
	// Ensure that the instance is destroyed when the game is stopped in the editor.
	void OnApplicationQuit() {
		s_Instance = null;
	}

    public void Start()
    {
        Customer newCustomer = new Customer();
        newCustomer.Create();
        customers.Add(newCustomer);
        GameObject customerView = Instantiate(customerPrefab,new Vector3(10000f,10000f,0f),customerPrefab.transform.rotation) as GameObject;
        //customerView.transform.localScale = new Vector3(.6f, .6f, 1f);
        customerView.transform.parent = entrance.transform;
        customerView.GetComponent<CustomerView>().Create(newCustomer);  
    }

    public void AddCustomer(Customer customer)
    {
        customers.Add(customer);
    }

    //void Update()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        Debug.Log("God IS PUSHING");
    //        Start();
    //    }
    //}
	

}