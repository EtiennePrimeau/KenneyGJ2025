using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{    
    [SerializeField] private Transform _rightShoulder = null;
    [SerializeField] private Transform _leftShoulder = null;
    [SerializeField] private Transform _characterMiddle = null;
    [SerializeField] private SpriteRenderer _rightArmSpriteRenderer = null;
    [SerializeField] private SpriteRenderer _leftArmSpriteRenderer = null;
    [SerializeField] private AudioClip _impulseSoundFX = null;

    [Header("SETTINGS")]
    [SerializeField] private float _impulseStrength = 5.0f;
    [SerializeField] private float _maxVelocity = 5.0f;
    [SerializeField] private float _shoulderRotationSpeed = 5.0f;
    [SerializeField] private float _idlingDelay = 5.0f;
    
    private static readonly Vector3 DefaultRightShouldRotation = new Vector3(0.0f, 0.0f, 0.0f);
    private static readonly Vector3 DefaultLeftShouldRotation = new Vector3(0.0f, 0.0f, 180.0f);
    private static readonly Vector3 UpShoulderRotation = new Vector3(0.0f, 0.0f, 90.0f);
    private static readonly Vector3 DownShoulderRotation = new Vector3(0.0f, 0.0f, -90.0f);
    private static readonly Vector3 RightShoulderRotation = new Vector3(0.0f, 0.0f, 0.0f);
    private static readonly Vector3 LeftShoulderRotation = new Vector3(0.0f, 0.0f, 180.0f);    
    private const int FirstPlanOrderInLayer = 2;
    private const int SecondPlanOrderInLayer = 1;

    private Rigidbody2D _rb = null;
    private AudioSource _soundFXAudioSource = null;
    private CharacterInputDirection _currentDirectionInputed = CharacterInputDirection.None;
    private Quaternion _targetShoulderRotation;
    private Quaternion _currentShoulderRotation;
    private Coroutine _idlingTimerCoroutine = null;
    private float _beforeIdlingTimer = 0.0f;
    private bool _areArmsSynchronized = false;
    private bool _isIdling = false;
    private bool _applyImpulseNextFixedUpdate = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _soundFXAudioSource = GetComponent<AudioSource>();

        DefaultRotateShoulders();
        _areArmsSynchronized = false;
    }

    private void Update()
    {
        GetCurrentFrameCharacterInput();
    }

    private void FixedUpdate()
    {
        if (_applyImpulseNextFixedUpdate)
        {
            ApplyImpulseToDirecion();
            _applyImpulseNextFixedUpdate = false;
        }
        UpdateShoulderRotation();

        CapVelocity();
    }

    private void GetCurrentFrameCharacterInput()
    {
        if (Input.GetKey(KeyCode.W))
            SetDirection(CharacterInputDirection.Up);
        else if (Input.GetKey(KeyCode.S))
            SetDirection(CharacterInputDirection.Down);
        else if (Input.GetKey(KeyCode.A))
            SetDirection(CharacterInputDirection.Left);
        else if (Input.GetKey(KeyCode.D))
            SetDirection(CharacterInputDirection.Right);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _applyImpulseNextFixedUpdate = true;
        }

        //Debug.Log("The current impulse direction will be " + _currentDirectionInputed.ToString());
    }

    private void SetDirection(CharacterInputDirection direction)
    {
        _currentDirectionInputed = direction;

        switch (_currentDirectionInputed)
        {
            case CharacterInputDirection.Up:
                _targetShoulderRotation = Quaternion.Euler(UpShoulderRotation);
                break;
            case CharacterInputDirection.Down:
                _targetShoulderRotation = Quaternion.Euler(DownShoulderRotation);
                break;
            case CharacterInputDirection.Left:
                _targetShoulderRotation = Quaternion.Euler(LeftShoulderRotation);
                break;
            case CharacterInputDirection.Right:
                _targetShoulderRotation = Quaternion.Euler(RightShoulderRotation);
                break;
            default:
                break;
        }

        _beforeIdlingTimer = 0.0f;
        _isIdling = false;
    }    

    private void ApplyImpulseToDirecion()
    {
        Vector2 impulseDirection = Vector2.zero;

        _beforeIdlingTimer = 0.0f;

        PlayImpulseSoundFX();

        if (!_areArmsSynchronized)
        {
            impulseDirection = Vector3.up;
            _rb.AddForce(impulseDirection.normalized * 1, ForceMode2D.Impulse);
            return;
        }

        impulseDirection = _currentShoulderRotation * Vector3.right;
        _rb.AddForce(impulseDirection.normalized * _impulseStrength, ForceMode2D.Impulse);        
    }

    private void PlayImpulseSoundFX()
    {        
        _soundFXAudioSource.pitch = Random.Range(0.90f, 1.1f);
        float volume = Random.Range(0.8f, 1.0f);
        _soundFXAudioSource.PlayOneShot(_impulseSoundFX, volume);
    }

    // TODO optimization à faire ici pour pas faire de update quand on est déjà arrivé à la rotation cible
    private void UpdateShoulderRotation()
    {
        if (_currentDirectionInputed == CharacterInputDirection.None)
            return;

        if (_isIdling)
        {
            UpdateEachShoulderRotationToIdlingRotation();
            return;
        }

        float targetZ = _targetShoulderRotation.eulerAngles.z;

        if (_areArmsSynchronized)
        {
            float currentZ = _currentShoulderRotation.eulerAngles.z;
            float newZ = RotateZUpward(currentZ, targetZ, _shoulderRotationSpeed * Time.deltaTime);
            _currentShoulderRotation = Quaternion.Euler(0.0f, 0.0f, newZ);

            _rightShoulder.localRotation = _currentShoulderRotation;
            _leftShoulder.localRotation = _currentShoulderRotation;

            //Debug.LogError("Still here");

            UpdateArmsOrderInLayer();

            if (_idlingTimerCoroutine == null && ApproximatelyAngle(_currentShoulderRotation.eulerAngles.z, targetZ))
            {
                _idlingTimerCoroutine = StartCoroutine(StartIdleAfterDirectionReached());
            }

            return;
        }

        float rightZ = _rightShoulder.localEulerAngles.z;
        float leftZ = _leftShoulder.localEulerAngles.z;

        float newRightZ = RotateZUpward(rightZ, targetZ, _shoulderRotationSpeed * Time.deltaTime);
        float newLeftZ = RotateZUpward(leftZ, targetZ, _shoulderRotationSpeed * Time.deltaTime);

        _rightShoulder.localRotation = Quaternion.Euler(0.0f, 0.0f, newRightZ);
        _leftShoulder.localRotation = Quaternion.Euler(0.0f, 0.0f, newLeftZ);

        UpdateArmsOrderInLayer();

        if (ApproximatelyAngle(newRightZ, targetZ) && ApproximatelyAngle(newLeftZ, targetZ))
        {
            _areArmsSynchronized = true;
            _currentShoulderRotation = Quaternion.Euler(0.0f, 0.0f, targetZ);
        }
    }

    private void UpdateEachShoulderRotationToIdlingRotation()
    {
        float rightZ = _rightShoulder.localEulerAngles.z;
        float leftZ = _leftShoulder.localEulerAngles.z;

        float targetRightZ = DefaultRightShouldRotation.z;
        float targetLeftZ = DefaultLeftShouldRotation.z;

        float newRightZ = RotateZUpward(rightZ, targetRightZ, _shoulderRotationSpeed * Time.deltaTime);
        float newLeftZ = RotateZUpward(leftZ, targetLeftZ, _shoulderRotationSpeed * Time.deltaTime);

        _rightShoulder.localRotation = Quaternion.Euler(0.0f, 0.0f, newRightZ);
        _leftShoulder.localRotation = Quaternion.Euler(0.0f, 0.0f, newLeftZ);

        UpdateArmsOrderInLayer();
    }

    private float RotateZUpward(float currentZ, float targetZ, float step)
    {
        currentZ = NormalizeAngle(currentZ);
        targetZ = NormalizeAngle(targetZ);

        if (Mathf.Approximately(currentZ, targetZ))
            return targetZ;

        bool isLeftOrRight = (Mathf.Approximately(currentZ, 0.0f) && Mathf.Approximately(targetZ, 180.0f)) || (Mathf.Approximately(currentZ, 180.0f) && Mathf.Approximately(targetZ, 0.0f));

        if (isLeftOrRight)
        {
            if (Mathf.Approximately(currentZ, 180.0f) && Mathf.Approximately(targetZ, 0.0f))
            {
                currentZ -= step;
                if (currentZ < 0.0f) currentZ += 360.0f;
                if (currentZ < 1.0f) currentZ = 0.0f;
            }
            else if (Mathf.Approximately(currentZ, 0.0f) && Mathf.Approximately(targetZ, 180.0f))
            {
                currentZ += step;
                if (currentZ > 360.0f) currentZ -= 360.0f;
                if (currentZ > 179.0f) currentZ = 180.0f;
            }
        }
        else
        {
            float delta = (targetZ - currentZ + 360.0f) % 360.0f;
            if (delta <= 180.0f)
            {
                currentZ += Mathf.Min(step, delta);
            }
            else
            {
                currentZ -= Mathf.Min(step, 360.0f - delta);
            }
        }

        return NormalizeAngle(currentZ);
    }    

    private void UpdateArmsOrderInLayer()
    {
        if (_areArmsSynchronized)
        {
            float currentShoulderZAngle = _currentShoulderRotation.eulerAngles.z;

            if (currentShoulderZAngle > 90.0f && currentShoulderZAngle < 270.0f)
            {
                _rightArmSpriteRenderer.sortingOrder = FirstPlanOrderInLayer;
                _leftArmSpriteRenderer.sortingOrder = SecondPlanOrderInLayer;
            }
            else
            {
                _rightArmSpriteRenderer.sortingOrder = SecondPlanOrderInLayer;
                _leftArmSpriteRenderer.sortingOrder = FirstPlanOrderInLayer;
            }
            return;
        }

        float currentRightShoulderZAngle = _rightShoulder.localRotation.eulerAngles.z;
        float currentLeftShoulderZAngle = _leftShoulder.localRotation.eulerAngles.z;

        //Debug.Log($"Right Shoulder Z Angle: {currentRightShoulderZAngle}, Left Shoulder Z Angle: {currentLeftShoulderZAngle}");

        if (currentLeftShoulderZAngle == 180.0f)
        {
            _leftArmSpriteRenderer.sortingOrder = SecondPlanOrderInLayer;
            _rightArmSpriteRenderer.sortingOrder = FirstPlanOrderInLayer;
        }
        if (currentRightShoulderZAngle == 0.0f)
        {
            _rightArmSpriteRenderer.sortingOrder = SecondPlanOrderInLayer;
            _leftArmSpriteRenderer.sortingOrder = FirstPlanOrderInLayer;
        }
    }

    private void DefaultRotateShoulders()
    {
        _rightShoulder.localRotation = Quaternion.Euler(DefaultRightShouldRotation);
        _leftShoulder.localRotation = Quaternion.Euler(DefaultLeftShouldRotation);
    }

    private IEnumerator StartIdleAfterDirectionReached()
    {
        _beforeIdlingTimer = 0.0f;

        while (_beforeIdlingTimer < _idlingDelay)
        {
            if (_currentDirectionInputed == CharacterInputDirection.None || _isIdling)
            {
                _idlingTimerCoroutine = null;
                yield break;
            }

            _beforeIdlingTimer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Now officially idling.");

        _areArmsSynchronized = false;
        _isIdling = true;
        _idlingTimerCoroutine = null;
    }

    private void CapVelocity()
    {
        Vector2 velocity = _rb.linearVelocity;

        if (velocity == Vector2.zero)
            return;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

        // Bigger downward buffer
        //if (angle >= -165.0f && angle <= -15.0f)
        //{
        //    Debug.Log($"Skipping cap — moving broadly downward (angle: {angle}°)");
        //    return;
        //}

        // Tighter downward buffer
        if (angle >= -100.0f && angle <= -80.0f)
        {
            //Debug.Log($"Skipping cap — moving almost directly downward (angle: {angle}°)");
            return;
        }

        if (velocity.magnitude > _maxVelocity)
        {
            //Debug.Log($"Capping velocity from {velocity.magnitude} to {_maxVelocity} (angle: {angle}°)");
            _rb.linearVelocity = velocity.normalized * _maxVelocity;
        }
    }

    private bool ApproximatelyAngle(float a, float b, float tolerance = 1f)
    {
        float diff = Mathf.Abs(NormalizeAngle(a) - NormalizeAngle(b));
        return diff <= tolerance || Mathf.Abs(diff - 360.0f) <= tolerance;
    }

    private float NormalizeAngle(float angle)
    {
        return (angle + 360.0f) % 360.0f;
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
