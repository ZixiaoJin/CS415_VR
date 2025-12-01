using UnityEngine;
using UnityEngine.InputSystem;

/// Require a CharacterController so movement collides with MeshColliders.
[RequireComponent(typeof(CharacterController))]
public class TunnelMover : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference rightMove;     // Vector2 (x: left/right, y: up/down)
    [SerializeField] private InputActionReference rightTrigger;  // fire (optional hook)

    [Header("Strafe Speeds (m/s)")]
    [SerializeField] private float strafeSpeed = 30f;
    [SerializeField] private float verticalSpeed = 30f;

    [Header("Forward Speed Ramp")]
    [Tooltip("Starting forward speed (m/s).")]
    [SerializeField] private float forwardMin = 8f;
    [Tooltip("Maximum forward speed (m/s).")]
    [SerializeField] private float forwardMax = 40f;
    [Tooltip("Seconds to go from Min to Max using the curve below.")]
    [SerializeField] private float rampSeconds = 120f;
    [Tooltip("0?1 time to 0?1 speed fraction; tweak for ease-in/out ramps.")]
    [SerializeField] private AnimationCurve rampCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Orientation")]
    [SerializeField] private Transform forwardBasis;   // tunnel direction
    [SerializeField] private Transform strafeBasis;    // head/camera for yaw for strafing
    [SerializeField] private bool projectForwardOnWorldUp = true;

    private CharacterController controller;
    private float elapsed;           // time since (re)start for ramp
    private float currentForward;    // current forward speed

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        ResetMovement(); // initialize currentForward & timers
    }

    void OnEnable()
    {
        if (rightMove) rightMove.action.Enable();
        if (rightTrigger) rightTrigger.action.Enable();
    }

    void OnDisable()
    {
        if (rightMove) rightMove.action.Disable();
        if (rightTrigger) rightTrigger.action.Disable();
    }

    void Update()
    {
        // ---- Compute forward direction ----
        Vector3 fwd = forwardBasis ? forwardBasis.forward : Vector3.forward;
        if (projectForwardOnWorldUp) { fwd.y = 0f; fwd.Normalize(); }

        // ---- Build strafe frame from camera yaw only ----
        Vector3 right = Vector3.right;
        Vector3 up = Vector3.up;
        if (strafeBasis)
        {
            var yawOnly = Quaternion.Euler(0f, strafeBasis.eulerAngles.y, 0f);
            right = (yawOnly * Vector3.right).normalized;
            up = Vector3.up;
        }

        // ---- Forward speed ramp (time-based) ----
        elapsed += Time.deltaTime;
        float t = (rampSeconds > 0f) ? Mathf.Clamp01(elapsed / rampSeconds) : 1f;
        float frac = rampCurve.Evaluate(t);
        currentForward = Mathf.Lerp(forwardMin, forwardMax, frac);

        // ---- Read input & move ----
        Vector2 move = rightMove ? rightMove.action.ReadValue<Vector2>() : Vector2.zero;

        Vector3 velocity =
            fwd * currentForward +
            right * (move.x * strafeSpeed) +
            up * (move.y * verticalSpeed);

        controller.Move(velocity * Time.deltaTime);

        // ---- Fire hook (optional) ----
        if (rightTrigger != null && rightTrigger.action.WasPressedThisFrame())
        {
            // TODO: call your projectile spawner here
        }
    }

    /// Call this from your Restart Screen when the player restarts.
    public void ResetMovement()
    {
        elapsed = 0f;
        currentForward = forwardMin;
    }

    /// If you ever need an instant difficulty jump (e.g., after N walls passed).
    public void BumpDifficulty(float newForwardMax, float newRampSeconds = -1f)
    {
        forwardMax = Mathf.Max(newForwardMax, forwardMin + 0.1f);
        if (newRampSeconds > 0f) rampSeconds = newRampSeconds;
        // keep elapsed as-is so the curve continues smoothly
    }
}
