using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum Actions
{
    ClimbDown,
    PickUp,
    Save,
    OpenCloseFurnace,
    SetFurnaceGem,
    RefillFurnaceCoal,
    PutSomethingInsideFurnace,
    OpenCloseFurnacePour,
    DisplayWeapon,
    NaN
}

public class InteractionIdentifier : MonoBehaviour
{
    public Actions action;
    public string interactionPrompt;

    private void Start()
    {
        
    }

    public void HandleInteraction(PlayerManager interactingPlayer)
    {
        switch (action)
        {
            case Actions.ClimbDown:
                ClimbDownAction(interactingPlayer);
                Debug.Log("Bajar");
                break;
            case Actions.PickUp:
                PickUpItemAction(interactingPlayer);
                Debug.Log("Recogio objeto");
                break;
            case Actions.Save:
                SaveGameAction(interactingPlayer);
                Debug.Log("Guardar progreso");
                break;
            case Actions.OpenCloseFurnace:
                OpenCloseFurnaceAction(interactingPlayer);
                Debug.Log("Abrir/Cerrar forja");
                break;
            case Actions.SetFurnaceGem:
                SetFurnaceGem(interactingPlayer);
                Debug.Log("Coloacar gema de forja");
                break;
            case Actions.RefillFurnaceCoal:
                Debug.Log("Rellenar de carbon forja");
                break;
            case Actions.PutSomethingInsideFurnace:
                Debug.Log("Meter algo a la forja");
                    break;
            case Actions.OpenCloseFurnacePour:
                OpenCloseFurnacePourAction(interactingPlayer);
                Debug.Log("Se abrio/cerro la trampilla para vertit metal");
                break;
            case Actions.DisplayWeapon:
                DisplayWeapon(interactingPlayer);
                Debug.Log("Se va escoger una arma para poner en display");
                break;
            case Actions.NaN:
                Debug.Log("No hay accion valida");
                break;
            default:
                break;
        }
    }

    public void ClimbDownAction(PlayerManager player)
    {
        PlayerMovementController playerMovement = player.GetComponent<PlayerMovementController>();
        playerMovement.ClimbDownLedge();
    }

    public void SaveGameAction(PlayerManager player)
    {
        SavesController savesController = FindObjectOfType<SavesController>();
        savesController.SaveGame();
    }

    public void PickUpItemAction(PlayerManager player)
    {
        PickableItem pickableItem = GetComponent<PickableItem>();
        pickableItem.PickUpItem(player);
    }

    public void OpenCloseFurnaceAction(PlayerManager player)
    {
        FurnaceController furnaceController = GetComponentInParent<FurnaceController>();
        if (!furnaceController.isOpen)
            furnaceController.OpenDoor(player);
        else
            furnaceController.CloseDoor(player);
    }

    public void OpenCloseFurnacePourAction(PlayerManager player)
    {
        FurnaceController furnaceController = GetComponentInParent<FurnaceController>();
        if (!furnaceController.isPourOpen)
            furnaceController.OpenPour(player);
        else
            furnaceController.ClosePour(player);
    }

    public void SetFurnaceGem(PlayerManager player)
    {
        FurnaceController furnaceController = GetComponentInParent<FurnaceController>();
        player.interactingObject = furnaceController.gameObject;
        furnaceController.OpenObjectGemSlot(player);
    }

    public void DisplayWeapon(PlayerManager player)
    {
        WeaponDisplayerManager weaponDisplayerManager = GetComponent<WeaponDisplayerManager>();
        player.interactingObject = weaponDisplayerManager.gameObject;
        weaponDisplayerManager.OpenWeaponSlot(player);
    }
}