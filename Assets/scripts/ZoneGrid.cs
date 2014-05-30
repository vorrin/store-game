using UnityEngine;
using System.Collections;

public class ZoneGrid : UIGrid {

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    //Transform SortQueue(Transform a, Transform b)
    //{
    //    return transform.parent.
    //}

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
            BetterList<Transform> list = new BetterList<Transform>();
            foreach (Transform childTransform in transform)
            {
                if (childTransform.gameObject.name == "Spot")
                {
                    list.Add(childTransform);
                }
            }

            BetterList<Transform> actualItems = new BetterList<Transform>();

            for (int i = 0; i < myTrans.childCount; ++i)
            {
                Transform t = myTrans.GetChild(i);
                if (t && (!hideInactive || NGUITools.GetActive(t.gameObject))) actualItems.Add(t);
            }

          //  SortHorizontal (Transform a, Transform b) { return a.localPosition.x.CompareTo(b.localPosition.x); }

            actualItems.Sort(SortVertical);
            //if (sorting == Sorting.Alphabetic) list.Sort(SortByName);
            //else if (sorting == Sorting.Horizontal) list.Sort(SortHorizontal);
            //else if (sorting == Sorting.Vertical) list.Sort(SortVertical);
            //else Sort(list);

            //list.Sort({ return list. })

            for (int i = 0, imax = actualItems.size; i < imax; ++i)
            {
                Transform t = actualItems[i];

                if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;

                float depth = t.localPosition.z;
                Debug.Log("POSITION NOW ");
                //Vector3 pos = t.parent.InverseTransformPoint(list[i].position);
                Vector3 pos = list[i].localPosition;
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
