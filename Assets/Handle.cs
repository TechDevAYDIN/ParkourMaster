using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Handle : MonoBehaviour
{
    public bool canControl;
    private Vector2 lastTapPos;
    private Vector3 delta;

    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;

    Transform outline;
    public float givenTime = 2;
    public Vector3 startPosition;
    float distanceTravelled;

    ShapeScript shapeScript;
    [SerializeField] Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        outline = transform.GetChild(0);
        outline.localScale = Vector3.one * (2.5f + givenTime);
        shapeScript = GetComponentInParent<ShapeScript>();
        if (pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            pathCreator.pathUpdated += OnPathChanged;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canControl)
        {
            outline.localScale = outline.localScale - (Vector3.one * Time.unscaledDeltaTime);
            if(outline.localScale.x < 2.5f)
            {
                canControl = false;
                shapeScript.ShapeFailed();
            }
        }
        TouchControl();
    }
    public void Calibrate()
    {
        outline = transform.GetChild(0);
        transform.localPosition = startPosition;
        outline.localScale = Vector3.one * (2.5f + givenTime);
    }
    private void OnMouseDrag()
    {
        if (pathCreator != null && canControl)
        {
            Vector2 curTapPos = cam.ScreenToWorldPoint(Input.mousePosition);
            if (lastTapPos == Vector2.zero)
                lastTapPos = curTapPos;
            delta = lastTapPos - curTapPos;
            lastTapPos = curTapPos;

            float minDistance = float.MaxValue;
            int minIndex = 0;
            for (int i = 0; i < pathCreator.path.NumPoints; i++)
            {
                float distance = Vector2.Distance(curTapPos, pathCreator.path.GetNormal(i));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minIndex = i;
                }
            }
            pathCreator.path.GetClosestPointOnPath(curTapPos);
            //transform.parent.position = pathCreator.path.GetClosestPointOnPath(transform.parent.position -delta); 


            if (Vector3.Distance(transform.position, pathCreator.path.GetClosestPointOnPath(transform.position - (delta * 1f))) < 1f)
            {                
                Vector3 mt = pathCreator.path.GetClosestPointOnPath(transform.position - (delta * .8f)) - transform.position;

                //print(pathCreator.path.GetClosestDistanceAlongPath(transform.position));

                //transform.LookAt(transform.position + mt);

                transform.position = pathCreator.path.GetClosestPointOnPath(transform.position + (mt * 1.2f));
                
                /*if (pathCreator.path.GetClosestDistanceAlongPath(transform.position) < pathCreator.path.GetClosestDistanceAlongPath(transform.position + (mt * 1.2f)))
                    transform.position = pathCreator.path.GetClosestPointOnPath(transform.position + (mt * 1.2f));*/

                //print(pathCreator.path.NumPoints);
            }
            if ((transform.position - pathCreator.path.GetClosestPointOnPath(pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1))).magnitude < .1f)
            {
                canControl = false;
                shapeScript.ShapePassed();
                /*
                GameManager.singleton.swipeState = SwipeState.Passed;
                GameManager.singleton.EndSlowMo();
                GetComponentInParent<SwipeHolder>().gameObject.SetActive(false);*/
            }

            distanceTravelled += 10 * Time.deltaTime;
            pathCreator.path.GetClosestDistanceAlongPath(Input.mousePosition);
            //Input.mousePosition
            /*
            if (Vector3.Distance(transform.parent.position, pathCreator.path.GetClosestPointOnPath(transform.parent.position - delta)) < .75f)
                transform.parent.position = pathCreator.path.GetDirectionAtDistance(Vector3.Magnitude(cam.ScreenToWorldPoint(Input.mousePosition)) * 1.3f, endOfPathInstruction);*/

            //transform.parent.position = pathCreator.path.GetClosestPointOnPath(transform.parent.position - delta * 1.3f);
            //pathCreator.path.GetPoint(pathCreator.path.GetClosestPointOnPath(transform.parent.position));
            //pathCreator.path.GetClosestDistanceAlongPath(transform.parent.position);
            ////print(pathCreator.path.GetClosestDistanceAlongPath(pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1)));
            /*
            if (Vector3.Distance(transform.parent.position, pathCreator.path.GetClosestPointOnPath(cam.ScreenToWorldPoint(Input.mousePosition))) < 1f)
                transform.parent.position = pathCreator.path.GetClosestPointOnPath(cam.ScreenToWorldPoint(Input.mousePosition));
            */
            //transform.parent.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        }
        /*
        if (pathCreator.GetComponentInParent<uiclicktest>().isOver)
        {

            if (pathCreator != null)
            {
                distanceTravelled += 15 * Time.deltaTime;
                pathCreator.path.GetClosestDistanceAlongPath(Input.mousePosition);
                //Input.mousePosition
                transform.parent.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                //transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }*/
        //print(pathCreator.path.GetPoint(0));
    }

    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
    private void TouchControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastTapPos = Vector2.zero;
            delta = Vector3.zero;
        }
        if (Input.GetMouseButton(0))
        {

        }
        if (Input.GetMouseButtonUp(0))
        {
            lastTapPos = Vector3.zero;
        }
    }
}
