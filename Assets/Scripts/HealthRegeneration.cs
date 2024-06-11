using UnityEngine;

public class HealthRegeneration : MonoBehaviour
{
    public float healthRegenRate = 1f; // Скорость восстановления здоровья
    public float regenDelay = 2f; // Задержка перед началом восстановления
    public GameObject regenPlatform; // Игровой объект, представляющий платформу восстановления

    private float currentHealth;
    private float maxHealth = 100f;
    private float regenTimer;
    private bool isOnRegenPlatform = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        isOnRegenPlatform = CheckIfOnRegenPlatform();
        
        if (isOnRegenPlatform)
        {
            RegenHealth();
        }
    }

    private void RegenHealth()
    {
        if (regenTimer <= 0f)
        {
            currentHealth = Mathf.Min(currentHealth + healthRegenRate * Time.deltaTime, maxHealth);
        }
        else
        {
            regenTimer -= Time.deltaTime;
        }
    }

    private bool CheckIfOnRegenPlatform()
    {
        return transform.position.y <= regenPlatform.transform.position.y + 0.1f;
    }
}