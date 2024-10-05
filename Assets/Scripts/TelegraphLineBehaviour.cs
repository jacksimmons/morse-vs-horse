using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public enum TelegraphPulse { Off, Pulsing, On }


public class TelegraphLineBehaviour : MonoBehaviour
{
    [SerializeField]
    private Image m_telegraphImage;
    private TelegraphPulse m_telegraphPulsing = TelegraphPulse.Off;
    public TelegraphPulse Pulse
    {
        get
        {
            return m_telegraphPulsing;
        }
        set
        {
            UpdateTelegraphLayer(value != TelegraphPulse.Off);
            m_telegraphPulsing = value;
        }
    }
    private static readonly Color m_telegraphPulseMin = Color.white;
    private static readonly Color m_telegraphPulseMax = Color.red;
    private float m_pulseUpElapsed = 0;
    private float m_pulseDownElapsed = 0;
    private bool m_pulseUpElseDown = true;
    private const float PULSE_TIME = 2f;


    private void Awake()
    {
        m_telegraphImage.color = m_telegraphPulseMin;
    }


    private void Update()
    {
        // Off case
        if (m_telegraphPulsing == TelegraphPulse.Off)
            m_telegraphImage.color = m_telegraphPulseMin;

        // Pulsing case
        else if (m_telegraphPulsing == TelegraphPulse.Pulsing)
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

        // On case
        else
            m_telegraphImage.color = m_telegraphPulseMax;
    }


    /// <summary>
    /// Moves a telegraph image to the front if it is pulsing/locked.
    /// Whenever a telegraph image is viewed, it is always in front.
    /// </summary>
    private void UpdateTelegraphLayer(bool infront)
    {
        if (infront)
            m_telegraphImage.transform.SetAsLastSibling();
    }
}