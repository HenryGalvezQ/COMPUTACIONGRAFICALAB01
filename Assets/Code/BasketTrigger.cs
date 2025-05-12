using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BasketTrigger : MonoBehaviour
{
    [Header("Referencia al TurnManager")]
    public TurnManager turnManager;

    [Header("Altura del aro (Y) para validar canasta")]
    public float rimY = 3.05f;

    private bool shotResolved = false;

    void Awake()
    {
        if (turnManager == null)
            turnManager = FindObjectOfType<TurnManager>();

        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (shotResolved || !other.CompareTag("Ball"))
            return;

        shotResolved = true;

        Rigidbody rb = other.attachedRigidbody;
        bool scored = (rb != null && rb.velocity.y < 0f && other.transform.position.y > rimY);

        if (scored)
        {
            turnManager.AddScore(1);
            Debug.Log($"[BasketTrigger] {turnManager.currentTurn} encestó!");
        }
        else
        {
            Debug.Log($"[BasketTrigger] {turnManager.currentTurn} falló.");
        }

        // Tras breve retardo, cambia turno
        StartCoroutine(ResolveShotAndSwitch(1f));
    }

    private IEnumerator ResolveShotAndSwitch(float delay)
    {
        yield return new WaitForSeconds(delay);
        turnManager.EndTurn();
        shotResolved = false;
    }
}