using System.Collections;
using UnityEngine;

public class FaceSwapper : MonoBehaviour
{
    [SerializeField] private Sprite _normal = null;
    [SerializeField] private Sprite _outch = null;
    [SerializeField] private float _swapDuration = 1.0f;

    private SpriteRenderer _spriteRenderer = null;
    private Coroutine _faceSwapCoroutine = null;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
