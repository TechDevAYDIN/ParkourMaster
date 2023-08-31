using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    AIState aiState = AIState.Wait;
    public float runSpeed = 2;
    Animator anim;
    [SerializeField] AnimatorOverrideController[] overrideController;
    [SerializeField] Rigidbody[] deathRigidBodies;
    Rigidbody mainRb;
    CapsuleCollider mainCollider;
    Transform lastWaypoint;
    int canPassObstacle = 0;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        mainRb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<CapsuleCollider>();
        aiState = AIState.Wait;
        StartCoroutine(GroundCheck());
        StartCoroutine(SpeedControl());
    }

    // Update is called once per frame
    void Update()
    {
        CheckObstacles();
        if (aiState.Equals(AIState.Running))
        {
            transform.position += Vector3.forward * runSpeed * Time.deltaTime;
            anim.SetFloat("Blend", Mathf.Lerp(anim.GetFloat("Blend"), runSpeed, Time.unscaledDeltaTime * 4));
        }
    }
    IEnumerator SpeedControl()
    {
        while (true)
        {
            if (PlayerController.singleton.gameObject.transform.position.z >= transform.position.z)
            {
                runSpeed = Random.Range(4, 9);
            }
            else
            {
                runSpeed = Random.Range(2, 7);
            }
            yield return new WaitForSeconds(2.5f);
        }
    }
    public void StartRunning()
    {
        aiState = AIState.Running;
        anim.SetBool("Running", true);
    }
    public void GoalReached()
    {
        EndAnim();
        aiState = AIState.Wait;        
        anim.SetBool("Running", false);
    }
    public void Death(float size)
    {
        anim.enabled = false;
        aiState = AIState.Dead;
        mainCollider.enabled = false;
        mainRb.isKinematic = true;
        foreach (var r in deathRigidBodies)
        {
            r.isKinematic = false;
        }
        foreach (var r in deathRigidBodies)
        {
            r.AddForce(Vector3.forward * size + Vector3.up * (size * .25f));
        }
        StartCoroutine(Resurraction());
    }
    IEnumerator Resurraction()
    {
        yield return new WaitForSeconds(2f);
        EndAnim();
        transform.position = lastWaypoint.position;
        foreach (var r in deathRigidBodies)
        {
            r.isKinematic = true;
        }
        anim.enabled = true;
        aiState = AIState.Running;
        anim.SetBool("Running", true);
        mainCollider.enabled = true;
        mainRb.isKinematic = false;
        StartCoroutine(GroundCheck());
        Invoke("CheckInvoker", .3f);
    }

    void StartAnim(bool passed = true, ObstaclesType obstacle = ObstaclesType.HalfWall)
    {
        if (passed)
        {
            mainCollider.enabled = false;
        }
        else
        {
            mainCollider.enabled = true;
        }
        aiState = AIState.Animation;
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
    public void RandomPass()
    {
        if(PlayerController.singleton.gameObject.transform.position.z >= transform.position.z)
        {
            canPassObstacle = 1; 
        }
        else
        {
            canPassObstacle = Random.Range(1,3);
        }
    }
    public void EndAnim()
    {
        //anim.SetBool("JumpHalf", false);
        foreach (var p in anim.parameters)
        {
            if (p.type.Equals(AnimatorControllerParameterType.Bool))
                anim.SetBool(p.name, false);
        }
        anim.SetBool("Running", true);
        StartCoroutine(GroundCheck());
        canPassObstacle = 0;
        aiState = AIState.Running;
        mainCollider.enabled = true;
    }
    public void CheckObstacles()
    {
        float maxDistance = 10f;
        RaycastHit hit;
        bool isHit = Physics.BoxCast(transform.position + Vector3.up, Vector3.one, transform.forward, out hit, transform.rotation, maxDistance, 1 << 12);
        //obstacleType = hit.collider.GetComponent<Obstacles>().type;

        if (isHit && aiState.Equals(AIState.Running))
        {
            HitCheckObstacle(hit);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            lastWaypoint = other.transform;
        }
        if (other.gameObject.layer == 14)
        {
            GameManager.singleton.Completed();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 12)
        {
            Death(250);
        }
    }
    public void HitCheckObstacle(RaycastHit hit)
    {
        switch (hit.collider.GetComponent<Obstacles>().obstacleType)
        {
            case ObstaclesType.HalfWall:
                if (Vector3.Distance(transform.position, hit.point) < 5f && canPassObstacle == 0)
                {
                    RandomPass();
                }
                if (Vector3.Distance(transform.position, hit.point) < 2.2f)
                {
                    if (aiState.Equals(AIState.Running))
                    {
                        if (canPassObstacle == 1)
                        {
                            StartAnim(true, ObstaclesType.HalfWall);
                        }

                        if (canPassObstacle == 2)
                        {
                            StartAnim(false, ObstaclesType.HalfWall);
                        }
                    }
                }
                break;
            case ObstaclesType.ClimbWall:
                if (Vector3.Distance(transform.position, hit.point) < 6.5f && canPassObstacle == 0)
                {
                    RandomPass();
                }
                if (Vector3.Distance(transform.position, hit.point) < 3.25f)
                {
                    if (aiState.Equals(AIState.Running))
                    {
                        if (canPassObstacle == 1)
                        {
                            StartAnim(true, ObstaclesType.ClimbWall);
                        }
                        if (canPassObstacle == 2)
                        {
                            StartAnim(false, ObstaclesType.ClimbWall);
                        }
                    }
                }
                break;
            case ObstaclesType.DownWall:
                if (Vector3.Distance(transform.position, hit.point) < 6f && canPassObstacle == 0)
                {
                    RandomPass();
                }
                if (Vector3.Distance(transform.position, hit.point) < 3f)
                {
                    if (aiState.Equals(AIState.Running))
                    {
                        if (canPassObstacle == 1)
                        {
                            StartAnim(true, ObstaclesType.DownWall);
                        }

                        if (canPassObstacle == 2)
                        {
                            StartAnim(false, ObstaclesType.DownWall);
                        }
                    }
                }
                break;
            case ObstaclesType.SideWall:
                if (Vector3.Distance(transform.position, hit.point) < 5f && canPassObstacle == 0)
                {
                    RandomPass();
                }
                if (Vector3.Distance(transform.position, hit.point) < 2f)
                {
                    if (aiState.Equals(AIState.Running))
                    {
                        if (canPassObstacle == 1)
                        {
                            StartAnim(true, ObstaclesType.SideWall);
                        }

                        if (canPassObstacle == 2)
                        {
                            StartAnim(false, ObstaclesType.SideWall);
                        }
                    }
                }
                break;
            case ObstaclesType.TurningCircle:
                if (Vector3.Distance(transform.position, hit.point) < 8f && canPassObstacle == 0)
                {
                    RandomPass();
                }
                if (Vector3.Distance(transform.position, hit.point) < 4.5f)
                {
                    if (aiState.Equals(AIState.Running))
                    {
                        if (canPassObstacle == 1)
                        {
                            StartAnim(true, ObstaclesType.TurningCircle);
                        }

                        if (canPassObstacle == 2)
                        {
                            StartAnim(false, ObstaclesType.TurningCircle);
                        }
                    }
                }
                break;
            case ObstaclesType.JumpCylinder:
                if (Vector3.Distance(transform.position, hit.point) < 7f && canPassObstacle == 0)
                {
                    RandomPass();
                }
                if (Vector3.Distance(transform.position, hit.point) < 3.7f)
                {
                    if (aiState.Equals(AIState.Running))
                    {
                        if (canPassObstacle == 1)
                        {
                            StartAnim(true, ObstaclesType.JumpCylinder);
                        }

                        if (canPassObstacle == 2)
                        {
                            StartAnim(false, ObstaclesType.JumpCylinder);
                        }
                    }
                }
                break;
        }
    }
    public void CheckInvoker()
    {
        StartCoroutine(GroundCheck());
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
}
