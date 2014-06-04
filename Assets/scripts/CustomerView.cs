using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerView : MonoBehaviour {

	public Customer customerModel;
	public GameObject[] spots;
	private TweenPosition tweener;
	private UIDragObject Dragger;

    public float dragStartTime;
    GoTween hoverTween;


	delegate void MyDelegate();
	MyDelegate myDelegate;

	// Use this for initialization
	public void Create (Customer customer) {
        customerModel = customer;
        GetComponent<CustomSpriteAnimation>().namePrefix = customerModel.avatarName;
        customerModel.customerView = this;



        //Go.defaultUpdateType = GoUpdateType.FixedUpdate;

        //GoTweenConfig hoverTweenConfig = new GoTweenConfig().localPosition(new Vector3(0f, 10f, 0f),true).startPaused();
        //hoverTween = new GoTween(gameObject.transform, 0.1f, hoverTweenConfig);
        //hoverTween.autoRemoveOnComplete = false;
        //Go.addTween(hoverTween);

	}


    public void CustomerDroppedInZone(Zone zone)
    {

        GameObject feedbackIcon = Instantiate(God.instance.feedbackIconPrefab, transform.position, Quaternion.identity) as GameObject;
        Customer.ZoneMatchingResults result = customerModel.DroppedInZone(zone);
        if (result == Customer.ZoneMatchingResults.Fail)
        {
            feedbackIcon.GetComponent<FeedbackIcon>().icon = FeedbackIcon.Icons.Fail;
            GetComponent<UIDragDropItem>().enabled = false;
            customerModel.Die();
            Go.to(gameObject.transform, .5f, new GoTweenConfig().scale(0f).onComplete(DestroyCustomerView));
            return;
        }
        else if (result == Customer.ZoneMatchingResults.SecondBest)
        {
            feedbackIcon.GetComponent<FeedbackIcon>().icon = FeedbackIcon.Icons.SecondBestOption;
        }
        else
        {
            feedbackIcon.GetComponent<FeedbackIcon>().icon = FeedbackIcon.Icons.BestOption;
        }

        GetComponent<UIDragDropItem>().enabled = false;
        zone.AddCustomer(customerModel);
        Go.to(gameObject.transform, .5f, new GoTweenConfig().scale(0f).onComplete(DestroyCustomerView));
    }


    public void StartDrag() 
    {
        God.instance.FadeZones(true);
    }


    public void EndDrag()
    {
        God.instance.FadeZones(false);
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


    public void DestroyCustomerView(AbstractGoTween obj)
    {
        GetComponent<CustomSpriteAnimation>().enabled = false;
        Destroy(gameObject);
        Debug.Log("THIS WAS CALLED");
    }


	void OnClick(){ 
        God.instance.customerPanelManager.Display(customerModel);
	}

	void OnMouseUp(){
	}

	void OnRelease(){
	}



    //void SetToWaiting() {
    //    customerModel.waiting = true;
    //    collider.enabled = true;
    //}

	
	void TweenToPosition(Vector3 startPos, Vector3 endPos, float duration, float delay, EventDelegate[] onFinished ) {
		tweener.from = startPos;
		tweener.to = endPos; 
		tweener.duration = duration;
		tweener.delay = delay;
		tweener.ResetToBeginning();
		tweener.Play();
		foreach (EventDelegate currentDelegate in onFinished) {
			tweener.AddOnFinished(currentDelegate);
		}
	}

	void WalkToRandomSpot ( ) {
		EventDelegate newDelegate = new EventDelegate();
		newDelegate.target = this;
		newDelegate.methodName = "WalkToRandomSpot";
		newDelegate.oneShot = true;
		tweener.SetOnFinished(newDelegate);

        Debug.Log("SPOT ZERO POS: " + spots[0].transform.localPosition);
        Debug.Log("INVERSE SPOT POS: " + transform.parent.InverseTransformPoint( spots[0].transform.position ));

		TweenToPosition (this.transform.localPosition,  transform.parent.InverseTransformPoint( spots [Random.Range (0, spots.Length)].transform.position   ), 1f, 1f, new EventDelegate[] { newDelegate});
	}


	// Update is called once per frame
	void FixedUpdate () {
	
	}

    void LateUpdate()
    {
    }
}
