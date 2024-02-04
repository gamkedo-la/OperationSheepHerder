using UnityEngine;

public class Dragon : Enemy
{
    public override void TakeDamage(GameObject weapon, float damage)
    {
        currentHealth -= damage;
        
        uiHealthValue.value = currentHealth / maxHealth;

        if (uiHealthObject)
        {
            //if the enemy health is less than the max, turn the UI on
            if (currentHealth <= maxHealth)
            {
                uiHealthObject.SetActive(true);
                uiHealthObject.transform.LookAt(Camera.main.transform);
            }
            //if the enemy health is at (or greater than) max, turn the UI off
            else
            {
                uiHealthObject.SetActive(false);
            }
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
    }
}