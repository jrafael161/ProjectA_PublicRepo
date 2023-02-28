using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    Animator animator;
    EnemyManager enemyManager;
    EnemyWeaponSlotManager enemyWeapon;
    float staminaRegenTimer = 0;
    public float staminaRegenAmount = 30;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        enemyWeapon = GetComponentInChildren<EnemyWeaponSlotManager>();
        enemyManager = GetComponent<EnemyManager>();
        maxHealth = SeMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
        //healthBar.SetMaxHealth(maxHealth);
    }

    private int SeMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;//This obviously needs to follow a curve like progression
        enemyManager.enemyHealthBarUI.SetMaxHealth(maxHealth);
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth = currentHealth - damage;
        enemyManager.enemyHealthBarUI.SetHealth(currentHealth);
        //healthBar.SetCurrentHealth(currentHealth);
        animator.Play("GettingHurt");
        enemyWeapon.DisableHandDamageCollider();

        if (currentHealth <= 0)
        {    
            currentHealth = 0;
            isDead = true;
            animator.Play("Dying");
        }
    }

    public void ConsumeStamina(int consumption)
    {
        currentStamina = currentStamina - consumption;
        if (currentStamina <= 0)
        {
            currentStamina = 0;
        }
    }

    public void RegenerateStamina()
    {
        if (enemyManager.isInteracting)
        {
            staminaRegenTimer = 0;
            return;
        }

        staminaRegenTimer += Time.deltaTime;

        if (currentStamina <= maxStamina && staminaRegenTimer > 1f)
        {
            currentStamina += staminaRegenAmount * Time.deltaTime;
        }
    }
}
