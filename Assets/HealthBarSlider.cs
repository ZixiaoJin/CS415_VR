using UnityEngine;
using UnityEngine.UI;

public class HealthBarSlider : MonoBehaviour
{
    [Header("Refs")]
    public PlayerHealth playerHealth;
    public Slider slider;

    void Awake()
    {
        // Fallbacks
        if (!slider) slider = GetComponent<Slider>();

        // Init from current values
        if (playerHealth)
        {
            slider.minValue = 0;
            slider.maxValue = playerHealth.maxHealth;
            slider.value = playerHealth.currentHealth;

            // Listen for changes
            playerHealth.onHealthChanged.AddListener(OnHealthChanged);
        }
    }

    void OnDestroy()
    {
        if (playerHealth)
            playerHealth.onHealthChanged.RemoveListener(OnHealthChanged);
    }

    void OnHealthChanged(int current, int max)
    {
        if (slider.maxValue != max) slider.maxValue = max;
        slider.value = current;
    }
}
