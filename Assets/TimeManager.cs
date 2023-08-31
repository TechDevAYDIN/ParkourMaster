using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public IEnumerator SlowDownTime(float timeToScale, float duration)
    {
        float timer = 0;
        while(timer < duration)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, timeToScale, timer * (1 / duration));
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator FastUpTime(float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, timer * (1 / duration));
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
