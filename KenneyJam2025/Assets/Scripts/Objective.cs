using System;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public int currentLevel = 0;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.FinishLevel(currentLevel);
    }
}
