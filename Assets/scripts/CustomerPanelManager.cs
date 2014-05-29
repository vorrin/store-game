using UnityEngine;
using System.Collections;

public class CustomerPanelManager : MonoBehaviour {
    public UISprite avatarWindow;
    public UILabel scenarioLabel;
    public Customer currentCustomer;


    void Start()
    {
        gameObject.SetActiveRecursively(false);           
    }

    public void Display(Customer customer)
    {
   //     gameObject.SetActiveRecursively(true);
        //DEBUG tmp hack
        GetComponent<UIPlayAnimation>().Play(true);

        currentCustomer = customer;
        avatarWindow.spriteName = customer.avatarName + "1" ;
        scenarioLabel.text = customer.scenario;
        GetComponentInChildren<UIScrollView>().ResetPosition();
        scenarioLabel.ResizeCollider();
    }

    public void Hide()
    {
        GetComponent<UIPlayAnimation>().Play(false);
        GetComponent<UIPlayAnimation>().disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterReverse;
    }


	// Update is called once per frame
	void Update () {
	
	}
}
