using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeScript : MonoBehaviour
{
    [SerializeField] Vector3 handleStartPos;
    Handle handle;
    Animator anim;
    private void OnEnable()
    {
        if(handle == null)
        {
            handle = transform.GetChild(0).GetComponentInChildren<Handle>();
            handle.startPosition = handleStartPos;
            handle.Calibrate();
        }
        else
        {
            handle.startPosition = handleStartPos;
            handle.Calibrate();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        handle = GetComponentInChildren<Handle>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShapeOpened()
    {
        handle.canControl = true;
    }
    public void ShapePassed()
    {
        anim.SetTrigger("Passed");
        GameManager.singleton.swipeState = SwipeState.Passed;
        GameManager.singleton.EndSlowMo();
        PlayerController.singleton.SwipePass();
    }
    public void ShapeFailed()
    {
        anim.SetTrigger("Failed");
        GameManager.singleton.swipeState = SwipeState.Failed;
        GameManager.singleton.EndSlowMo();
    }
    public void CloseShape()
    {
        gameObject.SetActive(false);
    }
}
