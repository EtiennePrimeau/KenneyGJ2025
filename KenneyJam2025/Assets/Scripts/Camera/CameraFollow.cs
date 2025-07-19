using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _character = null;
    [SerializeField] private Vector3 _offset = new Vector3(0.0f, 0.0f, -10.0f);
    [SerializeField] private float _smoothSpeed = 5.0f;
    [SerializeField] private float _maxDistance = 3.0f;

    private void FixedUpdate()
    {
        Vector3 desiredPosition = _character.position + _offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);

        Vector3 delta = smoothedPosition - _character.position - _offset;
        if (delta.magnitude > _maxDistance)
        {
            delta = delta.normalized * _maxDistance;
            smoothedPosition = _character.position + _offset + delta;
        }

        transform.position = smoothedPosition;
    }
}
