using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class ForceBar : MonoBehaviour
{
    [Header("Oscillation Settings")]
    public float speed = 1f;           // Velocidad de oscilación
    public float maxForce = 10f;       // Fuerza máxima

    [Header("Optional UI Debug")]
    public TMP_Text forceValueText;    // Asigna un TextMeshPro para ver el valor

    private Slider slider;
    private TurnManager turnManager;

    void Start()
    {
        slider = GetComponent<Slider>();
        turnManager = FindObjectOfType<TurnManager>();
        if (slider == null) Debug.LogError("ForceBar: no hay Slider attached.");
        if (turnManager == null) Debug.LogError("ForceBar: no hay TurnManager en la escena.");
    }

    void Update()
    {
        // Solo oscila durante el turno activo
        if (turnManager == null || (turnManager.currentTurn != Turn.Player1 && turnManager.currentTurn != Turn.Player2))
        {
            slider.value = 0;
        }
        else
        {
            slider.value = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        }

        // Debug: muestra el valor de fuerza
        float force = GetForce();
        Debug.Log($"ForceBar - slider: {slider.value:F2}, force: {force:F2}");

        // Si hay texto asignado, actualízalo
        if (forceValueText != null)
        {
            forceValueText.text = force.ToString("F2");
        }
    }

    /// <summary>
    /// Devuelve la fuerza que debe aplicarse al disparo
    /// </summary>
    public float GetForce()
    {
        return slider.value * maxForce;
    }
}
