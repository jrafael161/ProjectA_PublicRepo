using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsDatabaseManager : MonoBehaviour
{
    public static AssetsDatabaseManager _instance;

    [SerializeField]
    public ItemsDatabase itemsDatabase;

    [SerializeField]
    public List<Color> elementColors;

    [SerializeField]
    public List<GameObject> elementRections;

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
        {
            _instance = this;
        }
    }

    public Color GetElementColor(ElementTypes element)
    {
        switch (element)
        {
            case ElementTypes.Fire:
                return elementColors[0];

            case ElementTypes.Water:
                return elementColors[1];

            case ElementTypes.Electric:
                return elementColors[2];

            case ElementTypes.Rock:
                return elementColors[3];

            case ElementTypes.Wind:
                return elementColors[4];

            case ElementTypes.Steam:
                return elementColors[5];

            case ElementTypes.Lava:
                return elementColors[6];

            case ElementTypes.Explosive:
                return elementColors[7];

            case ElementTypes.Plasma:
                return elementColors[8];

            case ElementTypes.Plants:
                return elementColors[9];

            case ElementTypes.Ice:
                return elementColors[10];

            case ElementTypes.Laser:
                return elementColors[11];

            case ElementTypes.Dust:
                return elementColors[12];

            case ElementTypes.Magnetism:
                return elementColors[13];

            case ElementTypes.Storm:
                return elementColors[14];

            case ElementTypes.Void:
                return elementColors[15];

            case ElementTypes.Life:
                return elementColors[16];

            case ElementTypes.Death:
                return elementColors[17];

            case ElementTypes.None:
                return Color.clear;

            default:
                return Color.clear;
        }
    }
}
