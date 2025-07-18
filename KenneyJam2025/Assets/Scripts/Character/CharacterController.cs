using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float _impulseStrength = 5.0f;

    private Rigidbody2D _rb = null;

    private CharacterInputDirection _currentDirectionInputed = CharacterInputDirection.None;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GetCharacterInput();
        ApplyImpulseToDirecion();


        //Vector3 impulse = Vector3.zero;
        //
        //if (Input.GetKeyDown(KeyCode.W))
        //    impulse += Vector3.up;
        //
        //if (Input.GetKeyDown(KeyCode.S))
        //    impulse += Vector3.down;
        //
        //if (Input.GetKeyDown(KeyCode.A))
        //    impulse += Vector3.left;
        //
        //if (Input.GetKeyDown(KeyCode.D))
        //    impulse += Vector3.right;
        //
        //if (Input.GetKeyDown(KeyCode.Space))
        //    return;
        //
        //if (impulse != Vector3.zero)
        //{
        //    _rb.AddForce(impulse.normalized * _impulseStrength, ForceMode2D.Impulse);
        //}
    }

    private void GetCharacterInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            _currentDirectionInputed = CharacterInputDirection.Up;

        if (Input.GetKeyDown(KeyCode.S))
            _currentDirectionInputed = CharacterInputDirection.Down;

        if (Input.GetKeyDown(KeyCode.A))
            _currentDirectionInputed = CharacterInputDirection.Left;

        if (Input.GetKeyDown(KeyCode.D))
            _currentDirectionInputed = CharacterInputDirection.Right;

        Debug.Log("The current impulse direction wil be " + _currentDirectionInputed.ToString());
    }

    private void ApplyImpulseToDirecion()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 impulse = Vector3.zero;

            switch (_currentDirectionInputed)
            {
                case CharacterInputDirection.Up:
                    impulse = Vector3.up;
                    break;
                case CharacterInputDirection.Down:
                    impulse = Vector3.down;
                    break;
                case CharacterInputDirection.Left:
                    impulse = Vector3.left;
                    break;
                case CharacterInputDirection.Right:
                    impulse = Vector3.right;
                    break;
                default:
                    return; // No valid direction input
            }
            _rb.AddForce(impulse.normalized * _impulseStrength, ForceMode2D.Impulse);
        }         
    }

    private enum CharacterInputDirection
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Default
    }
}
