using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public float _currentHealth;
    public int currentUnits;
    public TMP_Text numberOfUnits;
    public void SetCurrentHealhtStart(float health)
    {
        _currentHealth = health;
    }
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = _currentHealth;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        _currentHealth = health;
    }

    public void AddNumberOfUnits(int amount)
    {
        currentUnits += amount;
        numberOfUnits.text = currentUnits.ToString();
    }
    public void SetNumberOfUnits(int amount)
    {
        currentUnits = amount;
        numberOfUnits.text = currentUnits.ToString();
    }
}
