using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float _impulseStrength = 5.0f;

    private Rigidbody2D _rb = null;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector3 impulse = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W))
            impulse += Vector3.up;

        if (Input.GetKeyDown(KeyCode.S))
            impulse += Vector3.down;

        if (Input.GetKeyDown(KeyCode.A))
            impulse += Vector3.left;

        if (Input.GetKeyDown(KeyCode.D))
            impulse += Vector3.right;

        if (impulse != Vector3.zero)
        {
            _rb.AddForce(impulse.normalized * _impulseStrength, ForceMode2D.Impulse);
        }
    }






}
