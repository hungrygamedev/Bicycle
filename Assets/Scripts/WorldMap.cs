using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldMap : MonoBehaviour
{
    public static WorldMap instance;

    public bool canGanarate;
    public int maxPlatforms;
    public List<GameObject> platforms = new List<GameObject>();
    public List<GameObject> platformsCheckPoint = new List<GameObject>();
    public List<GroundPlatform> _grounds = new List<GroundPlatform>();

    public Vector3 startPoint, endPoint;

    public List<int> generatedPlatformsID = new List<int>();
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        startPoint = _grounds[0].transform.position;
        if (canGanarate)
        {
            for (int i = 0; i < maxPlatforms; i++)
            {
                if (i % 2 == 0)
                {
                    if (i == maxPlatforms - 1)
                    {
                        SpawnPlatformCheckPoint(true);
                    }
                    else
                    {
                        SpawnPlatformCheckPoint(false);
                    }
                }
                else
                {
                    SpawnPlatform();
                }
            }

            endPoint = _grounds[_grounds.Count - 1].transform.position;
            UIController.instance._PlayersPositionUI.SetVectors(startPoint,endPoint);
        }
    }

    void SpawnPlatform()
    {
        int r = GetNextPlatformID();
        GameObject pl = Instantiate(platforms[r],transform);
        if (_grounds.Count > 0)
        {
            pl.transform.position = _grounds[_grounds.Count - 1].nextSpawnPoint.position;
        }
        else
        {
            pl.transform.position = Vector3.zero;
        }
        _grounds.Add(pl.GetComponent<GroundPlatform>());
    }

    int GetNextPlatformID()
    {
        int r = Random.Range(0, platforms.Count);
        if (generatedPlatformsID.Count > 0 && generatedPlatformsID.Count<platforms.Count)
        {
            if (generatedPlatformsID.Contains(r))
            {
                return GetNextPlatformID();
            }
            else
            {
                generatedPlatformsID.Add(r);
                return r;
            }
        }
        else
        {
            generatedPlatformsID.Clear();
            generatedPlatformsID.Add(r);
            return r;
        }
    }
    
    void SpawnPlatformCheckPoint(bool isFinish)
    {
        int r = Random.Range(0, platformsCheckPoint.Count);
        GameObject pl = Instantiate(platformsCheckPoint[r],transform);
        if (_grounds.Count > 0)
        {
            pl.transform.position = _grounds[_grounds.Count - 1].nextSpawnPoint.position;
        }

        pl.GetComponent<GroundPlatform>().isFinish = isFinish;
        _grounds.Add(pl.GetComponent<GroundPlatform>());
    }

    public bool isCanGetNextPlatform(int lastID)
    {
        if (lastID < _grounds.Count)
        {
            return true;
        }

        return false;
    }

    public int GetCurrentplatformID(GroundPlatform platform)
    {
        for (int i = 0; i < _grounds.Count; i++)
        {
            if (platform == _grounds[i])
            {
                return i;
            }
        }

        return 0;
    }
    public EnemyPath GetEnemyPath(int lastPlatormID)
    {
        if (lastPlatormID < _grounds.Count)
        {
            return _grounds[lastPlatormID].GetPath();
        }

        return null;
    }
}
