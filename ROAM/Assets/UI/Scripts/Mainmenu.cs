using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Mainmenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void ChangeScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CreditScene()
    {
        SceneManager.LoadScene("CreditScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
