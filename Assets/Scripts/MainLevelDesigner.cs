using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainLevelDesigner : MonoBehaviour
{
    public Transform goalTransform;
    public List<GameObject> PlatformPrefabs = new();
    private readonly List<GameObject> spawnedLevels = new();
    private Vector3 spawnPos = new(0,-.25f,2.5f);
    // Start is called before the first frame update
    void Start()
    {

    }

    private Level[] GetLevels()
    {
        return Resources.LoadAll<Level>("Levels");
    }
    public void LoadLevel(int levelNum)
    {
        // Get the correct stage
        for (int i = levelNum; i > GetLevels().Length - 1; i -= 10)
        {
            Debug.Log("Level " + levelNum);
            levelNum -= 10;
        }        
        Level level = GetLevels()[Mathf.Clamp(levelNum, 0, GetLevels().Length - 1)];

        if (level == null)
        {
            Debug.LogError("No Level " + levelNum + " found in allSLevels list (MainLevelDesigner). All stages assigned in list?");
            return;
        }

        for (int i = 0; i < level.platforms.Count; i++)
        {
            AddPlatform(level.platforms[i].obstacleType);
        }
        goalTransform.position = spawnPos;
        //FindObjectOfType<MovementController>().zKalan = spawnPosZ;
        //GetComponent<NavMeshSurface>().BuildNavMesh();
        //Debug.Log(NavMeshLoader.LoadNavMeshAsync(GetComponent<NavMeshSurface>()));        
    }
    void AddPlatform(ObstaclesType obstaclesType)
    {
        GameObject platform;
        switch (obstaclesType)
        {
            case ObstaclesType.Ground:                
                platform = Instantiate(PlatformPrefabs[0], null);
                platform.transform.localPosition = spawnPos;
                platform.transform.eulerAngles = new Vector3(0, 0, 0);
                spawnedLevels.Add(platform);
                spawnPos += Vector3.forward * 2.5f;
                break;
            case ObstaclesType.HalfWall:
                platform = Instantiate(PlatformPrefabs[1], null);
                platform.transform.localPosition = spawnPos;
                platform.transform.eulerAngles = new Vector3(0, 0, 0);
                spawnedLevels.Add(platform);
                spawnPos += Vector3.forward * 2.5f;
                break;
            case ObstaclesType.TurningCircle:
                platform = Instantiate(PlatformPrefabs[2], null);
                platform.transform.localPosition = spawnPos;
                platform.transform.eulerAngles = new Vector3(0, 0, 0);
                spawnedLevels.Add(platform);
                spawnPos += Vector3.forward * 2.5f;
                break;
            case ObstaclesType.JumpCylinder:
                platform = Instantiate(PlatformPrefabs[3], null);
                platform.transform.localPosition = spawnPos;
                platform.transform.eulerAngles = new Vector3(0, 0, 0);
                spawnedLevels.Add(platform);
                spawnPos += Vector3.forward * 2.5f;
                break;
            case ObstaclesType.DownWall:
                platform = Instantiate(PlatformPrefabs[4], null);
                platform.transform.localPosition = spawnPos;
                platform.transform.eulerAngles = new Vector3(0, 0, 0);
                spawnedLevels.Add(platform);
                spawnPos += Vector3.forward * 2.5f;
                break;
            case ObstaclesType.SideWall:
                platform = Instantiate(PlatformPrefabs[5], null);
                platform.transform.localPosition = spawnPos;
                platform.transform.eulerAngles = new Vector3(0, 0, 0);
                spawnedLevels.Add(platform);
                spawnPos += Vector3.forward * 5f;
                break;
            case ObstaclesType.ClimbWall:
                platform = Instantiate(PlatformPrefabs[6], null);
                platform.transform.localPosition = spawnPos;
                platform.transform.eulerAngles = new Vector3(0, 0, 0);
                spawnedLevels.Add(platform);
                spawnPos += (Vector3.forward * 5f + Vector3.up * 2.5f);
                break;
            case ObstaclesType.other:
                platform = Instantiate(PlatformPrefabs[7], null);
                platform.transform.localPosition = spawnPos;
                platform.transform.eulerAngles = new Vector3(0, 0, 0);
                spawnedLevels.Add(platform);
                spawnPos += (Vector3.forward * 2.5f);
                break;
        }
    }
}
