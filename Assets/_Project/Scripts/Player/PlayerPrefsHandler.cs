using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsHandler : MonoBehaviour
{
    PlayerPrefsData playerPrefsData = new PlayerPrefsData();    
}

public class PlayerPrefsData
{
    public int screenResX;
    public int screenResY;
    public int refreshRate;
    public int mainVolume;
    public int soundVolume;
    public int vfxVolume;
}