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

    public float[] customerSpawnMinMax;

    public GameObject customersQueue;
	//public  List<GameObject> zones = new List<GameObject>();
    public GameObject[] zones;
    public List<Customer> customers = new List<Customer>();
    public GameObject customerPrefab;
    public GameObject gameScreen;
    public GameObject feedbackIconPrefab;
    public CustomerPanelManager customerPanelManager;
    public ZonePanelManager zonePanelManager;
    public TextAsset csv;
    public List<Customer> possibleCustomersPool = new List<Customer>();
    public GameObject fader;
    public float totalTimeForTheDay = 600f;

    [DoNotSerialize] public static float amberMoodTreshold = 7;
    [DoNotSerialize] public static float redMoodTreshold = 5;
    
    //public enum PossibleMoods {
    //    green ,
    //    amber ,
    //    red 
    //}

    //public PossibleMoods moood;

    //public List<string, UILabel> scoreLabels;



    public ScoreTracker score;
    public MainScreenIconDictionary scoreLabels;

    //SCORE TRACKING BITS

    public void CustomerLost(Customer customer)
    {
        if (customer.currentZone != null ){
            customer.currentZone.RemoveCustomer(customer);
        }
        if (customerPanelManager.currentCustomer == customer)
        {
            customerPanelManager.Hide();
        }
        score.totalCustomersLost++;
        score.totalCustomersProcessed++;
        score.totalNPSForTheDay += 1f;
        List<Customer> removedList = new List<Customer>(customers);
        removedList.Remove(customer);
        customers = removedList;
        
        UpdateScoresMenu();
    }

    public void CustomerProcessedSuccesfully(Customer customer)
    {
        //Could check for failed upsell here ? And skip the rest and call custmerlost if upselling failed... 
        score.totalCustomersProcessed++;
        customers.Remove(customer);
        score.totalNPSForTheDay += customer.nps;
        score.totalSpendForTheDay += customer.spend;

        

        UpdateScoresMenu();
    }

    void UpdateScoresMenu()
    {
        scoreLabels.totalNPSLabel.text = Mathf.Floor(score.totalNPSForTheDay).ToString("00");
        scoreLabels.totalCustomersLabel.text = Mathf.Floor(customers.Count).ToString("00");
        scoreLabels.totalCustomersProcessedLabel.text = Mathf.Floor(score.totalCustomersProcessed - score.totalCustomersLost).ToString("00");
    }

    
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


	private List<Customer> test = new List<Customer>();


    public void Start()
    {
        UpdateScoresMenu();
        
        
        possibleCustomersPool = CustomerImporter.ProcessCSV(csv);
        zones = GameObject.FindGameObjectsWithTag("zone") as GameObject[];
        StartCoroutine(DelayedAddingOfCustomers());


    }

   
        static public GameObject AddChild(GameObject parent, GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;

            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                t.localPosition = prefab.transform.localPosition;
                t.localRotation = prefab.transform.localRotation;
                t.localScale = prefab.transform.localScale;
                go.layer = parent.layer;
            }
            return go;
        }

    void TestingGame()
    {
        //Customer newCustomer = new Customer();
        //newCustomer.Create();


		int num = Random.Range(0, 27); // This is only a test
        //You did the range wrong, 1 , 28 brings results... well, between 1 and 27. As the array size is 27, that means:
        //1- you never get the array[0] element
        //2- if you try and access array[27] everything breaks, cause it doesn't exist (it goes from 0 to 26).

		AddCustomer(new Customer(possibleCustomersPool[num]));

        //AddCustomer(newCustomer);

    }

    IEnumerator DelayedAddingOfCustomers()
    {
        TestingGame();

        yield return new WaitForSeconds(Random.Range(customerSpawnMinMax[0],customerSpawnMinMax[1]));
        StartCoroutine(DelayedAddingOfCustomers());

    }


    public void FadeZones(bool inOrOut)
    {
        foreach (GameObject zone in zones)
        {
            zone.GetComponent<UIPlayAnimation>().Play(inOrOut);
        }
    }


    public void AddCustomer(Customer customer)
    {
        customers.Add(customer);
        
        //GameObject customerView  = AddChild(gameScreen, customerPrefab);   


        GameObject customerView = NGUITools.AddChild(customersQueue, customerPrefab);
        customerView.transform.localPosition = new Vector3(customersQueue.GetComponent<UIGrid>().cellWidth * customersQueue.transform.childCount + 100f, customerView.transform.localPosition.y, 0f);
        customerView.GetComponent<CustomerView>().Create(customer);

        customersQueue.GetComponent<UIGrid>().Reposition();

        UpdateScoresMenu();
        //customersQueue.GetComponent<UIPanel>().Refresh();
        //BoxCollider coll = customersQueue.GetComponent<BoxCollider>();
      //  customerView.GetComponent<UISprite>().panel.Refresh() ;



        //coll.size = new Vector3(coll.size.x + customersQueue.GetComponent<UIGrid>().cellWidth * 2, coll.size.y, 0f);
        //collider.bounds.size = new Vector3(collider.bounds.size.x + 50, collider.bounds.size.y, 0f);
    }


    void UpdateCustomers()
    {
        foreach (Customer customer in customers)
        {

            if (customer.waiting)
            {
                customer.currentTimeAvailable -= Time.deltaTime;
                if (customer.currentTimeAvailable <= 0f)
                {
                    customer.Die();
                }
            }

            //Here be mood enhancing magiks
        }
    }


    public void SaveState()
    {
        fader.SetActive(true);
        LevelSerializer.Checkpoint();
        fader.SetActive(false);
    }

    public void LoadState()
    {
        LevelSerializer.Resume();
        //fader.SetActiveRecursively(true);


    }

    public void OnDeserialized()
    {
        print("deSERIALIZING GOD");
        fader.SetActive(false);
    }


    void Update()
    {
        UpdateCustomers();

        totalTimeForTheDay -= Time.deltaTime;
        scoreLabels.totalTimeLabel.text = Mathf.Ceil(totalTimeForTheDay / 60f).ToString();
        
        //DEBUG AREA
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestingGame();
            customerPanelManager.Hide();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //LevelSerializer.SaveGame("test");
            SaveState();

        }

        if (Input.GetKeyDown(KeyCode.A))
        {

            //LevelSerializer.SavedGames[LevelSerializer.SavedGames.Count - 1];
            LoadState();
            return;
            //LevelSerializer.LoadNow(LevelSerializer)
            //List<LevelSerializer.SaveEntry> tmp =  LevelSerializer.SavedGames.Get<string>(LevelSerializer.PlayerName);
            
            
            foreach (LevelSerializer.SaveEntry currentSvae in LevelSerializer.SavedGames[LevelSerializer.PlayerName]){
                if (currentSvae.Name == "test")
                {
                    LevelSerializer.LoadSavedLevel(currentSvae.Data);
                }
            }
           
        } 
    }
	

}