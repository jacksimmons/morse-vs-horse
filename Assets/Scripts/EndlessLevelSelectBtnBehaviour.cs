using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EndlessLevelSelectBtnBehaviour : MonoBehaviour
{
    [SerializeField]
    private int m_levelRequired;
    [SerializeField]
    private int m_levelStart;
    [SerializeField]
    private Image m_locked;


    private void Awake()
    {
        if (SaveData.Instance.highestLevelBeaten >= m_levelRequired)
        {
            m_locked.enabled = false;
        }
    }


    public void OnBtnClicked()
    {
        SaveData.Instance.levelSelected = m_levelStart;
        SaveData.Instance.endlessSelected = true;
        SceneManager.LoadScene("Game");
    }
}