using System;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ReloadLevel();
    }
}
