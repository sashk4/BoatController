using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 12f;
    public float maxSpeed = 20f;
    public float turnSpeed = 45f;
    public float drag = 0.98f;

    [Header("Buoyancy")]
    public float waterLevel = 0f;
    public float floatThreshold = 2f;
    public float waterDensity = 0.125f;

    [HideInInspector] public bool isBeingDriven = false;

    private Rigidbody rb;
    private float throttleInput;
    private float steerInput;
    private float forceFactor;
    private Vector3 floatForce;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void Update()
    {
        if (isBeingDriven)
            ReadInput();
        else
        {
            throttleInput = 0f;
            steerInput = 0f;
        }
    }

    void ReadInput()
    {
        throttleInput = 0f;
        steerInput = 0f;

        if (Keyboard.current == null) return;

        if (Keyboard.current.wKey.isPressed)
            throttleInput = 1f;
        else if (Keyboard.current.sKey.isPressed)
            throttleInput = -1f;

        if (Keyboard.current.dKey.isPressed)
            steerInput = 1f;
        else if (Keyboard.current.aKey.isPressed)
            steerInput = -1f;
    }

    void FixedUpdate()
    {
        ApplyBuoyancy();

        if (isBeingDriven)
            HandleMovement();
    }

    void ApplyBuoyancy()
    {
        forceFactor = 1.0f - ((transform.position.y - waterLevel) / floatThreshold);

        if (forceFactor > 0)
        {
            floatForce = -Physics.gravity * (forceFactor - rb.linearVelocity.y * waterDensity);
            rb.AddForceAtPosition(floatForce, transform.position, ForceMode.Acceleration);
        }
    }

    void HandleMovement()
    {
        Vector3 forwardForce = transform.forward * throttleInput * acceleration;
        rb.AddForce(forwardForce, ForceMode.Acceleration);

        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
        float turn = steerInput * turnSpeed * Mathf.Clamp01(speedFactor + 0.2f);
        Quaternion turnRotation = Quaternion.Euler(0, turn * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * turnRotation);

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        rb.linearVelocity *= drag;
    }
}