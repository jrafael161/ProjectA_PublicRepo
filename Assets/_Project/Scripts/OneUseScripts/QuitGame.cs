using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void CloseGame()
    {
        GameController._instance.QuitGame();
    }

    public void ReturnToMainMenu()
    {
        GameController._instance.ReturnToMainMenu();
    }
}
