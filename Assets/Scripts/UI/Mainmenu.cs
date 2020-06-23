using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
    public void MultiPlay()
    {
        SceneManager.LoadScene("Main");
    }

    public void PlayWithBot()
    {
        SceneManager.LoadScene("Bot");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeDifficulty(int value)
    {
        DifficultyManager.difficulty = value;
    }
}
