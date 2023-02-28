using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAssigner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine("WaitForMeshComputation");
    }

    IEnumerator WaitForMeshComputation()//This is too costly in on the physics calculations taking up to 6-7 seconds to assign the collider, why maybe because is instanciating a new one?
    {
        yield return new WaitForSeconds(5);
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.mesh;
    }
}
