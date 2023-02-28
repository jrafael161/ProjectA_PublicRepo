using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisions : MonoBehaviour
{
    // Start is called before the first frame update
    public Collider characterCollider;
    public Collider characterColliderBlocker;
    void Start()
    {

        Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
    }

}
