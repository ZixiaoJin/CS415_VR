using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged;  // (current, max)
    public UnityEvent onDamaged;
    public UnityEvent onHealed;
    public UnityEvent onDeath;

    void Awake() => currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || currentHealth <= 0) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        onDamaged?.Invoke();
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth == 0)
        {
            onDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || currentHealth <= 0) return;
        int before = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        if (currentHealth != before) onHealed?.Invoke();
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetMax(int newMax, bool fill = true)
    {
        maxHealth = Mathf.Max(1, newMax);
        if (fill) currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
