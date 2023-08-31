using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    #region SINGLETON
    public static GameManager Singleton;
    public static GameManager singleton
    {
        get
        {
            if (Singleton == null)
            {
                Singleton = GameObject.FindObjectOfType<GameManager>();

                if (Singleton == null)
                {
                    GameObject container = new GameObject("GM");
                    Singleton = container.AddComponent<GameManager>();
                }
            }
            return Singleton;
        }
    }
    #endregion
    public SwipeState swipeState = SwipeState.Closed;
    public TimeState timeState = TimeState.Normal;
    public GameObject tapButton;
    public CinemachineVirtualCamera vCam;
    [SerializeField] Camera depthCam;
    [SerializeField] List<GameObject> drawShapes;
    [SerializeField] Vector3 cameraBaseOffset, cameraBaseAim;
    TimeManager timeManager;
    [SerializeField] List<GameObject> clickEffects;
    [SerializeField] MeshTrailTest trailTest;
    [SerializeField] MainLevelDesigner levelDesigner;
    [SerializeField] AIController mrBot;
    [SerializeField] BoxCollider[] goals;
    [SerializeField] GameObject passedScreen, startScreen;

    public bool isGameStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        timeManager = GetComponent<TimeManager>();
        cameraBaseOffset = vCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        StartCoroutine(ReleaseMem());
        levelDesigner.LoadLevel(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(47.0339f + 11.8022f * Mathf.Log(PlayerController.singleton.runSpeed) > 60 &&
            47.0339f + 11.8022f * Mathf.Log(PlayerController.singleton.runSpeed) < 75)
        {
            vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, 47.0339f + 11.8022f * Mathf.Log(PlayerController.singleton.runSpeed), Time.unscaledDeltaTime * 8);
        }
            
    }
    public void ChangeCameraPos(Vector3 pos, Vector3 aim)
    {
        if (pos == Vector3.zero)
        {
            StartCoroutine(AdjustCamera(cameraBaseOffset, cameraBaseAim));

        }
        else
        {
            StartCoroutine(AdjustCamera(pos, aim));
        }        
    }
    public void Completed()
    {
        goals[0].enabled = false;
        goals[1].enabled = false;
        tapButton.SetActive(false);
        PlayerController.singleton.GoalReached();
        mrBot.GoalReached();
        passedScreen.SetActive(true);
    }
    IEnumerator ReleaseMem()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
    IEnumerator AdjustCamera(Vector3 position, Vector3 aim)
    {
        var transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
        var composer = vCam.GetCinemachineComponent<CinemachineComposer>();
        while (true)
        {
            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, position, Time.unscaledDeltaTime * 5);
            //composer.m_TrackedObjectOffset = Vector3.Lerp(composer.m_TrackedObjectOffset, aim, Time.deltaTime * 6);
            if (Vector3.Distance(position, transposer.m_FollowOffset) < .1f)
                break;
            yield return new WaitForEndOfFrame();
        }
    }
    public void StartSlowMo()
    {
        if (swipeState.Equals(SwipeState.Closed) && timeState.Equals(TimeState.Normal))
        {
            timeState = TimeState.SlowMo;
            StartCoroutine(timeManager.SlowDownTime(0.025f, 0.5f));
            tapButton.SetActive(false);
            GetDrawShape();
        }
    }
    public void EndSlowMo()
    {
        tapButton.SetActive(true);
        timeState = TimeState.Normal;
        StartCoroutine(timeManager.FastUpTime(0.5f));
    }
    void GetDrawShape()
    {
        drawShapes.RemoveAll(i => i == null);
        GameObject obj = null;
        while (drawShapes.Any(g => g.gameObject.activeSelf == false))
        {
            int index = Random.Range(0, drawShapes.Count);

            if (drawShapes[index] == null || drawShapes[index].activeSelf) continue;
            if (!drawShapes[index].gameObject.activeSelf)
            {
                obj = drawShapes[index];
                if (obj) break;
            }
        }
        if (obj != null)
        {
            obj.gameObject.SetActive(true);
            //return obj.gameObject;
        }
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(0);
    }
    public void ClickEffect()
    {
        if (!isGameStarted)
        {
            startScreen.SetActive(false);
            isGameStarted = true;
            mrBot.StartRunning();
        }
        clickEffects.RemoveAll(i => i == null);
        GameObject obj = null;
        trailTest.PrintOne();
        while (clickEffects.Any(g => g.gameObject.activeSelf == false))
        {
            int index = Random.Range(0, clickEffects.Count);

            if (clickEffects[index] == null || clickEffects[index].activeSelf) continue;
            if (!clickEffects[index].gameObject.activeSelf)
            {
                obj = clickEffects[index];
                break;
            }
        }
        if (obj != null)
        {
            Vector2 tempPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(obj.GetComponentInParent<RectTransform>(), Input.mousePosition, depthCam, out tempPos);
            obj.GetComponent<RectTransform>().anchoredPosition = tempPos; 
            obj.gameObject.SetActive(true);
        }
    }
}
#region Enums
public enum SwipeState
{
    Closed,
    Opened,
    Passed,
    Failed,
    other
}
public enum TimeState
{
    Normal,
    SlowMo,
    other
}
public enum PlayerState
{
    Wait,
    Running,
    Animation,
    Dead,
    other
}
public enum AIState
{
    Wait,
    Running,
    Animation,
    Dead,
    other
}
#endregion