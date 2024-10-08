using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButcherBehaviour : MonoBehaviour
{
    /// <summary>
    /// The GameObjects to butcher. These drift to the right until they reach the butcher, then explode.
    /// </summary>
    [SerializeField]
    private List<GameObject> m_objsToButcher;

    private const float BUTCHERED_SPEED = 0.05f;


    private void Awake()
    {
        // Level selected will still be the level just beaten/lost to
        int livesLeft = SaveData.Instance.completionRanks[SaveData.Instance.levelSelected];

        // If butchering the horses...
        if (livesLeft > 0)
        {
            // Hide all horses not to be executed (only execute n horses, where n is the completion rank)
            for (int i = livesLeft; i < 3; i++)
            {
                m_objsToButcher[i].SetActive(false);
                m_objsToButcher.RemoveAt(i);
            }
        }
        // Otherwise we are butchering the king, who is always butchered here
    }


    private void Update()
    {
        foreach (GameObject go in m_objsToButcher)
        {
            // If the GameObject has already exploded, just continue
            if (go == null) continue;

            if (go.transform.position.x < transform.position.x)
            {
                go.transform.position += new Vector3(1, 0) * BUTCHERED_SPEED;
            }
            else
            {
                Butcher(go);

                // Break to not mess up loop iteration
                break;
            }
        }
    }


    /// <summary>
    /// Explodes the GameObject provided.
    /// </summary>
    private void Butcher(GameObject go)
    {
        Destroy(go);
        GetComponent<ParticleSystem>().Play();
    }
}
