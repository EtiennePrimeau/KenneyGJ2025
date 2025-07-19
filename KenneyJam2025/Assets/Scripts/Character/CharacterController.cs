using UnityEngine;

public class CharacterController : MonoBehaviour
{    
    [SerializeField] private Transform _rightShoulder = null;
    [SerializeField] private Transform _leftShoulder = null;
    [SerializeField] private Transform _characterMiddle = null;
    [SerializeField] private float _impulseStrength = 5.0f;
    [SerializeField] private float _shoulderRotationSpeed = 5.0f;

    private static readonly Vector3 DefaultRightShouldRotation = new Vector3(0.0f, 0.0f, 0.0f);
    private static readonly Vector3 DefaultLeftShouldRotation = new Vector3(0.0f, 0.0f, 180.0f);
    private static readonly Vector3 UpShoulderRotation = new Vector3(0.0f, 0.0f, 90.0f);
    private static readonly Vector3 DownShoulderRotation = new Vector3(0.0f, 0.0f, -90.0f);
    private static readonly Vector3 RightShoulderRotation = new Vector3(0.0f, 0.0f, 0.0f);
    private static readonly Vector3 LeftShoulderRotation = new Vector3(0.0f, 0.0f, 180.0f);    

    private Rigidbody2D _rb = null;
    private CharacterInputDirection _currentDirectionInputed = CharacterInputDirection.None;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        _rightShoulder.localRotation = Quaternion.Euler(DefaultRightShouldRotation);
        _leftShoulder.localRotation = Quaternion.Euler(DefaultLeftShouldRotation);
    }

    void Update()
    {
        GetCurrentFrameCharacterInput();
        ApplyImpulseToDirecion();
        UpdateShoulderRotation();
    }

    private void GetCurrentFrameCharacterInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            _currentDirectionInputed = CharacterInputDirection.Up;

        if (Input.GetKeyDown(KeyCode.S))
            _currentDirectionInputed = CharacterInputDirection.Down;

        if (Input.GetKeyDown(KeyCode.A))
            _currentDirectionInputed = CharacterInputDirection.Left;

        if (Input.GetKeyDown(KeyCode.D))
            _currentDirectionInputed = CharacterInputDirection.Right;

        Debug.Log("The current impulse direction will be " + _currentDirectionInputed.ToString());
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

    private void UpdateShoulderRotation()
    {
        switch (_currentDirectionInputed)
        {
            case CharacterInputDirection.Up:
                _characterMiddle.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                break;
            case CharacterInputDirection.Down:
                _characterMiddle.localRotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
                break;
            case CharacterInputDirection.Left:
                _characterMiddle.localRotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
                break;
            case CharacterInputDirection.Right:
                _characterMiddle.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
            default:
                return;
        }

        _rightShoulder.localRotation = _characterMiddle.localRotation;
        _leftShoulder.localRotation = _characterMiddle.localRotation;
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

    // Default (or none?) could be arms on both side so both shoulder.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
}
