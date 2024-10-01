using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

// Sanitised 29/9/24

/// <summary>
/// Class which controls a "city" on the map. Each city can send and receive
/// "horses" with messages.
/// If a city is reached by a horse, the player loses
/// a life.
/// </summary>
public class CityBehaviour : MonoBehaviour
{
    /// <summary>
    /// The edges that morse and horses can travel away from the city along.
    /// </summary>
    /// <remarks>
    /// To get an inbound edge, just reverse its outbound entry.
    /// </remarks>
    public ReadOnlyCollection<Edge> OutboundEdges;


    private void Start()
    {
        // Edges are in the form AB where A is the first char in city A's name,
        // and equivalent for B.
        char cityId = gameObject.name[0];
        OutboundEdges = new(Init_GetOutboundEdges(cityId));
    }

    private List<Edge> Init_GetOutboundEdges(char cityId)
    {
        // Get the transform containing all edges.
        Transform edges = GameObject.Find("Edges").transform;
        List<Edge> outboundEdges = new();

        foreach (Transform edgeTransform in edges)
        {
            EdgeBehaviour eb = edgeTransform.GetComponent<EdgeBehaviour>();
            List<Vector2> edgePts;

            char destCityId;

            // If the edge leads out from this city, add it.
            if (edgeTransform.name[0] == cityId)
            {
                destCityId = edgeTransform.name[1];
                edgePts = new(eb.Points);
            }
            // If the edge leads into this city, add it reversed.
            else if (edgeTransform.name[1] == cityId)
            {
                destCityId = edgeTransform.name[0];
                edgePts = new(eb.Points.Reverse());
            }
            // If the edge doesn't involve this city, continue.
            else
            {
                continue;
            }

            // Add the edge, if we get to this point.
            Edge edge = new(edgePts.ToArray(), this, GetCityFromId(destCityId));
            outboundEdges.Add(edge);
        }

        return outboundEdges;
    }

    /// <summary>Gets a CityBehaviour from a city initial (id).</summary>
    /// <param name="cityId">The initial of the city (B = Bristol).</param>
    /// <remarks>This should not be static, to prevent use in invalid scenes.</remarks>
    /// <returns>The CityBehaviour if found, else null.</returns>
    private CityBehaviour GetCityFromId(char cityId)
    {
        string name;
        switch (cityId)
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
