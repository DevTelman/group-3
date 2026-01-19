using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Цель")]
    public Transform target;

    [Header("Настройки позиции")]
    public Vector3 offset = new Vector3(0, 5, -7);
    public float positionSmoothTime = 0.3f;

    [Header("Защита от стен")]
    public LayerMask collisionLayers;
    public float minDistance = 2.5f;
    public float cameraPadding = 0.3f;

    [Header("Настройки поворота")]
    public bool rotateWithTarget = true;
    public float rotationSmoothTime = 0.2f;
    public float verticalRotationLimit = 80f;

    [Header("Стабилизация")]
    public bool dampenVerticalMovement = true;
    public float verticalDampening = 0.5f;

    private Vector3 positionVelocity = Vector3.zero;
    private Vector3 rotationVelocity = Vector3.zero;
    private Vector3 currentRotation;
    private Vector3 targetRotationEuler;

    void Start()
    {
        if (target != null)
        {
            currentRotation = transform.eulerAngles;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredOffset = rotateWithTarget ? target.rotation * offset : offset;
        Vector3 targetPosition = target.position + desiredOffset;

        Vector3 safePosition = CheckCameraCollisions(target.position, targetPosition);

        if (dampenVerticalMovement)
        {
            Vector3 horizontalTarget = new Vector3(safePosition.x, transform.position.y, safePosition.z);
            Vector3 smoothHorizontal = Vector3.SmoothDamp(
                new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(horizontalTarget.x, 0, horizontalTarget.z),
                ref positionVelocity,
                positionSmoothTime
            );

            float smoothY = Mathf.SmoothDamp(
                transform.position.y,
                safePosition.y,
                ref positionVelocity.y,
                positionSmoothTime * (1f + verticalDampening)
            );

            transform.position = new Vector3(smoothHorizontal.x, smoothY, smoothHorizontal.z);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                safePosition,
                ref positionVelocity,
                positionSmoothTime
            );
        }

        SmoothLookAt(target.position + Vector3.up * 1.5f);
    }

    void SmoothLookAt(Vector3 lookPoint)
    {
        Vector3 direction = lookPoint - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotationEuler = targetRotation.eulerAngles;

        if (targetRotationEuler.x > 180f)
            targetRotationEuler.x -= 360f;

        targetRotationEuler.x = Mathf.Clamp(targetRotationEuler.x, -verticalRotationLimit, verticalRotationLimit);

        currentRotation.x = Mathf.SmoothDampAngle(currentRotation.x, targetRotationEuler.x, ref rotationVelocity.x, rotationSmoothTime);
        currentRotation.y = Mathf.SmoothDampAngle(currentRotation.y, targetRotationEuler.y, ref rotationVelocity.y, rotationSmoothTime);
        currentRotation.z = Mathf.SmoothDampAngle(currentRotation.z, 0f, ref rotationVelocity.z, rotationSmoothTime);

        transform.eulerAngles = currentRotation;
    }

    Vector3 CheckCameraCollisions(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 direction = toPosition - fromPosition;
        float distance = direction.magnitude;

        if (distance < minDistance)
        {
            return fromPosition + direction.normalized * minDistance;
        }

        RaycastHit hit;
        float shortestDistance = distance;

        if (Physics.Raycast(fromPosition, direction.normalized, out hit, distance, collisionLayers))
        {
            shortestDistance = Mathf.Min(shortestDistance, hit.distance - cameraPadding);
        }

        Vector3[] offsets = new Vector3[]
        {
            Vector3.up * 0.3f,
            Vector3.down * 0.3f,
            Vector3.right * 0.3f,
            Vector3.left * 0.3f
        };

        foreach (Vector3 offset in offsets)
        {
            Vector3 checkPoint = toPosition + offset;
            Vector3 dir = checkPoint - fromPosition;

            if (Physics.Raycast(fromPosition, dir.normalized, out hit, dir.magnitude, collisionLayers))
            {
                shortestDistance = Mathf.Min(shortestDistance, hit.distance - cameraPadding);
            }
        }

        shortestDistance = Mathf.Max(shortestDistance, minDistance);

        return fromPosition + direction.normalized * shortestDistance;
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Vector3 desiredOffset = rotateWithTarget ? target.rotation * offset : offset;
        Vector3 desiredPosition = target.position + desiredOffset;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(target.position, desiredPosition);
        Gizmos.DrawWireSphere(desiredPosition, 0.3f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, minDistance);
    }
}