using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponSlashBehaviour : MonoBehaviour
{
    [SerializeField]
    VisualEffect VFX;
    [SerializeField]
    ParticleSystem ModelTrails;
    [SerializeField]
    ParticleSystem ModelParticles;
    [SerializeField]
    Rigidbody slashRigidbody;
    [SerializeField]
    LayerMask CollisionLayers;
    [SerializeField]
    ElementTypes elementType = ElementTypes.None;
    [SerializeField]
    ElementLevel elementLevel = ElementLevel.Level0;

    void Start()
    {
        //GetFlowSimulation();
        StartMovement();
    }

    public void UpdateFXElement(Color FXColor, ElementTypes newElementType, ElementLevel newElementLevel)
    {
        elementType = newElementType;
        elementLevel = newElementLevel;

        ElementsInteractionHandler elementsInteractionHandler = GetComponentInChildren<ElementsInteractionHandler>();
        elementsInteractionHandler.SetElementType(newElementType);
        elementsInteractionHandler.SetElementLevel(newElementLevel);

        VFX.SetVector4("SlashColor", new Vector4(FXColor.r, FXColor.g, FXColor.b,1));

        var particlesColor = ModelParticles.main;
        particlesColor.startColor = FXColor;

        var trailsColor = ModelTrails.trails;
        trailsColor.colorOverLifetime = FXColor;
    }

    public void StartMovement()
    {
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (CameraHandler._instance.currentCamera == CameraMode.LockOnCamera)
        {
            transform.LookAt(CameraHandler._instance.lockOnCamera.LookAt);
        }
        else
        {
            transform.rotation = Quaternion.Euler(CameraHandler._instance.cameraTransform.eulerAngles.x, CameraHandler._instance.cameraTransform.eulerAngles.y, 0);
        }
        slashRigidbody.AddForce(transform.forward * 5, ForceMode.Impulse);
        //slashRigidbody.AddForce(player.transform.forward * 5,ForceMode.Impulse);
    }

    //public void GetFlowSimulation()
    //{
    //    FLOW.FlowSimulation worldFlowSimulation = FindObjectOfType<FLOW.FlowSimulation>();
    //    FLOW.FlowSample weaponFlowSimulationSample = GetComponentInChildren<FLOW.FlowSample>();
    //    if (worldFlowSimulation != null && weaponFlowSimulationSample != null)
    //    {
    //        weaponFlowSimulationSample.Simulation = worldFlowSimulation;
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & CollisionLayers) != 0)
        {
            if (other.gameObject.tag!="Player" && other.gameObject.tag != "IgnoreCollider")
            {
                //ElementsInteractionHandler elementsInteractionHandler = other.GetComponent<ElementsInteractionHandler>();
                //if (elementsInteractionHandler != null)
                //{
                //    GetComponent<ElementsInteractionHandler>().HandleElementInteraction(elementsInteractionHandler.GetElementType(), elementsInteractionHandler.GetElementLevel());
                //}
                if (other.gameObject.GetComponentInChildren<ElementsInteractionHandler>() == null)//If the slash is water type and upon contact it doesnt react with anything spawn water
                {
                    InstatiateWater();
                }
                Destroy(gameObject);
            }
        }
    }

    public void EvaporateWater()//Can't move to element interaction handler, atm, because the water surface lacks a collider with wich to interact
    {
        if (elementType == ElementTypes.Fire)
        {
            GameObject originalSteamPrefab = AssetsDatabaseManager._instance.elementRections[(int)ElementTypes.Steam];
            GameObject instatiatedSteam = Instantiate(originalSteamPrefab, transform.position, Quaternion.identity, transform.parent);
            //FLOW.FlowModifier waterEvaporator = GetComponentInChildren<FLOW.FlowModifier>(true);
            //waterEvaporator.Mode = FLOW.FlowModifier.ModeType.RemoveFluid;
            //waterEvaporator.transform.parent = transform.parent;
            //waterEvaporator.gameObject.SetActive(true);
            //Destroy(waterEvaporator.gameObject, 1);
            Destroy(instatiatedSteam, 8);

            switch (elementLevel)
            {
                case ElementLevel.Level0:
                    break;
                case ElementLevel.Level1:
                    //waterEvaporator.Strength *= 5f;
                    Destroy(gameObject);
                    break;
                case ElementLevel.Level2:
                    //waterEvaporator.Strength *= 10;
                    Destroy(gameObject);
                    break;
                case ElementLevel.Level3:
                    //waterEvaporator.Strength *= 20;
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }
    }

    public void InstatiateWater()//Can't move to element interaction handler, atm, because the water surface lacks a collider with wich to interact
    {
        if (elementType == ElementTypes.Water)
        {
            //FLOW.FlowModifier waterCreator = GetComponentInChildren<FLOW.FlowModifier>(true);
            //waterCreator.transform.parent = transform.parent;
            //waterCreator.gameObject.SetActive(true);

            //Destroy(waterCreator.gameObject, 1);

            switch (elementLevel)
            {
                case ElementLevel.Level0:
                    break;
                case ElementLevel.Level1:
                    //waterCreator.Strength *= 1;
                    break;
                case ElementLevel.Level2:
                    //waterCreator.Strength *= 3;
                    break;
                case ElementLevel.Level3:
                    //waterCreator.Strength *= 6;
                    break;
                default:
                    break;
            }
        }
    }
}
