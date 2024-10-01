using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

// Sanitised 29/9/24


public class EdgeBehaviour : MonoBehaviour
{
    /// <summary>
    /// The points of the edge that the "horse" travels between, from index 0 to Length - 1.
    /// </summary>
    /// <remarks>
    /// This data structure is populated with the use of GameObjects whose positions each represent a point.
    /// </remarks>
    public ReadOnlyCollection<Vector2> Points;


    private void Awake()
    {
        // Setup points: Each edge can have many points connecting the start and finish smoothly.
        // Each child of Edges/{Edge} is a GameObject whose position represents a point on the edge.
        int numPts = transform.childCount;
        GameObject[] objectPoints = new GameObject[numPts];

        // Populate objectPoints with the children of Edges/{Edge}
        for (int i = 0; i < numPts; i++)
        {
            objectPoints[i] = transform.GetChild(i).gameObject;
        }

        // Each point can be obtained from the anchored position of each point GameObject's transform.
        Points = new(objectPoints.Select(x => x.GetComponent<RectTransform>().anchoredPosition).ToArray());
    }
}