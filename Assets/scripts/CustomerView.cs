using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerView : MonoBehaviour {

	public Customer customerModel;
	public GameObject[] spots;
	private TweenPosition tweener;
	private UIDragObject Dragger;
	private bool beingDragged = false; 
	private Vector3 dragOffset = Vector3.zero;
	public GameObject EntranceStart;
	public GameObject EntranceEnd;
    public float dragStartTime; 


	delegate void MyDelegate();
	MyDelegate myDelegate;
//	private God god;

	// Use this for initialization
	public void Create (Customer customer) {
        customerModel = customer;
        GetComponent<CustomSpriteAnimation>().namePrefix = customerModel.avatarName;
        //EntranceStart = God.instance.entranceStart;
        //EntranceEnd = God.instance.entranceEnd;
        //spots = GameObject.FindGameObjectsWithTag("spot");
        //GetComponent<CustomSpriteAnimation>().namePrefix = customer.avatarName;
//		god = GameObject.Find("God").GetComponent<God>();
//		god.Zones[1].GetComponent<UIPlayAnimation>().Play(true);
		//tweener = GetComponent<TweenPosition>();
        //tweener = (TweenPosition) gameObject.AddComponent ("TweenPosition");
        //Dragger = GetComponent<UIDragObject>();
		CustomerEnters();

	}

	void OnPress(bool isDown) {
        //return;
		if (isDown){
            dragStartTime = Time.time;
			dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            God.instance.FadeZones(true);
		
		}
		else{
			beingDragged = false;
            God.instance.FadeZones(false);

            if (Time.time - dragStartTime > 0.2f)
            {
                Ray cast = UICamera.mainCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));

                RaycastHit[] hits;
                hits = Physics.RaycastAll(cast);
                bool hitAnotherZone = false;
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.name.Contains("Zone"))
                    {
                        Zone currentZone = hit.collider.GetComponent<Zone>();
                        customerModel.ChangeZone(currentZone);
                        //User dropped in the Entrance zone again.
                        //if (currentZone != God.instance.entrance)
                        //{
                        //    Go.to(gameObject.transform, .5f, new GoTweenConfig().scale(0f).onComplete(DestroyCustomerView));
                        //}
                        //else
                        //{
                        //    WalkToRandomSpot();
                        //}
                        hitAnotherZone = true;
                    }
                }

                if (!hitAnotherZone)
                {
             //       WalkToRandomSpot();
           //         customerModel.ChangeZone(God.instance.entrance);
                }


            }

						
		}
	}


    private void DestroyCustomerView(AbstractGoTween obj)
    {
        GetComponent<CustomSpriteAnimation>().enabled = false;
        Debug.Log("THIS WAS CALLED");
     //   Destroy(gameObject);
    }


	void OnClick(){ 
		Debug.Log ("RANDOMWALKING");
        God.instance.customerPanelManager.Display(customerModel.avatarName, customerModel.scenario);
		//WalkToRandomSpot();
	}

	void OnMouseUp(){
	}

	void OnRelease(){
	}

	void OnDrag(){
//		tweener.enabled = false;
	}



	void SetToWaiting() {
		Debug.Log ("setting to waiting");
		customerModel.waiting = true;
        collider.enabled = true;
	}

	void CustomerEnters () {
//		WalkToRandomSpot();
//		myDelegate = SetToWaiting;
//		WalkToRandomSpot (EntranceStart.transform.position, EntranceEnd.transform.position, myDelegate);
//		tweener.from = EntranceStart.transform.localPosition;
//		tweener.to = EntranceEnd.transform.localPosition; 
//		tweener.duration = 1f;
//		tweener.ResetToBeginning();
//		tweener.Play();
        //collider.enabled = false;

        SetToWaiting();
        return; //Not happening anymore
        //EventDelegate walkDelegate = new EventDelegate();
        //walkDelegate.target = this;
        //walkDelegate.methodName = "WalkToRandomSpot";
        //walkDelegate.oneShot = true;
        //tweener.SetOnFinished(walkDelegate);
        //EventDelegate setWaitingDelegate = new EventDelegate();
        //setWaitingDelegate.target = this;
        //setWaitingDelegate.methodName = "SetToWaiting";
        //setWaitingDelegate.oneShot = true;
        //tweener.AddOnFinished (setWaitingDelegate);
        //TweenToPosition (transform.parent.InverseTransformPoint(EntranceStart.transform.position),
        //    transform.parent.InverseTransformPoint(EntranceEnd.transform.position), 
        //    1f, 0f, new EventDelegate[] { walkDelegate, setWaitingDelegate });
	}

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
