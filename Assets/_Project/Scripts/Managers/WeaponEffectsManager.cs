using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffectsManager : MonoBehaviour
{
    [SerializeField]
    GameObject ModelFX;

    [SerializeField]
    ParticleSystem ModelTrails;
    [SerializeField]
    ParticleSystem ModelParticles;
    [SerializeField]
    MeshRenderer ModelGlow;
    
    public GameObject WeaponSlashFX;

    public ElementTypes elementType = ElementTypes.None;
    public ElementLevel elementLevel = ElementLevel.Level0;

    void Start()
    {
        if (GetComponentInParent<PickableItem>() != null)
        {
            Item item = GetComponentInParent<PickableItem>().item;
            if (item != null)
            {
                UpdateItemFX(item);
            }
        }
    }

    public void UpdateItemFX(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Consumable:
                break;
            case ItemType.KeyItem:
                break;
            case ItemType.Gem:
                break;
            case ItemType.Weapon:
                UpdateWeaponFX(item as WeaponItem);
                break;
            case ItemType.Ammo:
                break;
            case ItemType.Material:
                break;
            case ItemType.Trinket:
                break;
            default:
                break;
        }
    }

    public void UpdateWeaponFX(WeaponItem weaponItem)
    {
        List<GemItem> gems = new List<GemItem>();

        for (int i = 0; i < weaponItem.gemSockets.Length; i++)
        {
            if (weaponItem.gemSockets[i] != null)
            {
                gems.Add(weaponItem.gemSockets[i]);
            }
        }

        if (gems.Count < 1)
        {
            elementType = ElementTypes.None;
            elementLevel = ElementLevel.Level0;
            if (GetComponentInChildren<ElementsInteractionHandler>(true) != null)
            {
                GetComponentInChildren<ElementsInteractionHandler>(true).SetElementType(elementType);
                GetComponentInChildren<ElementsInteractionHandler>(true).SetElementLevel(elementLevel);
            }
            ModelFX.SetActive(false);
            return;
        }

        ModelFX.SetActive(true);

        DetermineElementsEffects(gems);
    }

    public void UpdateWeaponFX(ElementTypes newElementType, ElementLevel elementLevel)
    {
        //Color FXColor = ElementColorTable(newElementType);
        Color FXColor = AssetsDatabaseManager._instance.GetElementColor(newElementType);
        switch (elementLevel)
        {
            case ElementLevel.Level0:
                this.elementLevel = ElementLevel.Level0;
                if (GetComponentInChildren<ElementsInteractionHandler>(true) != null)
                {
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementType(newElementType);
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementLevel(elementLevel);
                }
                break;

            case ElementLevel.Level1:

                ModelGlow.gameObject.SetActive(true);
                ModelGlow.GetComponent<Renderer>().material.SetColor("_Color", FXColor);
                ModelGlow.GetComponent<Renderer>().material.SetFloat("_AlphaTransparency", 0.15f);

                ModelParticles.gameObject.SetActive(true);
                var particlesColor = ModelParticles.main;
                particlesColor.startColor = FXColor;

                ModelTrails.gameObject.SetActive(false);

                if (GetComponentInChildren<ElementsInteractionHandler>(true) != null)
                {
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementType(newElementType);
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementLevel(elementLevel);
                }
                break;

            case ElementLevel.Level2:
                ModelGlow.gameObject.SetActive(true);
                ModelGlow.GetComponent<Renderer>().material.SetColor("_Color", FXColor);
                ModelGlow.GetComponent<Renderer>().material.SetFloat("_AlphaTransparency", 0.3f);

                ModelParticles.gameObject.SetActive(true);
                var particlesColor2 = ModelParticles.main;
                particlesColor2.startColor = FXColor;

                ModelTrails.gameObject.SetActive(true);
                var trailsColor2 = ModelTrails.trails;
                trailsColor2.colorOverLifetime = FXColor;

                if (GetComponentInChildren<WeaponSlashBehaviour>(true) != null)
                {
                    GetComponentInChildren<WeaponSlashBehaviour>(true).UpdateFXElement(FXColor, newElementType, elementLevel);
                }
                if (GetComponentInChildren<ElementsInteractionHandler>(true) != null)
                {
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementType(newElementType);
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementLevel(elementLevel);
                }

                break;

            case ElementLevel.Level3:
                ModelGlow.gameObject.SetActive(true);
                ModelGlow.GetComponent<Renderer>().material.SetColor("_Color", FXColor);
                ModelGlow.GetComponent<Renderer>().material.SetFloat("_AlphaTransparency", 0.6f);

                ModelParticles.gameObject.SetActive(true);
                var particlesColor3 = ModelParticles.main;
                particlesColor3.startColor = FXColor;

                ModelTrails.gameObject.SetActive(true);
                var trailsColor3 = ModelTrails.trails;
                trailsColor3.colorOverLifetime = FXColor;

                if (GetComponentInChildren<WeaponSlashBehaviour>(true) != null)
                {
                    GetComponentInChildren<WeaponSlashBehaviour>(true).UpdateFXElement(FXColor, newElementType, elementLevel);
                }
                if (GetComponentInChildren<ElementsInteractionHandler>(true) != null)
                {
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementType(newElementType);
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementLevel(elementLevel);
                }

                break;

            default:
                break;
        }
    }

    private void DetermineElementsEffects(List<GemItem> gems)
    {
        for (int i = 0; i < gems.Count; i++)
        {
            if (i == 0)
            {
                Color FXColor = GemsElementColorTable(gems[0].gemType);

                ModelGlow.gameObject.SetActive(true);
                ModelGlow.GetComponent<Renderer>().material.SetColor("_Color", FXColor);
                ModelGlow.GetComponent<Renderer>().material.SetFloat("_AlphaTransparency", 0.15f);

                ModelParticles.gameObject.SetActive(true);
                var particlesColor = ModelParticles.main;
                particlesColor.startColor = FXColor;

                ModelTrails.gameObject.SetActive(false);

                if (GetComponentInChildren<ElementsInteractionHandler>(true) != null)
                {
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementType(elementType);
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementLevel(elementLevel);
                }
            }
            if (i == 1)
            {
                Color FXColor = DoubleElementCombinationColorTable(gems[0].gemType, gems[1].gemType);

                ModelGlow.gameObject.SetActive(true);
                ModelGlow.GetComponent<Renderer>().material.SetColor("_Color", FXColor);
                ModelGlow.GetComponent<Renderer>().material.SetFloat("_AlphaTransparency", 0.3f);

                ModelParticles.gameObject.SetActive(true);
                var particlesColor = ModelParticles.main;
                particlesColor.startColor = FXColor;

                ModelTrails.gameObject.SetActive(true);
                var trailsColor = ModelTrails.trails;
                trailsColor.colorOverLifetime = FXColor;

                if (GetComponentInChildren<WeaponSlashBehaviour>(true) != null)
                {
                    GetComponentInChildren<WeaponSlashBehaviour>(true).UpdateFXElement(FXColor,elementType, elementLevel);
                }
                if (GetComponentInChildren<ElementsInteractionHandler>(true) != null)
                {
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementType(elementType);
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementLevel(elementLevel);
                }
            }
            if (i == 2)
            {
                Color FXColor = TripleElementCombinationColorTable(gems[0].gemType, gems[1].gemType, gems[2].gemType);

                ModelGlow.gameObject.SetActive(true);
                ModelGlow.GetComponent<Renderer>().material.SetColor("_Color", FXColor);
                ModelGlow.GetComponent<Renderer>().material.SetFloat("_AlphaTransparency", 0.6f);

                ModelParticles.gameObject.SetActive(true);
                var particlesColor = ModelParticles.main;
                particlesColor.startColor = FXColor;

                ModelTrails.gameObject.SetActive(true);
                var trailsColor = ModelTrails.trails;
                trailsColor.colorOverLifetime = FXColor;

                if (GetComponentInChildren<WeaponSlashBehaviour>(true) != null)
                {
                    GetComponentInChildren<WeaponSlashBehaviour>(true).UpdateFXElement(FXColor,elementType, elementLevel);
                }
                if (GetComponentInChildren<ElementsInteractionHandler>(true) != null)
                {
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementType(elementType);
                    GetComponentInChildren<ElementsInteractionHandler>(true).SetElementLevel(elementLevel);
                }
            }
        }
    }

    public Color GemsElementColorTable(GemTypes gemType)
    {
        elementLevel = ElementLevel.Level1;

        switch (gemType)
        {
            case GemTypes.Fire:
                elementType = ElementTypes.Fire;
                break;

            case GemTypes.Water:
                elementType = ElementTypes.Water;
                break;

            case GemTypes.Electric:
                elementType = ElementTypes.Electric;
                break;

            case GemTypes.Rock:
                elementType = ElementTypes.Rock;
                break;

            case GemTypes.Wind:
                elementType = ElementTypes.Wind;
                break;

            default:
                elementLevel = ElementLevel.Level0;
                elementType = ElementTypes.None;
                break;
        }

        return AssetsDatabaseManager._instance.GetElementColor(elementType);
    }

    public Color ElementColorTable(ElementTypes elementTypes)
    {
        return AssetsDatabaseManager._instance.GetElementColor(elementTypes); ;
    }

    private Color DoubleElementCombinationColorTable(GemTypes gemType1, GemTypes gemType2)
    {
        elementLevel = ElementLevel.Level2;

        switch (gemType1, gemType2)
        {
            case (GemTypes.Fire,GemTypes.Fire):
                elementType = ElementTypes.Fire;
                break;

            case (GemTypes.Fire, GemTypes.Water):
                elementType = ElementTypes.Steam;
                break;

            case (GemTypes.Fire, GemTypes.Electric):
                elementType = ElementTypes.Plasma;
                break;

            case (GemTypes.Fire, GemTypes.Rock):
                elementType = ElementTypes.Lava;
                break;

            case (GemTypes.Fire, GemTypes.Wind):
                elementType = ElementTypes.Explosive;
                break;


            case (GemTypes.Water, GemTypes.Fire):
                elementType = ElementTypes.Steam;
                break;

            case (GemTypes.Water, GemTypes.Water):
                elementType = ElementTypes.Water;
                break;

            case (GemTypes.Water, GemTypes.Electric):
                elementType = ElementTypes.Laser;
                break;

            case (GemTypes.Water, GemTypes.Rock):
                elementType = ElementTypes.Plants;
                break;

            case (GemTypes.Water, GemTypes.Wind):
                elementType = ElementTypes.Ice;
                break;


            case (GemTypes.Electric, GemTypes.Fire):
                elementType = ElementTypes.Plasma;
                break;

            case (GemTypes.Electric, GemTypes.Water):
                elementType = ElementTypes.Laser;
                break;

            case (GemTypes.Electric, GemTypes.Electric):
                elementType = ElementTypes.Electric;
                break;

            case (GemTypes.Electric, GemTypes.Rock):
                elementType = ElementTypes.Magnetism;
                break;

            case (GemTypes.Electric, GemTypes.Wind):
                elementType = ElementTypes.Storm;
                break;


            case (GemTypes.Rock, GemTypes.Fire):
                elementType = ElementTypes.Lava;
                break;

            case (GemTypes.Rock, GemTypes.Water):
                elementType = ElementTypes.Plants;
                break;

            case (GemTypes.Rock, GemTypes.Electric):
                elementType = ElementTypes.Magnetism;
                break;

            case (GemTypes.Rock, GemTypes.Rock):
                elementType = ElementTypes.Rock;
                break;

            case (GemTypes.Rock, GemTypes.Wind):
                elementType = ElementTypes.Dust;
                break;


            case (GemTypes.Wind, GemTypes.Fire):
                elementType = ElementTypes.Explosive;
                break;

            case (GemTypes.Wind, GemTypes.Water):
                elementType = ElementTypes.Ice;
                break;

            case (GemTypes.Wind, GemTypes.Electric):
                elementType = ElementTypes.Storm;
                break;

            case (GemTypes.Wind, GemTypes.Rock):
                elementType = ElementTypes.Dust;
                break;

            case (GemTypes.Wind, GemTypes.Wind):
                elementType = ElementTypes.Wind;
                break;

            default:
                elementLevel = ElementLevel.Level0;
                elementType = ElementTypes.None;
                break;
        }

        return AssetsDatabaseManager._instance.GetElementColor(elementType);
    }

    private Color TripleElementCombinationColorTable(GemTypes gemType1, GemTypes gemType2, GemTypes gemType3)
    {
        elementLevel = ElementLevel.Level3;
        switch (gemType1, gemType2, gemType3)
        {
            case (GemTypes.Fire, GemTypes.Fire, GemTypes.Fire):
                elementType = ElementTypes.Fire;
                break;

            case (GemTypes.Water, GemTypes.Water, GemTypes.Water):
                elementType = ElementTypes.Water;
                break;

            case (GemTypes.Electric, GemTypes.Electric, GemTypes.Electric):
                elementType = ElementTypes.Electric;
                break;

            case (GemTypes.Rock, GemTypes.Rock, GemTypes.Rock):
                elementType = ElementTypes.Rock;
                break;

            case (GemTypes.Wind, GemTypes.Wind, GemTypes.Wind):
                elementType = ElementTypes.Wind;
                break;

            default:
                elementLevel = ElementLevel.Level0;
                break;
        }

        return AssetsDatabaseManager._instance.GetElementColor(elementType);
    }

    public void DisableAllFX()
    {
        ModelFX.SetActive(false);
    }
}