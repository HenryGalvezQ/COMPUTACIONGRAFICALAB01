// BasketballController.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BasketballController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public Transform cameraTransform;

    [Header("Ball & Shooting")]
    public Rigidbody BallRb;
    public Transform PosDribble;
    public Transform PosOverHead;
    public Transform Arms;
    public Transform Target;
    public float verticalMultiplier = 2f;   // Para aumentar la altura proporcionalmente

    [Header("Pickup Settings")]
    public KeyCode pickupKey = KeyCode.E;    // Tecla para recoger la pelota

    private CharacterController controller;
    private ForceBar forceBar;
    private bool isBallInHands = true;
    private bool canPickup = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        forceBar = FindObjectOfType<ForceBar>();

        if (BallRb != null)
        {
            BallRb.isKinematic = true;
            AttachBallToHand();
        }
    }

    void Update()
    {
        HandleMovement();
        HandleDribbleAndShoot();
    }

    void LateUpdate()
    {
        // Permite recoger la pelota si está en rango y no la tienes en la mano
        if (canPickup && !isBallInHands && Input.GetKeyDown(pickupKey))
        {
            ResetShot();
        }
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 dir = cameraTransform.right * x + cameraTransform.forward * z;
        dir.y = 0;
        controller.Move(dir.normalized * moveSpeed * Time.deltaTime);
        if (!controller.isGrounded)
            controller.Move(Physics.gravity * Time.deltaTime);
    }

    private void HandleDribbleAndShoot()
    {
        if (!isBallInHands) return;

        // Disparo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
            return;
        }

        // Visual dribbling / hold
        if (Input.GetKey(KeyCode.Space))
        {
            BallRb.transform.position = PosOverHead.position;
            Arms.localEulerAngles = Vector3.right * 180;
            transform.LookAt(Target.parent.position);
        }
        else
        {
            BallRb.transform.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 5));
            Arms.localEulerAngles = Vector3.zero;
        }
    }

    private void Shoot()
    {
        isBallInHands = false;
        BallRb.transform.SetParent(null);
        BallRb.isKinematic = false;

        float f = forceBar.GetForce();
        Vector3 dir = (Target.position - PosOverHead.position).normalized;
        Vector3 dirH = new Vector3(dir.x, 0, dir.z).normalized;

        Vector3 forceH = dirH * f;
        Vector3 forceV = Vector3.up * f * verticalMultiplier;
        Vector3 totalForce = forceH + forceV;

        Debug.Log($"[BasketballController] Shoot → H={forceH.magnitude:F1}, V={forceV.magnitude:F1}");
        BallRb.AddForce(totalForce, ForceMode.Impulse);
    }

    private void AttachBallToHand()
    {
        BallRb.transform.SetParent(Arms);
        BallRb.transform.position = PosOverHead.position;
        Arms.localEulerAngles = Vector3.right * 180;
    }

    /// <summary>
    /// Restablece la pelota en la mano del jugador activo.
    /// Llamado desde TurnManager y al recoger la pelota.
    /// </summary>
    public void ResetShot()
    {
        isBallInHands = true;
        BallRb.velocity = Vector3.zero;
        BallRb.angularVelocity = Vector3.zero;
        BallRb.isKinematic = true;
        AttachBallToHand();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detecta si la pelota entra en el área de recogida
        if (other.attachedRigidbody == BallRb && !isBallInHands)
            canPickup = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == BallRb)
            canPickup = false;
    }
}
