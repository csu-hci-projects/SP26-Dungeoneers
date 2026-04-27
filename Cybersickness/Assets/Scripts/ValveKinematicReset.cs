using UnityEngine;

public class ValveKinematicReset : MonoBehaviour
{
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private bool makeKinematicOnReset = true;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        if (targetRigidbody == null)
        {
            targetRigidbody = GetComponent<Rigidbody>();
        }

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    private void OnEnable()
    {
        ResetToInitialPose();
    }

    [ContextMenu("Reset To Initial Pose")]
    public void ResetToInitialPose()
    {
        transform.SetPositionAndRotation(initialPosition, initialRotation);

        if (targetRigidbody == null)
        {
            return;
        }

        targetRigidbody.velocity = Vector3.zero;
        targetRigidbody.angularVelocity = Vector3.zero;

        if (makeKinematicOnReset)
        {
            targetRigidbody.isKinematic = true;
        }
    }

    public void SetKinematic(bool isKinematic)
    {
        if (targetRigidbody != null)
        {
            targetRigidbody.isKinematic = isKinematic;
        }
    }
}
