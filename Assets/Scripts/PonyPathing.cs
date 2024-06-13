using System.Net.Security;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Different varieties of pony
/// </summary>
public enum PonyDiff
{
    Easy,
    Medium,
    Hard
}


public class PonyPathing : MonoBehaviour
{
    private struct PathEdge
    {
        public float ProportionOfTimer;
        public Vector2 Start;
        public Vector2 End;
    }

    [SerializeField]
    private GameObject[] m_ponyPathPts;

    private PathEdge[] m_path;
    private PathEdge m_currentEdge;
    private int m_currentEdgeIndex;

    private float m_currentEdgeTimerTotal;
    private float m_currentEdgeTimerElapsed;

    private GameBehaviour m_gb;

    public PonyDiff Diff
    {
        set
        {
            m_timerTotal = 10 * (1 + (int)value);
        }
    }
    private float m_timerTotal;


    private void Start()
    {
        // Calculate total path length (and number of edges in path)
        float totalDist = 0;
        int numEdges = 0;
        for (int i = 0; i < m_ponyPathPts.Length - 1; i++)
        {
            Vector2 start = m_ponyPathPts[i].transform.position;
            Vector2 end = m_ponyPathPts[i + 1].transform.position;
            totalDist += Vector2.Distance(start, end);
            numEdges++;
        }

        // Convert SerializeFields into PathEdge
        m_path = new PathEdge[numEdges];
        for (int i = 0; i < numEdges; i++)
        {
            PathEdge e = new PathEdge()
            {
                Start = m_ponyPathPts[i].transform.position,
                End = m_ponyPathPts[i + 1].transform.position,
            };
            float dist = Vector2.Distance(e.End, e.Start);
            e.ProportionOfTimer = dist / totalDist;
            m_path[i] = e;
        }
        SetEdge(0);



        // Init GameBehaviour
        m_gb = GameObject.FindWithTag("GameController").GetComponent<GameBehaviour>();
    }


    private void Update()
    {
        // Linearly interpolate pony pos over the path edge
        float t = m_currentEdgeTimerElapsed / m_currentEdgeTimerTotal;

        // If the edge destination has been reached...
        if (t >= 1)
        {
            // If the edge is not the final end...
            if (m_currentEdgeIndex + 1 < m_path.Length)
                SetEdge(m_currentEdgeIndex + 1);
            // Else the entire path has been completed, and the player loses a life.
            else
                m_gb.LoseLife();

        }
        else
        {
            // Increment current edge timer
            m_currentEdgeTimerElapsed += Time.deltaTime;

            // Move the pony along the edge
            transform.position = Vector2.Lerp(m_currentEdge.Start, m_currentEdge.End, t);
        }
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

        Vector3 prevScale = transform.localScale;
        float prevXScaleSize = Mathf.Abs(prevScale.x);
        // If this edge makes the pony go left, flip the sprite
        if ((nextEdgePos - edgePos).x < 0)
            transform.localScale = new Vector3(-prevXScaleSize, prevScale.y, prevScale.z);
        else
            transform.localScale = new Vector3(prevXScaleSize, prevScale.y, prevScale.z);
    }
}
