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

        if (surface != null)
        {
            if (surface.GetComponent<ZoneView>() == null)
            {
                surface = null;
            }
            else
            {
                Zone possibleZone = surface.GetComponent<ZoneView>().zoneModel;
                if (possibleZone && !possibleZone.queueOpen)
                //if (!surface.GetComponent<Zone>().queueOpen)
                //If queue is full, send the customer object back.
                {
                    surface = null;
                }

            }
            
            //else if (possibleZone)
            //{
            //    possibleZone.zoneViews[0].OnDrop(gameObject);
            //    return;
            //}
        }
        base.OnDragDropRelease(surface);

        dragging = false;
        customerView.EndDrag();

        
    }
    protected override void OnDragDropStart()
    {
        dragging = true;
        customerView.StartDrag();
        base.OnDragDropStart();
    }


}
