// TurnManager.cs
using UnityEngine;
using TMPro;

public enum Turn { Player1, Player2 }

public class TurnManager : MonoBehaviour
{
    [Header("Configuración de Turnos")]
    public Turn currentTurn = Turn.Player1;
    public float timePerTurn = 60f;         // Tiempo inicial por jugador

    [Header("Referencias UI")]
    public TMP_Text timerTextP1;
    public TMP_Text timerTextP2;
    public TMP_Text scoreText;

    [Header("Jugadores y UI")]
    public GameObject player1;
    public GameObject player2;
    public GameObject forceSliderUI;

    [Header("Punto único de Spawn")]
    public Transform spawnPoint;

    private float timeLeftP1;
    private float timeLeftP2;
    private int scoreP1 = 0;
    private int scoreP2 = 0;

    void Start()
    {
        // Inicializa relojes
        timeLeftP1 = timePerTurn;
        timeLeftP2 = timePerTurn;

        ApplyTurnStart();
        Debug.Log($"[TurnManager] Empieza turno de {currentTurn}");
    }

    void Update()
    {
        // Descuenta solo el reloj activo
        if (currentTurn == Turn.Player1) timeLeftP1 -= Time.deltaTime;
        else                              timeLeftP2 -= Time.deltaTime;

        // Si se acaba el tiempo, cambia de turno
        if ((currentTurn == Turn.Player1 && timeLeftP1 <= 0f) ||
            (currentTurn == Turn.Player2 && timeLeftP2 <= 0f))
        {
            Debug.Log($"[TurnManager] Tiempo expirado de {currentTurn}");
            EndTurn();
        }

        // Actualiza displays
        timerTextP1.text = Mathf.CeilToInt(timeLeftP1).ToString();
        timerTextP2.text = Mathf.CeilToInt(timeLeftP2).ToString();
    }

    /// <summary>
    /// Suma puntos al jugador activo.
    /// </summary>
    public void AddScore(int points)
    {
        if (currentTurn == Turn.Player1)
        {
            scoreP1 += points;
            Debug.Log($"[TurnManager] Player1 anotó! scoreP1={scoreP1}");
        }
        else
        {
            scoreP2 += points;
            Debug.Log($"[TurnManager] Player2 anotó! scoreP2={scoreP2}");
        }
        scoreText.text = $"P1: {scoreP1} – P2: {scoreP2}";
    }

    /// <summary>
    /// Cambia al siguiente turno, sin reiniciar relojes.
    /// </summary>
    public void EndTurn()
    {
        // Alterna turno
        currentTurn = (currentTurn == Turn.Player1) ? Turn.Player2 : Turn.Player1;
        Debug.Log($"[TurnManager] Cambio a turno de {currentTurn}");

        ApplyTurnStart();
    }

    /// <summary>
    /// Prepara escena, jugador y pelota para el turno activo.
    /// </summary>
    private void ApplyTurnStart()
    {
        bool isP1 = currentTurn == Turn.Player1;

        // Activa/desactiva jugadores
        player1.SetActive(isP1);
        player2.SetActive(!isP1);
        forceSliderUI.SetActive(true);

        // Reposiciona en el spawn central
        GameObject activePlayer = isP1 ? player1 : player2;
        activePlayer.transform.position = spawnPoint.position;

        // Resetea la pelota en su mano
        var bc = activePlayer.GetComponent<BasketballController>();
        if (bc != null) bc.ResetShot();
    }
}
