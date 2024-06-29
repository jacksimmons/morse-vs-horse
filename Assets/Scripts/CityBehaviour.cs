using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


public class CityBehaviour : MonoBehaviour
{
    private List<Path> m_adjacentPaths = new();
    public ReadOnlyCollection<Path> AdjacentPaths;


    private void Start()
    {
        // Edges are in the form AB where A is the first char in city A's name,
        // and equivalent for B.
        char nameFirstLetter = gameObject.name[0];

        Transform paths = GameObject.Find("Paths").transform;
        foreach (Transform pathTransform in paths)
        {
            PathBehaviour pb = pathTransform.GetComponent<PathBehaviour>();
            List<Vector2> path;

            char destChar;

            // If path leads out from this city, add the path.
            if (pathTransform.name[0] == nameFirstLetter)
            {
                destChar = pathTransform.name[1];
                path = new(pb.Points);
            }
            // If path leads into this city, add the reverse of the path.
            else if (pathTransform.name[1] == nameFirstLetter)
            {
                destChar = pathTransform.name[0];
                path = new(pb.Points.Reverse());
            }
            // Path doesn't involve this city, continue.
            else
                continue;

            // Add the above-determined path.
            m_adjacentPaths.Add(new Path(path.ToArray(), this, GetPathDest(destChar)));
        }

        AdjacentPaths = new(m_adjacentPaths);
    }


    private static CityBehaviour GetPathDest(char destChar)
    {
        string name;
        switch (destChar)
        {
            case 'B':
                name = "Bristol";
                break;
            case 'C':
                name = "Cornwall";
                break;
            case 'L':
                name = "London";
                break;
            case 'N':
                name = "Newport";
                break;
            case 'H':
                name = "Hull";
                break;
            default:
                return null;
        }

        return GameObject.Find(name).GetComponent<CityBehaviour>();
    }
}
