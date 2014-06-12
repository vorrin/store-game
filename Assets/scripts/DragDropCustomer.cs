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

    protected override void OnDragDropMove(Vector3 delta)
    {
        print("Drag drag dragging");
        base.OnDragDropMove(delta);
        Vector3 inputPosition;
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            inputPosition = Input.GetTouch(0).position;
        }
        else
        {
            inputPosition = Input.mousePosition;
        }

        GameObject hoveredObject;
        RaycastHit lastHit;
        UICamera cam = UICamera.mainCamera.GetComponent<UICamera>();
        Camera mainCam = UICamera.mainCamera;
        Ray ray = mainCam.ScreenPointToRay(inputPosition);
        float dist = (cam.rangeDistance > 0f) ? cam.rangeDistance : mainCam.farClipPlane - mainCam.nearClipPlane;
        int mask = mainCam.cullingMask & (int)cam.eventReceiverMask; // NOT NEEDED WE GOT ONLY ZONES
        RaycastHit[] hits = Physics.RaycastAll(ray, dist, 1 << LayerMask.NameToLayer("Zone"));
        foreach (RaycastHit hit in hits)
        {

            ZoneView view = hit.collider.GetComponent<ZoneView>();
            //if (!view)
            //{
            //    Debug.Log("CONTINUINGNGGG");
            //    continue;
            //}
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

            //if (!hoveredPreviously.Contains(hit.collider.GetComponent<ZoneView>()))
            //{
            //    view.OnCustomHover(true);
            //}
            //hoveredCurrently.Add(view);

        }

    }


    protected override void OnDragDropStart()
    {
        dragging = true;
        customerView.StartDrag();
        base.OnDragDropStart();
    }


}
