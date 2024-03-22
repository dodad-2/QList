namespace QList.UI;

using MelonLoader;
using UnityEngine;

[RegisterTypeInIl2Cpp]
public class CameraPosition : MonoBehaviour
{
    public Vector3 originalPosition,
        targetPosition;
    public Quaternion originalRotation,
        targetRotation;
    public Il2Cpp.FollowCursorCameraController? followController;

    public bool reposition;

    public float moveSpeed = 3f;
    public float rotationSpeed = 2f;

    private void Awake()
    {
        originalPosition = gameObject.transform.position;
        originalRotation = gameObject.transform.rotation;
        targetPosition = new Vector3(-70.5f, 1.9f, 28.1f); // -70.4658f, 1.8027f, 28.0986f

        Quaternion forward = Quaternion.identity;

        forward = Quaternion.AngleAxis(-5f, Vector3.up);
        forward *= Quaternion.AngleAxis(-20f, Vector3.right);

        targetRotation = forward;

        followController = GetComponent<Il2Cpp.FollowCursorCameraController>();
    }

    private void Update()
    {
        if (reposition)
        {
            if (followController.enabled)
                followController.enabled = false;

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * moveSpeed
            );
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
        else
        {
            if (!followController.enabled)
            {
                transform.rotation = originalRotation;
                followController.enabled = true;
            }

            transform.position = Vector3.Lerp(
                transform.position,
                originalPosition,
                Time.deltaTime * moveSpeed
            );
        }
    }
}
