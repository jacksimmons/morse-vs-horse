using System.Collections.Generic;
using UnityEngine;


public class MessengerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_cityParent;

    public MessengerBehaviour[] Messengers
    {
        get
        {
            // Get all messengers.
            List<MessengerBehaviour> mbs = new();
            foreach (Transform cityT in m_cityParent.transform)
            {
                // This will be null for all inactive messengers.
                MessengerBehaviour mb = cityT.GetComponentInChildren<MessengerBehaviour>();
                mbs.Add(mb);
            }
            return mbs.ToArray();
        }
    }
}