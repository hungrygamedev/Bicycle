using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GroundPlatform : MonoBehaviour
{
    public bool isFinish;
    public Transform nextSpawnPoint;
    public List<EnemyPath> pathTrue, pathFail = new List<EnemyPath>();

    public EnemyPath GetPath()
    {
        if (pathFail.Count > 0)
        {
            float percent = Random.value;
            if (percent > 0.5f)
            {
                return pathFail[Random.Range(0, pathFail.Count)];
            }
            else
            {
                return pathTrue[Random.Range(0, pathTrue.Count)];
            }
        }
        else
        {
            return pathTrue[Random.Range(0, pathTrue.Count)];
        }
    }
    
    
}
