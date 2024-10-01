using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Represents a two-way connection between two cities.
/// </summary>
/// <remarks>
/// An "edge" in this codebase means a connection, containing at least 2 points (a start and an end).
/// Edges can, however, have many points within them to allow for smooth moving of horses between cities.
/// </remarks>
public class Edge
{
    public readonly Vector2[] Points;
    public readonly CityBehaviour From;
    public readonly CityBehaviour To;


    public Edge(Vector2[] points, CityBehaviour from, CityBehaviour to)
    {
        Points = points;
        From = from;
        To = to;
    }


    /// <summary>
    /// Combines two paths, using original.From and extension.To.
    /// </summary>
    public static Edge Extend(Edge original, Edge extension)
    {
        List<Vector2> pts = original.Points.ToList();
        pts.AddRange(extension.Points);

        return new Edge(pts.ToArray(), original.From, extension.To);
    }
}