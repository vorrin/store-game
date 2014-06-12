﻿using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
//using System.Xml;

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
    public List<Zone> zones = new List<Zone>();
    public List<Customer> customers = new List<Customer>();
    public GameObject customerPrefab;
    public GameObject gameScreen;
    public GameObject feedbackIconPrefab;
    public GameObject zoneFeedbackIconPrefab;
    public CustomerPanelManager customerPanelManager;
    public ZonePanelManager zonePanelManager;
    public TextAsset csv;
    public List<Customer> possibleCustomersPool = new List<Customer>();
    public GameObject fader;
    public float daytimeTotal = 600f;
    public float daytimeRemaining = 600f;
    public GameObject buyPanel;
    public bool customerDragging = false;
    

    public float moodModifierBonusForBestChoice = 2f;
    public float moodModifierMalusForSecondBestChoice = 3f;
    public bool endOfDayPhase = false;
    public EndOfDayPanelManager endScreenPanel;
    public float trainingStepCost = 20f;
    public float hireNewStaffCost = 80f;
    public int currentLevel = 0;

    public DifficultyLevelEntry[] difficultyLevels;
    public UILabel difficultyLevelLabel;
    public bool gameStarted = false;
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
    public GameObject mainMenuContainer;

    //SCORE TRACKING BITS





    //DEBUG XML LOADER BAD
    public IEnumerator LoadDebugXML(string xmlName)
    {
        print("trying");
        //Root is project root-root (assets/ cd ../../)
        print(Application.dataPath);
        WWW www;
        if (Application.isWebPlayer)
        {
            www = new WWW(Application.dataPath + @"/" + xmlName);
        }
        else
        {
            www = new WWW("file://" + xmlName);
        }

        yield return www;
        DifficultyLevelEntry debugDifficulty = XmlSupport.DeserializeXml<DifficultyLevelEntry>(www.text);
        difficultyLevels[0] = debugDifficulty;
        SetDifficultyLevel(difficultyLevels[0]);
        //currentLevel = 0;
        StopCoroutine("DelayedAddingOfCustomers") ;
        StartCoroutine(DelayedAddingOfCustomers());

        string text = www.text;
        
    }


    //Homemade Hover
    //public List<ZoneView> hoveredPreviously = new List<ZoneView>();
    //private float touchStart = 0f;
    //public void  CustomHover (){
    //    print("over and over");
    //    Vector3 inputPosition = new Vector3(-500f, -500f, 0f);
    //    if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android || true )
    //    {
    //        if (Input.touchCount > 0)
    //        {
    //            if (touchStart == 0f)
    //            {
    //                touchStart = Time.time;
    //            }

    //            Touch touch = Input.GetTouch(0);
    //            // if touch ended

    //            inputPosition = touch.position;

    //            if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
    //            {
    //                float touchTime = Time.time - touchStart;
    //                if (touchTime < .3f)
    //                {
    //                    hoveredPreviously = new List<ZoneView>();
    //                    inputPosition = new Vector3(-500f, -500f, 0f);

    //                    return;
    //                }
    //                else
    //                {
    //                    inputPosition = new Vector3(-500f, -500f, 0f);
    //                }
    //                // hoveredPreviously = new List<ZoneView>();
    //                touchStart = 0f;
    //                //  Input.mousePosition = new Vector3(-500f, -500f, 0f);
    //            }
    //          //  print(Input.GetTouch(0).deltaTime);
    //        }
    //        //else
    //        //{

    //        //    float touchTime = Time.time - touchStart;
    //        //    if (touchTime < .3f && touchStart!=0f)
    //        //    {
    //        //        //was just a tap
    //        //        print("tappp");
    //        //        hoveredPreviously = new List<ZoneView>();
    //        //        return;

    //        //    }
    //        //    else {

    //        //        //       print("WAS NOT A TAP");
    //        //        //  print ("long");
    //        //        inputPosition = new Vector3(-5080f, -5080f, 0f);

    //        //    }
    //        //    touchStart = 0f;

                
    //        //}
    //    }
    //    else {
    //        inputPosition = Input.mousePosition;
    //    }
        

    //    //    if (hoveredPreviously.Count == 0)
    //    //    {
    //    //        print("going hooom");
    //    //        yield return new WaitForSeconds(0.1f);
    //    //    }
            
    //    //}
    //    GameObject hoveredObject;
    //    RaycastHit lastHit;
    //    UICamera cam = UICamera.mainCamera.GetComponent<UICamera>();
    //    Camera mainCam = UICamera.mainCamera;

    //    //if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
    //    //{
    //    //    if (Input.touchCount > 0)
    //    //    {
    //    //        inputPosition = Input.GetTouch(0).position;
    //    //    }
    //    //    else
    //    //    {
    //    //        inputPosition = new Vector3(-5080f, -5080f, 0f);
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    inputPosition = Input.mousePosition;
    //    //}

    //    Ray ray = mainCam.ScreenPointToRay(inputPosition);
    //    float dist = (cam.rangeDistance > 0f) ? cam.rangeDistance : mainCam.farClipPlane - mainCam.nearClipPlane;
    //    int mask = mainCam.cullingMask & (int)cam.eventReceiverMask; // NOT NEEDED WE GOT ONLY ZONES

    // //   print (LayerMask.NameToLayer("Zone"));

    //    RaycastHit[] hits = Physics.RaycastAll(ray, dist, 1 << LayerMask.NameToLayer("Zone"));

        
    //    List<ZoneView> hoveredCurrently = new List<ZoneView>();

    //    foreach (RaycastHit hit in hits)
    //    {
            
    //       // Debug.Log("COLLIDER + " + hit.collider.name);
    //        if (hit.collider.name == "Fader")
    //        {
    //            print("TOUCHING FADERRR");
    //            hoveredCurrently = new List<ZoneView>();
    //            break;
    //        }
    //        ZoneView view = hit.collider.GetComponent<ZoneView>();
    //        if (!view)
    //        {
    //            Debug.Log("CONTINUINGNGGG");
    //            continue;
    //        }
    //        if (!hoveredPreviously.Contains(hit.collider.GetComponent<ZoneView>()))
    //        {
    //            view.OnCustomHover(true);
    //        }
    //        hoveredCurrently.Add(view);

    //    }
    //    foreach (ZoneView zoneView in hoveredPreviously)
    //    {
    //        if (!hoveredCurrently.Contains(zoneView))
    //        {
    //            zoneView.OnCustomHover(false);
    //        }
    //    }

    //    hoveredPreviously = hoveredCurrently;

    //    //if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || true) && Input.touchCount == 0)
    //    //{
    //    //    print("I AM ANDROID AND A M GOINBAK ");
    //    //    hoveredPreviously = new List<ZoneView>();
    //    //}
    //    //else
    //    //{
    //    //    hoveredPreviously = hoveredCurrently;

    //    //}


    //    return; 
    // //   yield return new WaitForSeconds(0.1f);
    ////    StartCoroutine(CheckHoveredObjects());
    //    //if (!UICamera.Raycast(Input.mousePosition, out lastHit)) ;
    ////	Debug.Log(lastHit.collider.name);
    //    //if (hoveredObject == null) hoveredObject = genericEventHandler;
    ////	for (int i = 0; i < 3; ++i) mMouse[i].current = hoveredObject;
    //}



    public void SetDifficultyLevel(DifficultyLevelEntry level)
    {
        difficultyLevelLabel.text = (currentLevel + 1).ToString();
        customerSpawnMinMax=  new float[2] {level.minSpawnTime, level.maxSpawnTime };
        daytimeTotal = level.durationOfDay;
        daytimeRemaining = daytimeTotal;
    }

    public void CustomerLost(Customer customer)
    {
        if (customer.currentZone != null ){
            customer.currentZone.RemoveCustomer(customer,ZoneFeedbackIcon.Icons.DeathInQueue);
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


    public void CustomerProcessedSuccessfully(Customer customer, bool upselling = false )
    {
        if (upselling)
        {
            print("UPSELLING yO");
            customer.spend = customer.spend * 2;
        }
        score.totalCustomersProcessed++;
        score.totalNPSForTheDay += customer.nps;
        score.totalSpendForTheDay += customer.spend;
        customers.Remove(customer);
        UpdateScoresMenu();
    } 

    public void CustomerProcessingCompleteGod(Customer customer)
    {
        //Could check for failed upsell here ? And skip the rest and call custmerlost if upselling failed... 
        if (customer.attemptingUpsell)
        {
            string currentMood = customer.GetMoodColor();
            if (!customer.upsellable)
                //customer is NOT upsellable
            {
                if (currentMood == "green")
                {
                    //nothing happens ,sale completes normally.
                    customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.SaleFine);
                    //customer.currentZone.FireFeedbackToZones(ZoneFeedbackIcon.Icons.SaleFine);
                    CustomerProcessedSuccessfully(customer);
                    return;
                }
                else
                {
                    //Customer sale fails through upselling
                    customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.UpsellFail);

                    //customer.currentZone.FireFeedbackToZones(ZoneFeedbackIcon.Icons.UpsellFail);
                    CustomerLost(customer);
                    return;
                }
            }
            else //customer upsellable and upsold
            {
                if (currentMood == "red")
                {
                    //customer.currentZone.FireFeedbackToZones(ZoneFeedbackIcon.Icons.UpsellFail);
                    customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.UpsellFail);

                    CustomerLost(customer);
                    return;
                }
                else
                {
                    //SUCCESS CUSTOMER UPSELLABLE AND UPSOLD
                    //customer.currentZone.FireFeedbackToZones(ZoneFeedbackIcon.Icons.UpsellFine);
                    customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.UpsellFine);

                    CustomerProcessedSuccessfully(customer,true);
                    return;

                }
            }

            
            //Feedback for upsellfail somehow?

        }
        else
        {
            //Upsell not attempted
            customer.currentZone.RemoveCustomer(customer, ZoneFeedbackIcon.Icons.SaleFine);
            CustomerProcessedSuccessfully(customer);
            return;
        }

    }


    //void TestTweens(AbstractGoTween asd )
    //{
    //    Debug.Log(asd);
    //    print("GGIGIGIGIO");
    //}

    

    void EndWorkingDay()
    {
        
        zonePanelManager.Hide();
        customerPanelManager.Hide();
        endOfDayPhase = true;
        customers = new List<Customer>();   
        foreach (Zone zone in zones)
        {
            zone.ClearZone();
            zone.zoneViews.ForEach((zoneView) =>
            {
                zoneView.ZoneViewStateSetup();
            });
            //zone.GetComponent<ZoneView>().ZoneViewStateSetup();
        }
        foreach (GameObject customerView in GameObject.FindGameObjectsWithTag("customer"))
        {
            customerView.GetComponent<CustomerView>().DestroyCustomerView();
        }
        StopCoroutine("DelayedAddingOfCustomers");
        
        endScreenPanel.Display( (x) => {
            RefreshStaffBuyingMenu();
            //CALLBACK -> do whatever you like / attach a method / whatever really!
        } );
        

    }

    void UpdateScoresMenu()
    {
        scoreLabels.totalNPSLabel.text = Mathf.Floor(score.totalNPSForTheDay / ( score.totalCustomersProcessed == 0 ? 1 : score.totalCustomersProcessed ) ).ToString("00");
        scoreLabels.totalCustomersLabel.text = Mathf.Floor(customers.Count).ToString("00");
        scoreLabels.totalCustomersProcessedLabel.text = Mathf.Floor(score.totalCustomersProcessed - score.totalCustomersLost).ToString("00");
    }

    void ResetScoresAtEndOfDay()
    {
        score.totalSpendForTheDay = score.resultSpending;
        score.totalCustomersLost = 0;
        score.totalCustomersProcessed = 0;
        score.totalNPSForTheDay = 0;
        score.resultSpending = 0;
    }

    
	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	
    public void DeleteOldSavegames()
    {
        //PlayerPrefs.GetString(PlayerName + "__RESUME__") = 
        PlayerPrefs.DeleteKey(LevelSerializer.PlayerName + "__RESUME__");
        Application.LoadLevel(0);
        foreach (LevelSerializer.SaveEntry currentSvae in LevelSerializer.SavedGames[LevelSerializer.PlayerName])
        {
            currentSvae.Delete();
        }

    }

	private List<Customer> test = new List<Customer>();

    public void OnApplicationFocus(bool isInFocus)
    {
        if (isInFocus)
        {
            //LOADING FOR MOBILES GOES HERE
         //   LoadState();
            // not the right one ... actually should work better... //if (LevelSerializer.CanResume) LoadState();
        }
        else
        {
            print("PAUSING AND SAVING ");
            SaveState();
        }
    }


    public void StartNextDay()
    {
        ResetScoresAtEndOfDay();
        currentLevel += 1;
        endOfDayPhase = false;
        daytimeRemaining = daytimeTotal;
        RefreshStaffBuyingMenu();
        foreach (Zone zone in zones)
        {
            zone.zoneViews.ForEach((zoneView) =>
            {
                zoneView.ZoneViewStateSetup();
            });
        };
        BeginGame();
    }

    public void BeginGame()
    {
        //if (PlayerPrefs.GetInt("playedBefore") == 0)
        //{
     //   print(PlayerPrefs.GetInt("playedBefore"));

      

        if (gameStarted == true)
        {
            LevelSerializer.LoadNow(LevelSerializer.SavedGames[LevelSerializer.PlayerName][0].Data);
            StartCoroutine("DelayedAddingOfCustomers");
            return;
            //customers = new List<Customer>();
            //zones.ForEach(zone =>
            //{
            //    zone.ClearZone();
            //});
            //ResetScoresAtEndOfDay();
            //score.totalSpendForTheDay = 0;
            //currentLevel = 0;
            //SetDifficultyLevel(difficultyLevels[currentLevel]);
            //UpdateScoresMenu();
            //StartCoroutine(DelayedAddingOfCustomers());

        }
        
        else {
            StartCoroutine("LoadDebugXML", "DebugSettings.xml");
            gameStarted = true;
            if (currentLevel >= difficultyLevels.Length)
            {
                //Quick smart so if you go over 5th day you can keep playing (same harsh difficulty level) 
                SetDifficultyLevel(difficultyLevels[difficultyLevels.Length - 1]);
            }
            else
            {
                SetDifficultyLevel(difficultyLevels[currentLevel]);
            }

            UpdateScoresMenu();

            FindTheZones();
            possibleCustomersPool = CustomerImporter.ProcessCSV(csv);
            LevelSerializer.SaveGame("base");
            StartCoroutine(DelayedAddingOfCustomers());
        }

      //  StartCoroutine(CheckHoveredObjects());
        
        //StartCoroutine(DelayedAddingOfCustomers());

        //    print("NEVER PLAYED");
        //    print("creating only game");

        //    LevelSerializer.SaveGame("base");

            //LevelSerializer.load
         //   PlayerPrefs.SetInt("playedBefore", 1);

        //}
        //else
        //{
        //    print("LOADING only game");
        ////    PlayerPrefs.SetInt("playedBefore", 0);
        //    StopCoroutine("DelayedAddingOfCustomers");

        //    foreach (LevelSerializer.SaveEntry save in LevelSerializer.SavedGames[LevelSerializer.PlayerName])
        //    {
        //        print("found " + save.Name);
        //    }
        //    LevelSerializer.LoadNow(LevelSerializer.SavedGames[LevelSerializer.PlayerName][0].Data);
        //    StartCoroutine(DelayedAddingOfCustomers());

        //}
    }

    public void FindTheZones()
    {
        GameObject[] zoneTagObjects = GameObject.FindGameObjectsWithTag("zone") as GameObject[];

        zones = new List<Zone>();
        foreach (GameObject possibleZone in zoneTagObjects)
        {
            if (possibleZone.GetComponent<Zone>() != null)
            {
                zones.Add(possibleZone.GetComponent<Zone>());
            }
        }
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
        if (!endOfDayPhase)
        {
            StartCoroutine(DelayedAddingOfCustomers());
        }

    }


    public void FadeZones(bool inOrOut)
    {
        print("fadzones");
        foreach (Zone zone in zones)
        {
            zone.zoneViews.ForEach( (zoneView) =>
            {
                zoneView.GetComponent<UIPlayAnimation>().Play(inOrOut);
            });
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

    public void QuitGame()
    {
        Application.Quit();
    }
    void OnApplicationQuit()
    {
        s_Instance = null;
        SaveState();
    }
    void OnApplicationPause(bool isPaused) // THIS IS WHAT SAVES THE GAME ON MOBILE! 
    {
        print("PAUSING AND " + isPaused);
        if (isPaused)
        {
            SaveState();
        }
    }



    public void OnDeserialized()
    {
        print("deSERIALIZING GOD");
        mainMenuContainer.GetComponent<UITweener>().PlayForward();
        FindTheZones();
        foreach (Zone zone in zones)
        {
            zone.zoneViews.ForEach((zoneView) =>
            {
                zoneView.ZoneViewStateSetup();
            });
            //zone.GetComponent<ZoneView>().ZoneViewStateSetup();
        }
        fader.SetActive(false);
        UpdateScoresMenu();
        RefreshStaffBuyingMenu();
        customersQueue.GetComponent<UIGrid>().Reposition();
    }

    public void RefreshStaffBuyingMenu()
    {
        foreach (Transform child in buyPanel.transform)
        {
            child.gameObject.SetActive( endOfDayPhase);
            
        };
        if (endOfDayPhase) buyPanel.GetComponentInChildren<UILabel>().text = score.resultSpending.ToString("0");
            
    }

    void OnGUI()
    {
    //    CheckHoveredObjects();
    }

    void Update()
    {

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DeleteOldSavegames();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            EndWorkingDay();

        }



        if (Input.GetKeyDown(KeyCode.A))
        {

            //LevelSerializer.SavedGames[LevelSerializer.SavedGames.Count - 1];
            LoadState();
            return;
            //LevelSerializer.LoadNow(LevelSerializer)
            //List<LevelSerializer.SaveEntry> tmp =  LevelSerializer.SavedGames.Get<string>(LevelSerializer.PlayerName);


            foreach (LevelSerializer.SaveEntry currentSvae in LevelSerializer.SavedGames[LevelSerializer.PlayerName])
            {
                if (currentSvae.Name == "test")
                {
                    LevelSerializer.LoadSavedLevel(currentSvae.Data);
                }
            }

        } 

        //END OF DEBUG


        daytimeRemaining -= Time.deltaTime;
        if (endOfDayPhase) return;

        UpdateCustomers();

        if (daytimeRemaining <= 0 )
        {
            scoreLabels.totalTimeLabel.text = "0"; 
            //DO SOMETHING LIKE MAKE CUSTOMERS DISAPPEAR AND BRING UP THE END SCREEN. 
            EndWorkingDay();
        }
        else
        {
            scoreLabels.totalTimeLabel.text = Mathf.Ceil(daytimeRemaining / 60f).ToString();
        }
        


        
    }
    public static God instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(God)) as God;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("AManager");
                s_Instance = obj.AddComponent(typeof(God)) as God;
                Debug.Log("Could not locate an AManager object.  AManager was Generated Automaticly.");
            }

            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.



}