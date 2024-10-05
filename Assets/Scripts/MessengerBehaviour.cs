using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;


/// <summary>
/// Class representing a messenger.
/// </summary>
/// <remarks>
/// It belongs wholly to its CityMessageBehaviour, and is activated when that
/// is activated.
/// </remarks>
public class MessengerBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static MessengerManager Manager => GameObject.FindWithTag("GameController").GetComponent<MessengerManager>();

    /// <summary>
    /// A connection between two path points (finer than city connections).
    /// </summary>
    private struct PathEdge
    {
        public float ProportionOfTimer { get; set; }
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
    }

    private class Messenger
    {
        private readonly MessengerBehaviour m_mb;
        private readonly Image m_image;
        private readonly TMP_Text m_timer;

        private readonly PathEdge[] m_path;
        private PathEdge m_currentEdge;
        private int m_currentEdgeIndex;

        private float m_speed;
        private float m_currentEdgeTimerTotal;
        private float m_currentEdgeTimerElapsed;

        private readonly float m_timerTotal;
        private float m_timerElapsed;


        public Messenger(MessengerType type, MessengerBehaviour mb, Edge path)
        {
            m_speed = 5 + 3 * (int)type;
            m_mb = mb;
            m_image = m_mb.GetComponentInChildren<Image>();
            m_timer = m_mb.GetComponentInChildren<TMP_Text>();

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
            m_timerTotal = totalDist / m_speed;

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
            // Linearly interpolate pos over the path edge
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
                    m_mb.OnGoalReached();
                }
            }
            else
            {
                // Increment current edge timer & total timer
                m_currentEdgeTimerElapsed += dt;
                m_timerElapsed += dt;

                // Update timer UI (rounded down to seconds)
                m_timer.text = $"{(m_timerTotal - m_timerElapsed):F2}s";

                // Move the messenger along the edge
                RectTransform rt = m_mb.GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, m_currentEdge.End, m_speed * dt);
            }

            // Enable the image once it has been moved to its starting point
            m_image.enabled = true;
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

            // If this edge makes the messenger go left, flip the sprite
            Vector3 prevScale = m_image.transform.localScale;
            float prevXScaleSize = Mathf.Abs(prevScale.x);
            if ((nextEdgePos - edgePos).x < 0)
                m_image.transform.localScale = new Vector3(-prevXScaleSize, prevScale.y, prevScale.z);
            else
                m_image.transform.localScale = new Vector3(prevXScaleSize, prevScale.y, prevScale.z);

            // Ensure position is correct at the start of an edge.
            m_mb.GetComponent<RectTransform>().anchoredPosition = m_currentEdge.Start;
        }
    }

    private GameBehaviour m_gb;
    private Messenger m_messenger = null;

    /// <summary>
    /// Has the messenger begun moving?
    /// </summary>
    public bool MessengerActive { get; private set; } = false;

    private List<EdgeBehaviour> m_messengerEdges;

    [SerializeField]
    private CityMessageBehaviour m_message;


    private void Start()
    {
        m_gb = GameObject.FindWithTag("GameController").GetComponent<GameBehaviour>();
    }


    private void Update()
    {
        m_messenger?.Step(Time.deltaTime);
    }


    /// <summary>
    /// Activates the messenger, and sets it off on its journey to the destination city.
    /// </summary>
    /// <param name="type">Type of the messenger (person/pony/train/...).
    /// <param name="startCity">The city the messenger starts at.</param>
    /// <param name="cityDist">The number of cities the messenger travels to.</param>
    public void ActivateMessenger(MessengerType type, CityBehaviour startCity, int cityDist)
    {
        // This assertation fixes ridiculous distances, but doesn't cover slightly excessive
        // distances (which are handled below and ignored).
        Debug.Assert(cityDist < GameObject.Find("Cities").transform.childCount);

        // Recursively, randomly generate journey.
        Edge journey = RandomlyGeneratePath(cityDist, new List<CityBehaviour> { startCity }, null);

        // Calculate which edges are contained entirely by the path.
        m_messengerEdges = new();
        Transform edgeTransforms = GameObject.Find("Edges").transform;
        foreach (Transform edgeTransform in edgeTransforms)
        {
            EdgeBehaviour eb = edgeTransform.GetComponent<EdgeBehaviour>();
            Vector2[] ebPathPts = eb.Points.ToArray();

            // Check path completely contains messenger's points.
            if (journey.Points.Intersect(ebPathPts).Count() == ebPathPts.Length)
            {
                m_messengerEdges.Add(eb);
            }
        }

        m_messenger = new Messenger(type, this, journey);
        MessengerActive = true;
    }


    /// <summary>
    /// Recursively, randomly generates a path.
    /// </summary>
    /// <param name="dist">The maximum (goal) number of cities the path will be. May be lower
    /// if the path reaches a leaf node early. This is fine as messenger will just move slower.
    /// </param>
    /// <param name="visited">A record of all visited cities.</param>
    private Edge RandomlyGeneratePath(int dist, List<CityBehaviour> visited, Edge genPath)
    {
        // Base case.
        if (dist == 0) return genPath;

        // Get most recently added city.
        CityBehaviour city = visited[^1];

        // Cannot travel to/from city already travelled to in visited.
        List<Edge> paths = new(city.OutboundEdges);
        foreach (Edge path in city.OutboundEdges)
        {
            if (visited.Contains(path.To)) paths.Remove(path);
        }

        // Pick random next city. Add points from the travelled path and add the city to visited.
        if (paths.Count > 0)
        {
            Edge ext = paths[Random.Range(0, paths.Count)];
            genPath = (genPath == null ? ext : Edge.Extend(genPath, ext));
            visited.Add(ext.To);
            return RandomlyGeneratePath(dist - 1, visited, genPath);
        }

        // No cities left => return; a dist too high was picked.
        Debug.LogWarning($"Excess distance: {dist}");
        return RandomlyGeneratePath(0, visited, genPath);
    }


    /// <summary>
    /// Called when the messenger reaches its dest, or you beat it there. Messenger disappears,
    /// and the messenger activity status is updated.
    /// </summary>
    public void Explode()
    {
        gameObject.SetActive(false);
        MessengerActive = false;
    }


    /// <summary>
    /// The messenger beat you to its destination; you lose a life, and the messenger disappears.
    /// </summary>
    private void OnGoalReached()
    {
        m_gb.LoseLife(CityMessageBehaviour.Manager.ActiveMessages[m_message.Index]);
        Explode();
    }


    /// <summary>
    /// Delegate to the city's OnPointerEnter event.
    /// </summary>
    public void OnPointerEnter(PointerEventData _)
    {
        m_message.OnPointerEnter(_);
    }


    /// <summary>
    /// Delegate to the city's OnPointerExit event.
    /// </summary>
    public void OnPointerExit(PointerEventData _)
    {
        m_message.OnPointerExit(_);
    }


    /// <summary>
    /// Delegate to the city's OnClicked event.
    /// </summary>
    public void OnClicked()
    {
        m_message.OnClicked();
    }

    
    /// <summary>
    /// Sets the pulsing status of all telegraph segments on the messenger's path.
    /// </summary>
    /// <param name="pulse">Whether it should be off, pulsing, or on.</param>
    public void SetTelegraphPulse(TelegraphPulse pulse)
    {
        for (int i = 0; i < m_messengerEdges.Count; i++)
        {
            m_messengerEdges[i].GetComponent<TelegraphLineBehaviour>().Pulse = pulse;
        }
    }
}
