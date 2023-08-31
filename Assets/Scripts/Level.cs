using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "TechDevAydin/Add level", order = 1)]
public class Level : ScriptableObject
{
    public List<Platform> platforms = new();
}

[Serializable]
public class Platform
{
    /*
    [Range(1, 6)]
    public int minValue = 1;

    [Range(1, 6)]
    public int maxValue = 1;*/
    public ObstaclesType obstacleType;
}
