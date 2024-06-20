using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PonyBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// A connection between two path points (finer than city connections).
    /// </summary>
    private struct PathEdge
    {
        public float ProportionOfTimer;
        public Vector2 Start;
        public Vector2 End;
    }

    private class Pony
    {
        private readonly PonyBehaviour m_pony;
        private readonly Image m_ponyImage;
        private readonly TMP_Text m_ponyTimer;

        // !Debug
        private readonly Path m_origPath;

        private PathEdge[] m_path;
        private PathEdge m_currentEdge;
        private int m_currentEdgeIndex;

        private float m_speed;
        private float m_currentEdgeTimerTotal;
        private float m_currentEdgeTimerElapsed;

        private readonly float m_timerTotal;
        private float m_timerElapsed;


        public Pony(Difficulty diff, PonyBehaviour pony, Path path)
        {
            m_origPath = path;

            m_timerTotal = 45 + (1 + (int)diff) * 10;
            m_pony = pony;
            m_ponyImage = pony.GetComponentInChildren<Image>();
            m_ponyTimer = pony.GetComponentInChildren<TMP_Text>();

            // Calculate total path length (and number of edges in path)
            float totalDist = 0;
            int numEdges = 0;
            for (int i = 0; i < path.Points.Length - 1; i++)
            {
                Vector2 start = path.Points[i];
                Vector2 end = path.Points[i + 1];
                totalDist += Vector2.Distance(start, end);
                numEdges++;
            }
            m_speed = totalDist / m_timerTotal;

            // Convert SerializeFields into PathEdge
            m_path = new PathEdge[numEdges];
            for (int i = 0; i < numEdges; i++)
            {
                PathEdge e = new()
                {
                    Start = path.Points[i],
                    End = path.Points[i + 1],
                };
                float dist = Vector2.Distance(e.End, e.Start);
                e.ProportionOfTimer = dist / totalDist;
                m_path[i] = e;
            }
            SetEdge(0);
        }


        public void Step(float dt)
        {
            // Linearly interpolate pony pos over the path edge
            float t = m_currentEdgeTimerElapsed / m_currentEdgeTimerTotal;

            // If the edge destination has been reached...
            if (t >= 1)
            {
                // If the edge is not the final end...
                if (m_currentEdgeIndex + 1 < m_path.Length)
                {
                    SetEdge(m_currentEdgeIndex + 1);
                }
                // Else the entire path has been completed, and the player loses a life.
                else
                {
                    m_pony.OnGoalReached();
                }
            }
            else
            {
                // Increment current edge timer & total timer
                m_currentEdgeTimerElapsed += dt;
                m_timerElapsed += dt;

                // Update timer UI (rounded down to seconds)
                m_ponyTimer.text = $"{(m_timerTotal - m_timerElapsed):F2}s";

                // Move the pony along the edge
                m_pony.transform.position = Vector2.MoveTowards(m_pony.transform.position, m_currentEdge.End, m_speed * dt);
            }

            // Enable the pony image once it has been moved to its starting point
            m_ponyImage.enabled = true;
        }

        private void SetEdge(int edgeIndex)
        {
            m_currentEdge = m_path[edgeIndex];
            m_currentEdgeIndex = edgeIndex;
            m_currentEdgeTimerElapsed = 0;
            m_currentEdgeTimerTotal = m_timerTotal * m_currentEdge.ProportionOfTimer;

            // If the end hasn't been reached, check to see if the direction is left
            // If so, set the local scale to the negative of its magnitude
            Vector2 edgePos = m_currentEdge.Start;
            Vector2 nextEdgePos = m_currentEdge.End;

            // If this edge makes the pony go left, flip the sprite
            Vector3 prevScale = m_ponyImage.transform.localScale;
            float prevXScaleSize = Mathf.Abs(prevScale.x);
            if ((nextEdgePos - edgePos).x < 0)
                m_ponyImage.transform.localScale = new Vector3(-prevXScaleSize, prevScale.y, prevScale.z);
            else
                m_ponyImage.transform.localScale = new Vector3(prevXScaleSize, prevScale.y, prevScale.z);

            // Ensure pony position is correct at the start of an edge.
            m_pony.transform.position = m_currentEdge.Start;
        }
    }

    private GameBehaviour m_gb;
    private Pony m_pony = null;

    /// <summary>
    /// Has the pony begun moving?
    /// </summary>
    public bool PonyActive { get; private set; } = false;

    private List<PathBehaviour> m_ponyPaths;
    /// <summary>
    /// The paths that make up the pony's journey.
    /// </summary>
    public ReadOnlyCollection<PathBehaviour> PonyPaths;

    [SerializeField]
    private EarlBehaviour m_earl;


    private void Start()
    {
        PonyPaths = new(m_ponyPaths);
        m_gb = GameObject.FindWithTag("GameController").GetComponent<GameBehaviour>();
    }


    private void Update()
    {
        if (m_pony != null)
        {
            m_pony.Step(Time.deltaTime);
        }
    }


    /// <summary>
    /// Activates the pony express, and sets it off on its journey to the destination city.
    /// </summary>
    /// <param name="diff">Difficulty of the pony/word. Higher difficulty causes pony to take
    /// longer in total, and causes words to be selected from a harder word list file.</param>
    /// <param name="startCity">The city the pony starts at.</param>
    /// <param name="cityDist">The number of cities the pony travels to.</param>
    public void ActivatePony(Difficulty diff, CityBehaviour startCity, int cityDist)
    {
        // This assertation fixes ridiculous distances, but doesn't cover slightly excessive
        // distances (which are handled below and ignored).
        Debug.Assert(cityDist < GameObject.Find("Cities").transform.childCount);

        // Recursively, randomly generate pony journey.
        Path journey = RandomlyGeneratePath(cityDist, new List<CityBehaviour> { startCity }, null);

        // Calculate which paths are contained entirely by the pony path.
        m_ponyPaths = new();
        Transform pathTransforms = GameObject.Find("Paths").transform;
        foreach (Transform pathTransform in pathTransforms)
        {
            PathBehaviour pb = pathTransform.GetComponent<PathBehaviour>();
            Vector2[] pbPathPts = pb.Points.Select(x => (Vector2)x.transform.position).ToArray();

            // Check path completely contains pb's points.
            if (journey.Points.Intersect(pbPathPts).Count() == pbPathPts.Length)
            {
                m_ponyPaths.Add(pb);
            }
        }

        m_pony = new Pony(diff, this, journey);
        PonyActive = true;
    }


    /// <summary>
    /// Recursively, randomly generates a pony path.
    /// </summary>
    /// <param name="dist">The maximum (goal) number of cities the path will be. May be lower
    /// if the path reaches a leaf node early. This is fine as the pony will just move slower.
    /// </param>
    /// <param name="visited">A record of all visited cities.</param>
    private Path RandomlyGeneratePath(int dist, List<CityBehaviour> visited, Path genPath)
    {
        // Base case.
        if (dist == 0) return genPath;

        // Get most recently added city.
        CityBehaviour city = visited[^1];

        // Cannot travel to/from city already travelled to in visited.
        List<Path> paths = new(city.AdjacentPaths);
        foreach (Path path in city.AdjacentPaths)
        {
            if (visited.Contains(path.To)) paths.Remove(path);
        }

        // Pick random next city. Add points from the travelled path and add the city to visited.
        if (paths.Count > 0)
        {
            Path ext = paths[Random.Range(0, paths.Count)];
            genPath = (genPath == null ? ext : Path.Extend(genPath, ext));
            visited.Add(ext.To);
            return RandomlyGeneratePath(dist - 1, visited, genPath);
        }

        // No cities left => return; a dist too high was picked.
        Debug.LogWarning($"Excess distance: {dist}");
        return RandomlyGeneratePath(0, visited, genPath);
    }


    /// <summary>
    /// Called when the pony is victorious or loses. Pony disappears here,
    /// and the earl's pony activity status is updated.
    /// </summary>
    public void Explode()
    {
        Destroy(gameObject);
        PonyActive = false;
    }


    private void OnGoalReached()
    {
        m_gb.LoseLife();
        Explode();
    }


    /// <summary>
    /// When hovering, show timer.
    /// </summary>
    public void OnPointerEnter(PointerEventData _)
    {
        GetComponentInChildren<TMP_Text>().enabled = true;
        SetPathPulse(true);
    }


    /// <summary>
    /// Hide timer.
    /// </summary>
    public void OnPointerExit(PointerEventData _)
    {
        GetComponentInChildren<TMP_Text>().enabled = false;
        SetPathPulse(false);
    }


    public void SetPathPulse(bool pulse)
    {
        foreach (PathBehaviour pb in m_ponyPaths)
        {
            pb.SetPulsing(pulse);
        }
    }


    public void SetPathLocked(bool locked)
    {
        foreach (PathBehaviour pb in m_ponyPaths)
        {
            pb.SetLocked(locked);
        }
    }
}
