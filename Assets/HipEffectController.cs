using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipEffectController : MonoBehaviour
{
    [SerializeField] GameObject hitImpact;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 10)
        {
            if (!hitImpact.activeSelf)
            {
                hitImpact.transform.position = collision.contacts[0].point;
                hitImpact.SetActive(true);
            }
        }
    }
}
