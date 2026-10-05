using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerhealth;
    [SerializeField] private Image HealthBar_Total;
    [SerializeField] private Image HealthBar_Current;

    private void Start()
    {
        HealthBar_Total.fillAmount = playerhealth.currentHealth / 10;
    }

    private void Update()
    {
        HealthBar_Current.fillAmount = playerhealth.currentHealth / 10;
    }
}
