using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class uiclicktest : MonoBehaviour
{
    private Vector2 lastTapPos;
    private Vector3 theTarStartPosition;
    private Vector3 delta;
    private Vector3 startPos;

    public Transform timerCircle;
    [SerializeField]bool isParent;
    [SerializeField] int index = 0;
    public bool isOver;

    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;
    float distanceTravelled;

    [SerializeField]Camera cam;

    Vector3 circleStartPos, circleStartRot;
    // Start is called before the first frame update
    void Start()
    {
        //transform.parent.GetComponentInChildren<Image>().alphaHitTestMinimumThreshold = 0.001f;
        if (pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            pathCreator.pathUpdated += OnPathChanged;
            
        }
        circleStartPos = transform.parent.localPosition;
        circleStartRot = transform.parent.localEulerAngles;
        //StartCoroutine(setPosition());
        
        //StartCoroutine(CircleTimer());
    }
    private void OnEnable()
    {
        timerCircle.localScale = Vector3.one * 2.5f;

        transform.parent.position = pathCreator.path.GetClosestPointOnPath(pathCreator.path.GetPoint(0));
        transform.parent.LookAt(pathCreator.path.GetClosestPointOnPath(pathCreator.path.GetPoint(1)));
        StartCoroutine(CircleTimer());
    }
    // Update is called once per frame
    void Update()
    {
        TouchControl();
        //print(pathCreator.path.GetClosestPointOnPath(Input.mousePosition));
        //print(cam.WorldToScreenPoint(pathCreator.path.GetPoint(0)));
        //print(pathCreator.path.GetClosestPointOnPath(cam.ScreenToWorldPoint(Input.mousePosition)));
        
    }
    IEnumerator CircleTimer()
    {
        while(timerCircle.localScale.x > 1f)
        {
            timerCircle.localScale -= Vector3.one * Time.deltaTime * (1 / Time.timeScale) * .2f;
            yield return new WaitForEndOfFrame();
        }
        if (GameManager.singleton.timeState.Equals(TimeState.SlowMo))
        {
            GameManager.singleton.swipeState = SwipeState.Failed;
            GameManager.singleton.EndSlowMo();
            GetComponentInParent<SwipeHolder>().gameObject.SetActive(false);
        }
    }
    IEnumerator setPosition()
    {
        while (true)
        {
            
            //transform.localPosition = GetComponentInParent<SpriteShapeController>().spline.GetPosition(index);
            yield return new WaitForSeconds(.1f);
        }

    }
    private void OnMouseOver()
    {
        if (isParent)
        {
            //print("Over");
            isOver = true;
        }
            
        if (!isParent)
        {
            //print("over");
            isOver = true;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                
                //print("Yess");
            }
        }

    }
    private void OnMouseExit()
    {
        isOver = false;
    }
    private void OnMouseDrag()
    {
        if (pathCreator != null)
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
            if (Vector3.Distance(transform.parent.position, pathCreator.path.GetClosestPointOnPath(transform.parent.position - (delta * .5f))) < .5f)
            {
                Vector3 mt = pathCreator.path.GetClosestPointOnPath(transform.parent.position - (delta * .2f)) - transform.parent.position;
                transform.parent.LookAt(transform.parent.position + mt);
                transform.parent.position = pathCreator.path.GetClosestPointOnPath(transform.parent.position + (mt * 6));

                //print(pathCreator.path.NumPoints);
            }
            if((transform.parent.position - pathCreator.path.GetClosestPointOnPath(pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1))).magnitude < .1f)
            {
                GameManager.singleton.swipeState = SwipeState.Passed;
                GameManager.singleton.EndSlowMo();
                GetComponentInParent<SwipeHolder>().gameObject.SetActive(false);
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
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.parent.position);
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
