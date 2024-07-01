using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class TelegraphLineBehaviour : MonoBehaviour
{
    [SerializeField]
    private Image m_telegraphImage;
    private bool m_telegraphPulsing = false;
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
        // Default case
        if (!m_telegraphPulsing)
            m_telegraphImage.color = m_telegraphPulseMin;

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
        UpdateTelegraphLayer(pulsing);
        m_telegraphPulsing = pulsing;
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