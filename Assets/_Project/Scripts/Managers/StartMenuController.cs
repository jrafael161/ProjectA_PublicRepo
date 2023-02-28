using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [Header("Continue Game Button")]
    [SerializeField]
    Button ContinueGameBtn;

    [Header("Load Game Button")]
    [SerializeField]
    Button LoadGameBtn;

    [Header("New Game Button")]
    [SerializeField]
    Button NewGameBtn;
    
    void Start()
    {
        EnableInteraction();
    }

    private void OnEnable()
    {
        if (SavesController._instance!=null)
        {
            EnableInteraction();
        }        
    }

    void EnableInteraction()
    {
        if (SavesController._instance.CheckLastSave() >= 0)
        {
            ContinueGameBtn.interactable = true;
            ContinueGameBtn.Select();
        }
        else
        {
            ContinueGameBtn.interactable = false;
        }

        if (SavesController._instance.GetAllSaves().Count > 0)
        {
            LoadGameBtn.interactable = true;
            return;
        }
        else
        {
            LoadGameBtn.interactable = false;
        }

        NewGameBtn.Select();
    }
}
