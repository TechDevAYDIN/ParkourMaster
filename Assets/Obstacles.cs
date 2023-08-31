using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstaclesType
{
    HalfWall,
    BigCylinder,
    ClimbWall,
    DownWall,
    SideWall,
    TurningCircle,
    JumpCylinder,
    Tree,
    Ground,
    other
}
public class Obstacles : MonoBehaviour
{
    public ObstaclesType obstacleType;
    [SerializeField] Renderer[] renderers;
    Material material;
    [SerializeField] Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<MeshRenderer>() != null)
        {
            material = GetComponent<MeshRenderer>().material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(material != null)
        {
            material.mainTextureOffset += direction * Time.unscaledDeltaTime * .5f;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }
    public IEnumerator ChangeColor(Color color)
    {
        if(renderers != null)
        {
            float t = 0f;
            Color startColor = renderers[0].material.color;
            while (t < 1)
            {
                t += Time.deltaTime * 4;
                foreach (var item in renderers)
                {
                    item.material.color = Color.Lerp(startColor, color, t);
                }
                yield return new WaitForFixedUpdate();
            }
            if (color == Color.green)
            {
                yield return new WaitForSeconds(3);
                StartCoroutine(ChangeColor(startColor));
            }                
        }
        yield return null;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(obstacleType.Equals(ObstaclesType.HalfWall) || obstacleType.Equals(ObstaclesType.SideWall) || obstacleType.Equals(ObstaclesType.TurningCircle))
                collision.gameObject.GetComponent<PlayerController>().Death(600);
            if (obstacleType.Equals(ObstaclesType.ClimbWall) || obstacleType.Equals(ObstaclesType.DownWall) || obstacleType.Equals(ObstaclesType.JumpCylinder))
                collision.gameObject.GetComponent<PlayerController>().Death(100);
        }
    }
}
