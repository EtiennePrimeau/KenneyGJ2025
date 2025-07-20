using System.Collections;
using UnityEngine;

public class BodyController : MonoBehaviour
{
    [SerializeField] private Sprite _normal = null;
    [SerializeField] private Sprite _outch = null;
    [SerializeField] private AudioClip _collisionSoundFX = null;
    [SerializeField] private float _swapDuration = 1.0f;    

    private AudioSource _soundFXAudioSource = null;
    private SpriteRenderer _spriteRenderer = null;
    private Coroutine _faceSwapCoroutine = null;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _soundFXAudioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _soundFXAudioSource.pitch = Random.Range(0.90f, 1.1f);
        float volume = Random.Range(0.8f, 1.0f);

        _soundFXAudioSource.PlayOneShot(_collisionSoundFX, volume);

        if (_faceSwapCoroutine == null)
            _faceSwapCoroutine = StartCoroutine(SwapFaceCoroutine());
    }

    private IEnumerator SwapFaceCoroutine()
    {
        _spriteRenderer.sprite = _outch;

        yield return new WaitForSeconds(_swapDuration);

        _spriteRenderer.sprite = _normal;
        _faceSwapCoroutine = null;
    }
}
