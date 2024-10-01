using UnityEngine;

public class LevelSelectBehaviour : MonoBehaviour
{
    private bool m_initialised = false;

    public virtual bool Init(int _)
    {
        if (m_initialised)
        {
            Debug.LogWarning("Already initialised.");
            return false;
        }
        m_initialised = true;
        return true;
    }
}