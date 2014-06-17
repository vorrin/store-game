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
        RaycastHit[] hits = RaycastForZones();
        if (hits.Length >0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.name == "Fader")
                {
                    return;
                }
            }
            ZoneView possibleZoneView = hits[0].collider.GetComponent<ZoneView>();
            possibleZoneView.OnCustomDrop(gameObject);
        }
        else
        {
            AudioManager.instance.CustomerReleased();
        }


        
    }

    RaycastHit[] RaycastForZones()
    {
        Vector3 inputPosition;
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            inputPosition = Input.GetTouch(0).position;
        }
        else
        {
            inputPosition = Input.mousePosition;
        }

        UICamera cam = UICamera.mainCamera.GetComponent<UICamera>();
        Camera mainCam = UICamera.mainCamera;
        Ray ray = mainCam.ScreenPointToRay(inputPosition);
        float dist = (cam.rangeDistance > 0f) ? cam.rangeDistance : mainCam.farClipPlane - mainCam.nearClipPlane;
        int mask = mainCam.cullingMask & (int)cam.eventReceiverMask; // NOT NEEDED WE GOT ONLY ZONES
        RaycastHit[] hits = Physics.RaycastAll(ray, dist, 1 << LayerMask.NameToLayer("Zone"));
        return hits;

    }

    protected override void OnDragDropMove(Vector3 delta)
    {
        //Fades zones in when dragging customer.
        base.OnDragDropMove(delta);


        RaycastHit[] hits = RaycastForZones();
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.name == "Fader")
            {
                return;
            }
        }

        foreach (RaycastHit hit in hits)
        {
            ZoneView view = hit.collider.GetComponent<ZoneView>();
            view.zoneModel.zoneViews.ForEach(zonView =>
            {
                zonView.GetComponent<UIPlayAnimation>().Play(true);
            });
            foreach (Zone zone in God.instance.zones)
            {
                if (zone != view.zoneModel)
                {
                    zone.zoneViews.ForEach(zonView =>
                    {
                        zonView.GetComponent<UIPlayAnimation>().Play(false);
                    });
                }
            }

        }

    }


    protected override void OnDragDropStart()
    {
        dragging = true;
        customerView.StartDrag();
        base.OnDragDropStart();
    }


}
