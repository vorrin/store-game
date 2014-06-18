using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerView : MonoBehaviourEx {

	public Customer customerModel;
	private TweenPosition tweener;
    public UISprite moodBubble;

    GoTween hoverTween;


	delegate void MyDelegate();
	MyDelegate myDelegate;
    public bool markedForDestruction = false;

	// Use this for initialization
	public void Create (Customer customer) {
        customerModel = customer;
        GetComponent<UISprite>().spriteName = customerModel.avatarName + "_QUEUE";
        GetComponent<CustomSpriteAnimation>().namePrefix = customerModel.avatarName + "_QUEUE" ;
        customerModel.customerView = this;
        SetMoodSprite();
	}

    public void OnDeserialized()
    {
        if (markedForDestruction)
        {
            Destroy(gameObject); 
            return;
        }
        SetMoodSprite();
    }

    public void CustomerDroppedInZone(Zone zone)
    {
        GameObject feedbackIcon = Instantiate(God.instance.feedbackIconPrefab, transform.position, Quaternion.identity) as GameObject;
        Customer.ZoneMatchingResults result = customerModel.DroppedInZone(zone);
        if (result == Customer.ZoneMatchingResults.Fail)
        {
            feedbackIcon.GetComponent<FeedbackIcon>().icon = FeedbackIcon.Icons.Fail;
            AudioManager.instance.PlayAudioForIcon(FeedbackIcon.Icons.Fail);
            GetComponent<UIDragDropItem>().enabled = false;
            customerModel.Die();
            return;
        }
        else if (result == Customer.ZoneMatchingResults.SecondBest)
        {
            //Should create little icon customer here?
            feedbackIcon.GetComponent<FeedbackIcon>().icon = FeedbackIcon.Icons.SecondBestOption;
        }
        else
        {
            //Should create little icon customer here?
            feedbackIcon.GetComponent<FeedbackIcon>().icon = FeedbackIcon.Icons.BestOption;
        }
        GetComponent<UIDragDropItem>().enabled = false;
        zone.AddCustomer(customerModel);
        AudioManager.instance.PlayAudioForIcon(feedbackIcon.GetComponent<FeedbackIcon>().icon);
    //    transform.parent = null;
        DestroyCustomerView();
    }

    //public void CreateCustomerIconInZone(Zone zone)
    //{
    //    GameObject customerIcon =  Instantiate(God.instance.customerIconPrefab, transform.position, Quaternion.identity) as GameObject;
    //    customerIcon.GetComponent<CustomerIcon>().Init(this);
    //    customerIcon.transform.parent = zone.transform;
    //    customerIcon.transform.localScale = Vector3.one * .5f;

    //}

    public void SetMoodSprite()
    {
        moodBubble.spriteName = "mood_" + customerModel.GetMoodColor() + "_QUEUE";
        //float nps = customerModel.nps;
        //string moodColor = "green";
        //if (nps > God.amberMoodTreshold)
        //{
        //    moodColor = "green";
        //}
        //else if (nps > God.redMoodTreshold)
        //{
        //    moodColor = "amber";
        //}
        //else
        //{
        //    moodColor = "red";
        //}
        //moodBubble.spriteName = "mood_" + moodColor + "_QUEUE";
    }

    public void StartDrag() 
    {
        God.instance.FadeZones(true);
        God.instance.customerDragging = true;
    }


    public void EndDrag()
    {
        God.instance.FadeZones(false);
        God.instance.customerDragging = false;
    }


    public void OnHover(bool isOver)
    {
        if (isOver)
        {
            //hoverTween.playForward();
            
            //Go.t
        }
        else
        {
            //hoverTween.playBackwards();
            //Go.to(gameObject.transform, .5f, new GoTweenConfig().localPosition(new Vector3(0f, 0f, 0f), true));
        }
    }

    //void OnPress(bool isDown) {
    //    return;
    //    if (isDown){
    //        //dragStartTime = Time.time;
    //        //dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    //        God.instance.FadeZones(true);
		
    //    }
    //    else{
    //        //beingDragged = false;
    //        God.instance.FadeZones(false);

    //        //if (Time.time - dragStartTime > 0.2f)
    //        //{
    //        //    Ray cast = UICamera.mainCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));

    //        //    RaycastHit[] hits;
    //        //    hits = Physics.RaycastAll(cast);
    //        //    bool hitAnotherZone = false;
    //        //    foreach (RaycastHit hit in hits)
    //        //    {
    //        //        if (hit.collider.name.Contains("Zone"))
    //        //        {
    //        //            Zone currentZone = hit.collider.GetComponent<Zone>();
    //        //            customerModel.ChangeZone(currentZone);
    //        //            //User dropped in the Entrance zone again.
    //        //            //if (currentZone != God.instance.entrance)
    //        //            //{
    //        //            //    Go.to(gameObject.transform, .5f, new GoTweenConfig().scale(0f).onComplete(DestroyCustomerView));
    //        //            //}
    //        //            //else
    //        //            //{
    //        //            //    WalkToRandomSpot();
    //        //            //}
    //        //            hitAnotherZone = true;
    //        //        }
    //        //    }

    //       //     if (!hitAnotherZone)
    //       //     {
    //       //  //       WalkToRandomSpot();
    //       ////         customerModel.ChangeZone(God.instance.entrance);
    //       //     }


    //        //}

						
    //    }
    //}

    public void DestroyCustomerView()
    {
       // GetComponent<StoreInformation>().enabled = false;
        DestroyCustomerView( () => { });
        
        //Go.to(gameObject.transform, .5f, new GoTweenConfig().scale(0f).onComplete(goTween =>
        //{
        //    Destroy(gameObject);
        //}));
        //GetComponent<CustomSpriteAnimation>().enabled = false;
        //collider.enabled = false;
    }
    public void DestroyCustomerView(System.Action action)
    {
        markedForDestruction = true;
        //Destroy(GetComponent<StoreInformation>());
        GetComponent<CustomSpriteAnimation>().enabled = false;
        collider.enabled = false;
      //  Debug.Break();
        
        Go.to(gameObject.transform, .5f, new GoTweenConfig().scale(0f).onComplete( goTween => {
            Destroy(gameObject);
            action();
        }));
      
       // Destroy(gameObject);
    }


	void OnClick(){ 
        God.instance.customerPanelManager.Display(customerModel);
	}

	void OnMouseUp(){
	}

	void OnRelease(){
	}
	
    //void TweenToPosition(Vector3 startPos, Vector3 endPos, float duration, float delay, EventDelegate[] onFinished ) {
    //    tweener.from = startPos;
    //    tweener.to = endPos; 
    //    tweener.duration = duration;
    //    tweener.delay = delay;
    //    tweener.ResetToBeginning();
    //    tweener.Play();
    //    foreach (EventDelegate currentDelegate in onFinished) {
    //        tweener.AddOnFinished(currentDelegate);
    //    }
    //}

    //void WalkToRandomSpot ( ) {
    //    EventDelegate newDelegate = new EventDelegate();
    //    newDelegate.target = this;
    //    newDelegate.methodName = "WalkToRandomSpot";
    //    newDelegate.oneShot = true;
    //    tweener.SetOnFinished(newDelegate);

    //    Debug.Log("SPOT ZERO POS: " + spots[0].transform.localPosition);
    //    Debug.Log("INVERSE SPOT POS: " + transform.parent.InverseTransformPoint( spots[0].transform.position ));

    //    TweenToPosition (this.transform.localPosition,  transform.parent.InverseTransformPoint( spots [Random.Range (0, spots.Length)].transform.position   ), 1f, 1f, new EventDelegate[] { newDelegate});
    //}


	// Update is called once per frame
    void FixedUpdate()
    {

    }
    void Update()
    {

    }

    void LateUpdate()
    {
    }
}
