    using System;
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    private List<Transform> pathPoints = new List<Transform>();


    public Transform GetNextPoint(int id)
    {
        if (pathPoints.Count == 0)
        {
            foreach (Transform point in transform)
            {
                pathPoints.Add(point);
            }
        }

        if (id+1 < pathPoints.Count)
        {
            return pathPoints[id + 1];
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.GetChild(i).position,2f);
        }
    }
}
