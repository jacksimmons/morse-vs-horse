using System.Collections.ObjectModel;
using UnityEngine;


public class PathBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_points;
    public ReadOnlyCollection<GameObject> Points;


    private void Awake()
    {
        Points = new(m_points);
    }
}