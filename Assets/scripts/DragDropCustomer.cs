using UnityEngine;
using System.Collections;

public class DragDropCustomer : UIDragDropItem
{
    public bool dragging;
    public CustomerView customerView;

	protected override void Start ()
    {
        customerView = GetComponent<CustomerView>();
        base.Start();
    }

    protected override void OnDragDropRelease(GameObject surface)
    {

        customerView.EndDrag();
        dragging = false;
        if (surface != null)
        {
            Zone possibleZone = surface.GetComponent<Zone>();
            if (possibleZone && !possibleZone.queueOpen)
            //if (!surface.GetComponent<Zone>().queueOpen)
            //If queue is full, send the customer object back.
            {
                surface = null;
            }
        }
        
        base.OnDragDropRelease(surface);
    }
    protected override void OnDragDropStart()
    {
        dragging = true;
        customerView.StartDrag();
        base.OnDragDropStart();
    }


}
