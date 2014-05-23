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
	void Start () {
		customerModel = GetComponent<Customer> ();
		spots = GameObject.FindGameObjectsWithTag("spot");
//		god = GameObject.Find("God").GetComponent<God>();
//		god.Zones[1].GetComponent<UIPlayAnimation>().Play(true);
		//tweener = GetComponent<TweenPosition>();
		tweener = (TweenPosition) gameObject.AddComponent ("TweenPosition");
		Dragger = GetComponent<UIDragObject>();
		CustomerEnters();
		
		
	}

	void OnPress(bool isDown) {
		if (isDown){
			beingDragged = true;
            dragStartTime = Time.time;
			dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			foreach (GameObject zone in God.instance.Zones){
				zone.GetComponent<UIPlayAnimation>().Play(true);
			}
		
		}
		else{
			beingDragged = false;
			foreach (GameObject zone in God.instance.Zones){
				zone.GetComponent<UIPlayAnimation>().Play(false);
			}
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
                        customerModel.ChangeZone(hit.collider.gameObject.GetComponent<Zone>());
                        hitAnotherZone = true;
                    }
                }

                if (!hitAnotherZone)
                {
                    WalkToRandomSpot();
                    customerModel.ChangeZone(God.instance.Entrance.GetComponent<Zone>());
                }


            }

						
		}
	}

	void OnClick(){ 
		Debug.Log ("RANDOMWALKING");
		WalkToRandomSpot();
	}

	void OnMouseUp(){
	}

	void OnRelease(){
	}

	void OnDrag(){
		tweener.enabled = false;
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
		EventDelegate walkDelegate = new EventDelegate();
		walkDelegate.target = this;
		walkDelegate.methodName = "WalkToRandomSpot";
		walkDelegate.oneShot = true;
		tweener.SetOnFinished(walkDelegate);
		EventDelegate setWaitingDelegate = new EventDelegate();
		setWaitingDelegate.target = this;
		setWaitingDelegate.methodName = "SetToWaiting";
		setWaitingDelegate.oneShot = true;
		tweener.AddOnFinished (setWaitingDelegate);
		TweenToPosition (EntranceStart.transform.localPosition, EntranceEnd.transform.localPosition, 1f, 0f, new EventDelegate[] { walkDelegate, setWaitingDelegate });

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
		TweenToPosition (this.transform.localPosition, spots [Random.Range (0, spots.Length)].transform.localPosition, 1f, 1f, new EventDelegate[] { newDelegate});
	}


	// Update is called once per frame
	void Update () {
		if (beingDragged) {
			transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
		}
	}
}
