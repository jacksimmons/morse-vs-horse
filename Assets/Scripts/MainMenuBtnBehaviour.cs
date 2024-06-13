using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBtnBehaviour : MonoBehaviour
{
    public void OnMainMenuBtnClicked()
    {
        SceneManager.LoadScene("Main");
    }
}
