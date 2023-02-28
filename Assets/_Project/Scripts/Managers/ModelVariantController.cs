using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelVariantController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> ModelVariants;

    void Start()
    {
        EnableRandomModel();
    }

    public void EnableRandomModel()
    {
        int index = Random.Range(0, ModelVariants.Count);
        ModelVariants[0].SetActive(false);
        ModelVariants[index].SetActive(true);
    }
}
