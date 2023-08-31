using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{   
    #region SINGLETON
    public static PlayerController SharedInstance;
    public static PlayerController singleton
    {
        get
        {
            if (SharedInstance == null)
            {
                SharedInstance = GameObject.FindObjectOfType<PlayerController>();

                if (SharedInstance == null)
                {
                    GameObject container = new GameObject("GM");
                    SharedInstance = container.AddComponent<PlayerController>();
                }
            }
            return SharedInstance;
        }
    }
    #endregion
    public PlayerState playerState = PlayerState.Wait;
    public float runSpeed = 2;
    public bool isMoving = true;
    Animator anim;
    [SerializeField] AnimatorOverrideController[] overrideController;
    [SerializeField] Rigidbody[] deathRigidBodies;
    Rigidbody mainRb;
    CapsuleCollider mainCollider;
    MeshTrailTest meshTrail;
    ObstaclesType obstacleType;
    Transform lastWaypoint;
    [SerializeField] ParticleSystem trailParticles;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        mainRb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<CapsuleCollider>();
        playerState = PlayerState.Wait;
        meshTrail = GetComponent<MeshTrailTest>();
        StartCoroutine(GroundCheck());
        StartCoroutine(SpeedControl());
    }

    // Update is called once per frame
    void Update()
    {
        CheckObstacles();
        if (playerState.Equals(PlayerState.Running))
        {
            transform.position += Vector3.forward * runSpeed * Time.deltaTime;
            anim.SetFloat("Blend",Mathf.Lerp(anim.GetFloat("Blend"),runSpeed, Time.unscaledDeltaTime * 10));
        }        
    }
    IEnumerator SpeedControl()
    {
        while (true)
        {
            if (runSpeed > 2)
            {
                runSpeed -= Time.fixedUnscaledDeltaTime * .15f + (runSpeed * .005f);
            }
            yield return new WaitForFixedUpdate();
        }
    }
    public void GoalReached()
    {
        EndAnim();
        playerState = PlayerState.Wait;
        anim.SetBool("Running",false);
    }
    public void JumpHalf(bool passed = true)
    {
        if (passed)
        {
            mainCollider.enabled = false;
        }
        else
        {
            mainCollider.enabled = true;
        }
        GameManager.singleton.ChangeCameraPos(new Vector3(4,2,-1), Vector3.zero);
        playerState = PlayerState.Animation;
        SelectRandomAnim();
        anim.SetBool("JumpHalf", true);
    }
    public void SlideDown(bool passed = true)
    {
        if (passed)
        {
            mainCollider.enabled = false;
        }
        else
        {
            mainCollider.enabled = true;
        }
        GameManager.singleton.ChangeCameraPos(new Vector3(5, 2, -2), Vector3.zero);
        playerState = PlayerState.Animation;
        anim.SetBool("Slide", true);
    }
    public void ClimbJump(bool passed = true)
    {
        if (passed)
        {
            mainCollider.enabled = false;
        }
        else
        {
            mainCollider.enabled = true;
        }
        GameManager.singleton.ChangeCameraPos(new Vector3(4, 2, -1), Vector3.zero);
        playerState = PlayerState.Animation;
        anim.SetBool("ClimbJump", true);
    }
    void StartAnim(bool passed = true, ObstaclesType obstacle = ObstaclesType.HalfWall)
    {
        if (passed)
        {
            mainCollider.enabled = false;
            meshTrail.isPrint = true;
            GameManager.singleton.ChangeCameraPos(new Vector3(4, 2, -1), Vector3.zero);
        }
        else
        {
            mainCollider.enabled = true;
        }
        trailParticles.Stop();
        playerState = PlayerState.Animation;
        GameManager.singleton.tapButton.SetActive(false);
        switch (obstacle)
        {
            case ObstaclesType.HalfWall:
                anim.SetBool("JumpHalf", true);
                break;
            case ObstaclesType.ClimbWall:
                anim.SetBool("ClimbJump", true);
                break;
            case ObstaclesType.DownWall:
                anim.SetBool("Slide", true);
                break;
            case ObstaclesType.SideWall:
                anim.SetBool("WallRun", true);
                break;
            case ObstaclesType.JumpCylinder:
                anim.SetBool("SwingJump", true);
                break;
            case ObstaclesType.TurningCircle:
                if (passed)
                {
                    RaycastHit hit;
                    bool isHit = Physics.BoxCast(transform.position + Vector3.up, Vector3.one, transform.forward, out hit, transform.rotation, 6, 1 << 12);
                    hit.collider.gameObject.GetComponent<Animator>().SetTrigger("Jump");
                }
                anim.SetBool("JumpOver", true);
                
                break;
        }
    }
    public void SpeedUp()
    {
        if (playerState.Equals(PlayerState.Wait))
        {
            playerState = PlayerState.Running;
            anim.SetBool("Running", true);
        }
        if(playerState.Equals(PlayerState.Running))
            if(runSpeed < 7)
                runSpeed += .8f;
    }
    void SelectRandomAnim()
    {
        int x = Random.Range(0, overrideController.Length);
        anim.runtimeAnimatorController = overrideController[x];
    }
    public void EndAnim()
    {
        GameManager.singleton.ChangeCameraPos(Vector3.zero, Vector3.zero);
        //anim.SetBool("JumpHalf", false);
        foreach(var p in anim.parameters)
        {
            if(p.type.Equals(AnimatorControllerParameterType.Bool))
                anim.SetBool(p.name, false);
        }
        anim.SetBool("Running", true);
        StartCoroutine(GroundCheck());
        playerState = PlayerState.Running;
        mainCollider.enabled = true;
        meshTrail.isPrint = false;
        trailParticles.Play();
        GameManager.singleton.swipeState = SwipeState.Closed;
        GameManager.singleton.tapButton.SetActive(true);
    }
    public void CheckObstacles()
    {
        float maxDistance = 10f;
        RaycastHit hit;
        bool isHit = Physics.BoxCast(transform.position + Vector3.up, Vector3.one, transform.forward, out hit, transform.rotation, maxDistance, 1 << 12);
        //obstacleType = hit.collider.GetComponent<Obstacles>().type;

        if (isHit && playerState.Equals(PlayerState.Running))
        {
            HitCheckObstacle(hit);
        }
    }
    public void HitCheckObstacle(RaycastHit hit)
    {
        switch (hit.collider.GetComponent<Obstacles>().obstacleType)
        {
            case ObstaclesType.HalfWall:
                if (Vector3.Distance(transform.position, hit.point) < 5f)
                {
                    GameManager.singleton.StartSlowMo();
                }
                if (Vector3.Distance(transform.position, hit.point) < 2.2f)
                {
                    if (playerState.Equals(PlayerState.Running))
                    {
                        if (GameManager.singleton.swipeState.Equals(SwipeState.Passed))
                        {
                            StartAnim(true, ObstaclesType.HalfWall);
                        }
                        
                        if (GameManager.singleton.swipeState.Equals(SwipeState.Failed))
                        {
                            StartAnim(false, ObstaclesType.HalfWall);
                        }
                    }
                }
                break;
            case ObstaclesType.ClimbWall:
                if (Vector3.Distance(transform.position, hit.point) < 6.5f)
                {
                    GameManager.singleton.StartSlowMo();
                }
                if (Vector3.Distance(transform.position, hit.point) < 3.25f)
                {
                    if (playerState.Equals(PlayerState.Running))
                    {
                        if (GameManager.singleton.swipeState.Equals(SwipeState.Passed))
                        {
                            StartAnim(true, ObstaclesType.ClimbWall);
                        }
                        if (GameManager.singleton.swipeState.Equals(SwipeState.Failed))
                        {
                            StartAnim(false, ObstaclesType.ClimbWall);
                        }
                    }
                }
                break;
            case ObstaclesType.DownWall:
                if (Vector3.Distance(transform.position, hit.point) < 6f)
                {
                    GameManager.singleton.StartSlowMo();
                }
                if (Vector3.Distance(transform.position, hit.point) < 3f)
                {
                    if (playerState.Equals(PlayerState.Running))
                    {
                        if (GameManager.singleton.swipeState.Equals(SwipeState.Passed))
                        {
                            StartAnim(true, ObstaclesType.DownWall);
                        }

                        if (GameManager.singleton.swipeState.Equals(SwipeState.Failed))
                        {
                            StartAnim(false, ObstaclesType.DownWall);
                        }
                    }
                }
                break;
            case ObstaclesType.SideWall:
                if (Vector3.Distance(transform.position, hit.point) < 5f)
                {
                    GameManager.singleton.StartSlowMo();
                }
                if (Vector3.Distance(transform.position, hit.point) < 2f)
                {
                    if (playerState.Equals(PlayerState.Running))
                    {
                        if (GameManager.singleton.swipeState.Equals(SwipeState.Passed))
                        {
                            StartAnim(true, ObstaclesType.SideWall);
                        }

                        if (GameManager.singleton.swipeState.Equals(SwipeState.Failed))
                        {
                            StartAnim(false, ObstaclesType.SideWall);
                        }
                    }
                }
                break;
            case ObstaclesType.TurningCircle:
                if (Vector3.Distance(transform.position, hit.point) < 8f)
                {
                    GameManager.singleton.StartSlowMo();
                }
                if (Vector3.Distance(transform.position, hit.point) < 4.5f)
                {
                    if (playerState.Equals(PlayerState.Running))
                    {
                        if (GameManager.singleton.swipeState.Equals(SwipeState.Passed))
                        {
                            StartAnim(true, ObstaclesType.TurningCircle);
                        }

                        if (GameManager.singleton.swipeState.Equals(SwipeState.Failed))
                        {
                            StartAnim(false, ObstaclesType.TurningCircle);
                        }
                    }
                }
                break;
            case ObstaclesType.JumpCylinder:
                if (Vector3.Distance(transform.position, hit.point) < 7f)
                {
                    GameManager.singleton.StartSlowMo();
                }
                if (Vector3.Distance(transform.position, hit.point) < 3.7f)
                {
                    if (playerState.Equals(PlayerState.Running))
                    {
                        if (GameManager.singleton.swipeState.Equals(SwipeState.Passed))
                        {
                            StartAnim(true, ObstaclesType.JumpCylinder);
                        }

                        if (GameManager.singleton.swipeState.Equals(SwipeState.Failed))
                        {
                            StartAnim(false, ObstaclesType.JumpCylinder);
                        }
                    }
                }
                break;
        }
    }
    public void Death(float size)
    {
        anim.enabled = false;
        playerState = PlayerState.Dead;
        mainCollider.enabled = false;
        mainRb.isKinematic = true;
        foreach(var r in deathRigidBodies)
        {
            r.isKinematic = false;
        }
        foreach (var r in deathRigidBodies)
        {
            r.AddForce(Vector3.forward * size + Vector3.up * (size *.25f));
        }
        StartCoroutine(Resurraction());
    }
    public void SwipePass()
    {
        RaycastHit hit;
        bool isHit = Physics.BoxCast(transform.position + Vector3.up, Vector3.one, transform.forward, out hit, transform.rotation, 6, 1 << 12);
        if(isHit)
            StartCoroutine(hit.collider.gameObject.GetComponent<Obstacles>().ChangeColor(Color.green));
    }
    IEnumerator GroundCheck()
    {           
        while (true)
        {
            RaycastHit hit;
            bool isHit = Physics.Raycast(transform.position + Vector3.up * 3, Vector3.down, out hit, 7f, 1 << 10);
            if (isHit)
            {
                if (hit.distance < 3)
                {
                    transform.position += Vector3.up * Time.deltaTime * (1 / Time.timeScale);
                    Physics.Raycast(transform.position + Vector3.up * 3, Vector3.down, out hit, 7f, 1 << 10);
                    if (hit.distance >= 3)
                    {
                        break;
                    }
                }
                if (hit.distance > 3)
                {
                    transform.position += Vector3.down * Time.deltaTime * (1 / Time.timeScale);
                    Physics.Raycast(transform.position + Vector3.up * 3, Vector3.down, out hit, 7f, 1 << 10);
                    if (hit.distance <= 3)
                    {
                        break;
                    }
                }
                if (hit.distance == 3)
                {
                    break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator Resurraction()
    {
        yield return new WaitForSeconds(2.5f);
        EndAnim();
        transform.position = lastWaypoint.position;
        foreach (var r in deathRigidBodies)
        {
            r.isKinematic = true;
        }
        anim.enabled = true;
        playerState = PlayerState.Wait;
        anim.SetBool("Running", false);
        mainCollider.enabled = true;
        mainRb.isKinematic = false;
        GameManager.singleton.swipeState = SwipeState.Closed;
        GameManager.singleton.tapButton.SetActive(true);
        StartCoroutine(GroundCheck());
        Invoke("CheckInvoker", .3f);
    }
    public void CheckInvoker()
    {
        StartCoroutine(GroundCheck());
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 11)
        {
            lastWaypoint = other.transform;
        }
        if(other.gameObject.layer == 14)
        {
            GameManager.singleton.Completed();
        }
    }
    private void OnDrawGizmos()
    {
        float maxDistance = 10f;
        RaycastHit hit;
        bool isHit = Physics.BoxCast(transform.position + Vector3.up, Vector3.one, transform.forward, out hit, transform.rotation, maxDistance, 1 << 12);
        if (isHit)
        {
            //print(Vector3.Distance(transform.position, hit.point));
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance + Vector3.up, Vector3.one * 2);
        }
    }
}
