using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class PathBehaviour : MonoBehaviour
{
    private GameObject[] m_pathPoints;
    public Vector2[] Points { get; private set; }


    private void Awake()
    {
        // Setup points
        int numPts = transform.childCount;
        m_pathPoints = new GameObject[numPts];
        for (int i = 0; i < numPts; i++)
        {
            m_pathPoints[i] = transform.GetChild(i).gameObject;
        }
        Points = m_pathPoints.Select(x => (Vector2)x.transform.position).ToArray();
    }
}