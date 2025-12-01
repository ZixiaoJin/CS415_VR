using UnityEngine;

public class Spinner : MonoBehaviour
{
    [Tooltip("Local axis to spin around (Z = forward).")]
    public Vector3 localAxis = Vector3.forward;
    [Tooltip("Degrees per second.")]
    public float degreesPerSecond = -20f;

    void Update()
    {
        transform.Rotate(localAxis, degreesPerSecond * Time.deltaTime, Space.Self);
    }
}
