using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PlayerInteraction : MonoBehaviour
{
    InputHandler inputHandler;
    UIManager uIManager;

    public List<PickableItem> pickableObjects;
    public List<InteractionIdentifier> interactables;//I need to make an event to update the interaction prompt as soon as this list changes
    public TMP_Text interactionText;

    public Transform interactVolume;

    [SerializeField]
    int interactionIndex = 0;

    private void Start()
    {
        pickableObjects = new List<PickableItem>();
        inputHandler = GetComponentInParent<InputHandler>();
        uIManager = GetComponentInParent<PlayerManager>().UIManager;
    }
    public void ChecForPlayerAction()
    {
        if (uIManager.interactionMessagePrompt.gameObject.activeSelf)
        {
            if (inputHandler.a_Btn_Input)
            {
                PerformPlayerAction();
            }
            if (inputHandler.Dpad_Down_Input)
            {
                if (interactables.Count > 1)
                {
                    ShowNextInteractionPrompt();
                }
            }
        }
    }

    public void PerformPlayerAction()
    {
        if (interactionIndex <= interactables.Count)
        {
            interactables[interactionIndex].HandleInteraction(GetComponentInParent<PlayerManager>());
        }
        UpdateInteractables();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Interactable")
        {
            Debug.Log("hay " + interactables.Count + "objetos interactuables en rango");
            if (!interactables.Contains(other.GetComponent<InteractionIdentifier>()))
                interactables.Add(other.GetComponent<InteractionIdentifier>());

            if (other.GetComponentInChildren<PickableItem>() != null)
            {
                if (!pickableObjects.Contains(other.GetComponentInChildren<PickableItem>()))
                {
                    pickableObjects.Add(other.GetComponentInChildren<PickableItem>());
                }
            }

            if (interactables.Count > 1)
            {
                uIManager.interactionMessageTogglePrompt.SetActive(true);
            }

            interactionIndex = interactables.Count - 1;
            ShowInteractionPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Interactable")
        {
            Debug.Log("salio un objeto interactuable de rango");
            interactables.Remove(other.GetComponent<InteractionIdentifier>());

            if (other.GetComponentInChildren<PickableItem>() != null)
            {
                pickableObjects.Remove(other.GetComponentInChildren<PickableItem>());
            }

            if (interactables.Count <= 1)
            {
                uIManager.interactionMessageTogglePrompt.SetActive(false);
            }

            if (interactables.Count >= 1)
            {
                //interactionText.text = interactables[interactables.Count - 1].GetComponent<InteractionIdentifier>().interactionPrompt;
                interactionIndex = interactables.Count - 1;
                ShowInteractionPrompt();
            }
            else
            {
                HideInteractionPrompt();
            }
        }
    }


    public void UpdateInteractables()
    {
    //Collider[] insideColliders = Physics.OverlapBox(interactVolume.position + new Vector3(0, 1, 0), new Vector3(2, 2, 2));
    //List<InteractionIdentifier> insideObjects = new List<InteractionIdentifier>();
    //foreach (Collider insideObject in insideColliders)
    //{
    //    if (insideObject.GetComponent<InteractionIdentifier>() != null)
    //    {
    //        insideObjects.Add(insideObject.GetComponent<InteractionIdentifier>());
    //    }
    //}

    //if (insideObjects.Count < 1)
    //{            
    //    interactables.Clear();
    //    HideInteractionPrompt();
    //}
    //else
    //{
    //    x:
    //    foreach (InteractionIdentifier interactable in interactables)
    //    {
    //        if (!insideObjects.Contains(interactable))
    //        {
    //            interactables.Remove(interactable);
    //            if (insideObjects.Count == interactables.Count)
    //            {
    //                break;
    //            }
    //            else
    //                goto x;
    //        }
    //    }

    //    if (interactables.Count >= 1)
    //    {
    //        interactionIndex = interactables.Count - 1;
    //        ShowInteractionPrompt();
    //    }
    //    else
    //    {
    //        HideInteractionPrompt();
    //    }
    //}
    //insideObjects.Clear();

    x:
        foreach (InteractionIdentifier interactable in interactables)
        {
            if (interactable == null)//I can check against null because most of the time when an interactable leaves the list by other means than exiting the collider is because the gameobject was destroyed
            {
                interactables.Remove(interactable);
                goto x;
            }
        }

    y:
        foreach (PickableItem pickable in pickableObjects)
        {
            if (pickable == null)
            {
                pickableObjects.Remove(pickable);
                goto y;
            }
        }

        if (interactables.Count >= 1)
        {
            interactionIndex = interactables.Count - 1;
            ShowInteractionPrompt();
            if (interactables.Count == 1)
            {
                uIManager.interactionMessageTogglePrompt.SetActive(false);
            }
        }
        else
        {
            HideInteractionPrompt();
        }

    }

    public void ShowInteractionPrompt()//Maybe move this to the pickable item script as with the other interactables
    {
        if (interactionIndex < 0)
        {
            interactionIndex = 0;
        }

        if (interactionIndex <= interactables.Count && interactables.Count > 0)
        {
            interactionText.text = interactables[interactionIndex].interactionPrompt;
            uIManager.interactionMessagePrompt.SetActive(true);
        }
    }

    public void HideInteractionPrompt()
    {
        uIManager.interactionMessagePrompt.SetActive(false);
        uIManager.interactionMessageTogglePrompt.SetActive(false);
    }

    public void StartDisableMessageCoroutine()
    {
        StartCoroutine("DisableAdquisitionMessage");
    }

    public void ShowNextInteractionPrompt()
    {
        interactionIndex++;
        if (interactionIndex >= interactables.Count)
        {
            interactionIndex = 0;
        }

        if (interactables.Count >=1)
        {
            ShowInteractionPrompt();
        }
        else
        {
            interactionIndex = 0;
            HideInteractionPrompt();
        }
    }

    IEnumerator DisableAdquisitionMessage()
    {
        yield return new WaitForSeconds(.5f);
        uIManager.itemAdquisitionMessagePrompt.SetActive(false);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(interactVolume.position + new Vector3(0,1,0), new Vector3(2,2,2));
    //}
}
