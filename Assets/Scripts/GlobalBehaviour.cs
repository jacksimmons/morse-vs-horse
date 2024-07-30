using UnityEngine;


public class GlobalBehaviour : MonoBehaviour
{
    private static GlobalBehaviour m_inst;
    public static GlobalBehaviour Instance
    {
        get
        {
            if (m_inst == null)
            {
                m_inst = GameObject.Find("Global").GetComponent<GlobalBehaviour>();
            }
            return m_inst;
        }
    }


    public int Level { get; set; }
    public bool Endless { get; set; }


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}