using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBehaviour : MonoBehaviour
{
    private Animator m_animator;

    [SerializeField]
    private GameObject m_staffOnGround;


    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_animator.speed = 0.2f;
    }


    public void DropStaff()
    {
        m_staffOnGround.SetActive(true);
    }
}
