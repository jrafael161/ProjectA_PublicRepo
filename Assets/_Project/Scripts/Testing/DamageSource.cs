using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private int damage = 25;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerStats playerStats = other.GetComponentInParent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Player")
        {
            PlayerStats playerStats = other.GetComponentInParent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }
    }
}
