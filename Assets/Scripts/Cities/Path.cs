using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class Path
{
    public readonly Vector2[] Points;
    public readonly string[] PointNames;
    public readonly CityBehaviour From;
    public readonly CityBehaviour To;


    public Path(GameObject[] pointObjs, CityBehaviour from, CityBehaviour to)
    {
        Points = pointObjs.Select(x => (Vector2)(x.transform.position)).ToArray();
        PointNames = pointObjs.Select(x => x.name).ToArray();
        From = from;
        To = to;
    }


    public Path(Vector2[] pts, string[] ptNames, CityBehaviour from, CityBehaviour to)
    {
        Points = pts;
        PointNames = ptNames;
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

        List<string> ptNames = original.PointNames.ToList();
        ptNames.AddRange(extension.PointNames);
        
        return new Path(pts.ToArray(), ptNames.ToArray(), original.From, extension.To);
    }
}