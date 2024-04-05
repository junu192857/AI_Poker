using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null) Destroy(this);
        else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
