using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Asegúrate de incluir esto

public class SimpleHealthSystem : MonoBehaviour
{
    [Header("Configuración Básica")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Debug")]
    [SerializeField] private bool showLogs = true;

    public event System.Action<int> OnHealthChanged; 
    public event System.Action OnDeath;
    public event System.Action OnDamageTaken;
    public HealthBar healthBar;
    public event System.Action OnHealed;

    private void Awake()
    {
        currentHealth = maxHealth;
        Log("Sistema de vida inicializado. Salud: " + currentHealth + "/" + maxHealth);
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Log("Daño recibido: " + damage + ". Salud restante: " + currentHealth);
        healthBar.SetHealth(currentHealth);
        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
            SceneManager.LoadScene("DefeatScene");
        }
    }

    public void Heal(int healAmount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        Log("Curado: +" + healAmount + ". Salud actual: " + currentHealth);

        OnHealthChanged?.Invoke(currentHealth);
        OnHealed?.Invoke();
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    private void Die()
    {
        Log("¡Jugador muerto!");
        OnDeath?.Invoke();
    }

    private void Log(string message)
    {
        if (showLogs)
        {
            Debug.Log("[Sistema de Vida] " + message);
        }
    }
}
