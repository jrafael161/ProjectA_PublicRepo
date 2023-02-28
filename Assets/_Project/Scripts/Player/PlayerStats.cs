using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    PlayerManager playerManager;

    public HealthBarManager healthBar;
    public StaminaBarManager staminaBar;

    PlayerAnimController playerAnim;

    public float staminaRegenAmount = 30;
    float staminaRegenTimer = 0;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        playerAnim = GetComponentInChildren<PlayerAnimController>();        

        maxHealth = SeMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
        if (healthBar == null)
        {
            healthBar = playerManager.UIManager.GetComponentInChildren<HealthBarManager>();
        }
        healthBar.SetMaxHealth(maxHealth);

        if (staminaBar == null)
        {
            staminaBar = playerManager.UIManager.GetComponentInChildren<StaminaBarManager>();
        }
        maxStamina = SeMaxStaminaFromHealthLevel();
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
    }

    private int SeMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;//This obviously needs to follow a curve like progression
        return maxHealth;
    }

    private float SeMaxStaminaFromHealthLevel()
    {
        maxStamina = staminaLevel* 10;//This obviously needs to follow a curve like progression
        return maxStamina;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        if (playerManager.isInvulnerable)
            return;

        playerAnim.anim.SetBool("isInvulnerable", true);
        StartCoroutine("IFrames");

        currentHealth = currentHealth - damage;
        healthBar.SetCurrentHealth(currentHealth);
        playerAnim.playTargetAnimation("GettingHurt",true);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            CapsuleCollider collider = GetComponentInChildren<CapsuleCollider>();
            collider.enabled = false;
            isDead = true;
            playerManager.ChangePlayerState(PlayerState.DEAD);
            playerAnim.anim.SetBool("isInvulnerable", true);
            playerAnim.playTargetAnimation("Dying", true, false);
            UIManager uIManager = playerManager.UIManager;
            uIManager.DeadScreen.SetActive(true);
            StartCoroutine("FadeDeathScreen");
        }

        
    }

    public void ConsumeStamina(int consumption)
    {
        currentStamina = currentStamina - consumption;
        if (currentStamina <= 0)
        {
            currentStamina = 0;
        }
        staminaBar.SetCurrentStamina(currentStamina);
    }

    public void RegenerateStamina()
    {
        if (playerManager.isInteracting)
        {
            staminaRegenTimer = 0;
            return;
        }            

        staminaRegenTimer += Time.deltaTime;

        if (currentStamina <= maxStamina && staminaRegenTimer > 1f)
        {
            currentStamina += staminaRegenAmount * Time.deltaTime;
            staminaBar.SetCurrentStamina(currentStamina);
        }
    }

    IEnumerator FadeDeathScreen()
    {
        UIManager uIManager = playerManager.UIManager;
        UnityEngine.UI.Image deathScreen = uIManager.DeadScreen.GetComponentInChildren<UnityEngine.UI.Image>();
        Color c = deathScreen.color;
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= 0.1f;
            c.a = alpha;
            deathScreen.color = c;
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(5);
        GameController._instance.RespawnPlayerAfterDeath();
    }

    IEnumerator IFrames()//After taking damage the player canot take damage until the next frame
    {
        yield return new WaitForSeconds(2);//In theory it should last the duration of the damaged animation and a little more to give the player a little room to leave the source of damage
        playerAnim.anim.SetBool("isInvulnerable", false);
    }
}
