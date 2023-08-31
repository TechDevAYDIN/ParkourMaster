using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAway : MonoBehaviour
{
    MeshRenderer[] renderers;
    Material[] materials;
    public float opRate = 0;
    //Material mat;
    // Start is called before the first frame update
    void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        //mat = GetComponent<MeshRenderer>().material;
        Color color = renderers[0].material.color;        
        color.a = opRate;
        foreach (Renderer r in renderers)
        {
            r.material.color = color;
        }
        color.a = 0;
        StartCoroutine(LerpColorOverTime(renderers,renderers[0].material.color,color,.5f));
    }
    /*
    IEnumerator Fader()
    {
        Color color = mat.color;
        color.a = 0;
        while (true)
        {
            mat.color = Color.Lerp(mat.color, color, Time.unscaledDeltaTime * 5);
            if(mat.color.a <= 0.02f)
            {
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime * Time.timeScale);
        }
        Destroy(gameObject);
    }*/
    IEnumerator LerpColorOverTime(Renderer[] obj, Color startColor, Color endColor, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;
        float elapsedTime = 0.0f;

        while (Time.time <= endTime)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            foreach (var r in obj)
            {
                r.material.color = Color.Lerp(startColor, endColor, t);
            }
            yield return null;
        }
        foreach (var r in obj)
        {
            r.material.color = endColor;
        }
        Destroy(gameObject);
    }
}
