using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class Path
{
    public readonly Vector2[] Points;
    public readonly CityBehaviour From;
    public readonly CityBehaviour To;


    public Path(Vector2[] points, CityBehaviour from, CityBehaviour to)
    {
        Points = points;
        From = from;
        To = to;
    }


    /// <summary>
    /// Combines two paths, using original.From and extension.To.
    /// </summary>
    public static Path Extend(Path original, Path extension)
    {
        List<Vector2> pts = original.Points.ToList();
        pts.AddRange(extension.Points);

        return new Path(pts.ToArray(), original.From, extension.To);
    }
}