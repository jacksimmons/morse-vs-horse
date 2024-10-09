using UnityEngine;


public class KingStaffBehaviour : MonoBehaviour
{
    private bool m_dropping = false;


    private void Update()
    {
        if (m_dropping)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, -4), 1);
        }
    }


    public void Drop()
    {
        m_dropping = true;
    }
}