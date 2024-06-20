using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;


public class PathBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_points;
    public ReadOnlyCollection<GameObject> Points;

    [SerializeField]
    private Image m_telegraphImage;
    private bool m_telegraphPulsing = false;
    private bool m_telegraphLocked = false;
    private Color m_telegraphPulseMin = Color.black;
    private Color m_telegraphPulseMax = Color.red;
    private float m_pulseUpElapsed = 0;
    private float m_pulseDownElapsed = 0;
    private bool m_pulseUpElseDown = true;
    private const float PULSE_TIME = 2f;


    private void Awake()
    {
        Points = new(m_points);
        m_telegraphImage.color = m_telegraphPulseMin;
    }


    private void Update()
    {
        // Default case
        if (!m_telegraphLocked && !m_telegraphPulsing)
            m_telegraphImage.color = m_telegraphPulseMin;

        // Locked case overrides Default case
        if (m_telegraphLocked)
            m_telegraphImage.color = m_telegraphPulseMax;

        // Pulsing case overrides Locked case
        if (m_telegraphPulsing)
        {
            float t;
            Color lerpTo;
            if (m_pulseUpElseDown)
            {
                t = m_pulseUpElapsed / PULSE_TIME;
                lerpTo = m_telegraphPulseMax;
                m_pulseUpElapsed += Time.deltaTime;
            }
            else
            {
                t = m_pulseDownElapsed / PULSE_TIME;
                lerpTo = m_telegraphPulseMin;
                m_pulseDownElapsed += Time.deltaTime;
            }

            m_telegraphImage.color = Color.Lerp(m_telegraphImage.color, lerpTo, t);

            // Flip direction of lerp when the end is reached
            if (m_telegraphImage.color == lerpTo)
            {
                m_pulseUpElseDown = !m_pulseUpElseDown;
                if (m_pulseUpElseDown) m_pulseUpElapsed = 0;
                else m_pulseDownElapsed = 0;
            }
        }
    }


    public void SetPulsing(bool pulsing)
    {
        m_telegraphPulsing = pulsing;
    }


    public void SetLocked(bool locked)
    {
        m_telegraphLocked = locked;
    }
}