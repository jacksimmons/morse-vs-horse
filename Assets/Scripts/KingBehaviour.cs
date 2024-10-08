using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBehaviour : MonoBehaviour
{
    private Animator m_animator;


    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_animator.speed = 0.2f;
    }
}
