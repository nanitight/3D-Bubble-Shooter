using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameBall : MonoBehaviour
{
    protected GameManager manager; 

    private void Awake()
    {
        manager = FindAnyObjectByType<GameManager>();
    }
}
