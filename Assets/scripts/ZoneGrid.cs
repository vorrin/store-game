using UnityEngine;
using System.Collections;

public class ZoneGrid : UIGrid {

    public ZonePanelManager zonePanelManager;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    private int SortQueue(Transform a, Transform b)
    {
        int aIndex = 0;
        int bIndex = 0;
        for (int i = 0; i < a.parent.childCount; i++ ){
            Transform currentTransform = a.parent.GetChild(i);
            if (currentTransform == a)
            {
                aIndex = i;
            }
            else if (currentTransform == b)
            {
                bIndex = i;
            }
       }
        if (aIndex > bIndex)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public override void Reposition()
    {

        if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
        {
            mReposition = true;
            return;
        }

        if (!mInitDone) Init();

        mReposition = false;
        Transform myTrans = transform;

        int x = 0;
        int y = 0;
        int maxX = 0;
        int maxY = 0;

        if (sorting != Sorting.None )
        {


            BetterList<Transform> spotsList = new BetterList<Transform>();

            BetterList<Transform> actualItems = new BetterList<Transform>();

            foreach (Transform childTransform in transform)
            {
                if (childTransform.gameObject.name.Contains("Spot"))
                {

                    //spotsList.Insert(int.Parse(childTransform.gameObject.name.Substring(4, 2) ) - 1, childTransform);
                    spotsList.Add(childTransform);
                }
                else
                {
                    if (childTransform && (!hideInactive || NGUITools.GetActive(childTransform.gameObject))) actualItems.Add(childTransform);
                }
            }


            //Debug.Log(spotsList);   

            //for (int i = 0; i < myTrans.childCount; ++i)
            //{
                
            //    Transform t = myTrans.GetChild(i);
               
            //}

          //SortHorizontal (Transform a, Transform b) { return a.localPosition.x.CompareTo(b.localPosition.x); }

            actualItems.Sort(SortVertical);
            spotsList.Sort(SortVertical);

            //actualItems.Sort(SortQueue);
            
            
            
            
            //if (sorting == Sorting.Alphabetic) list.Sort(SortByName);
            //else if (sorting == Sorting.Horizontal) list.Sort(SortHorizontal);
            //else if (sorting == Sorting.Vertical) list.Sort(SortVertical);
            //else Sort(list);

            //list.Sort({ return list. })

            for (int i = 0, imax = actualItems.size; i < imax; ++i)
            {
                //Transform t = actualItems[actualItems.size - 1 - i];

                Transform t = actualItems[ i];

                if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;

                float depth = t.localPosition.z;
                //Debug.Log("POSITION NOW ");
                //Vector3 pos = t.parent.InverseTransformPoint(list[i].position);
                Vector3 pos = spotsList[i].localPosition;
                //Vector3 pos = (arrangement == Arrangement.Horizontal) ?
                //    new Vector3(cellWidth * x, -cellHeight * y, depth) :
                //    new Vector3(cellWidth * y, -cellHeight * x, depth);

                if (animateSmoothly && Application.isPlaying)
                {
                    SpringPosition.Begin(t.gameObject, pos, 15f).updateScrollView = true;
                }
                else t.localPosition = pos;

                maxX = Mathf.Max(maxX, x);
                maxY = Mathf.Max(maxY, y);

                if (++x >= maxPerLine && maxPerLine > 0)
                {
                    x = 0;
                    ++y;
                }
            }

            zonePanelManager.ReorderCustomerList(actualItems);
            //print(" THE LIST IS SIZED SO : " + list.size);
        }
        else
        {
            for (int i = 0; i < myTrans.childCount; ++i)
            {
                Transform t = myTrans.GetChild(i);

                if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;

                float depth = t.localPosition.z;
                Vector3 pos = (arrangement == Arrangement.Horizontal) ?
                    new Vector3(cellWidth * x, -cellHeight * y, depth) :
                    new Vector3(cellWidth * y, -cellHeight * x, depth);

                if (animateSmoothly && Application.isPlaying)
                {
                    SpringPosition.Begin(t.gameObject, pos, 15f).updateScrollView = true;
                }
                else t.localPosition = pos;

                maxX = Mathf.Max(maxX, x);
                maxY = Mathf.Max(maxY, y);

                if (++x >= maxPerLine && maxPerLine > 0)
                {
                    x = 0;
                    ++y;
                }
            }
        }

        // Apply the origin offset
        if (pivot != UIWidget.Pivot.TopLeft)
        {
            Vector2 po = NGUIMath.GetPivotOffset(pivot);

            float fx, fy;

            if (arrangement == Arrangement.Horizontal)
            {
                fx = Mathf.Lerp(0f, maxX * cellWidth, po.x);
                fy = Mathf.Lerp(-maxY * cellHeight, 0f, po.y);
            }
            else
            {
                fx = Mathf.Lerp(0f, maxY * cellWidth, po.x);
                fy = Mathf.Lerp(-maxX * cellHeight, 0f, po.y);
            }

            for (int i = 0; i < myTrans.childCount; ++i)
            {
                Transform t = myTrans.GetChild(i);

                if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;

                SpringPosition sp = t.GetComponent<SpringPosition>();

                if (sp != null)
                {
                    sp.target.x -= fx;
                    sp.target.y -= fy;
                }
                else
                {
                    Vector3 pos = t.localPosition;
                    pos.x -= fx;
                    pos.y -= fy;
                    t.localPosition = pos;
                }
            }
        }

        if (keepWithinPanel && mPanel != null)
            mPanel.ConstrainTargetToBounds(myTrans, true);

        if (onReposition != null)
            onReposition();
    }
}
