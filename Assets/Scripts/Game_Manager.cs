using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game_Manager : MonoBehaviour
{
    public static PlayerData Instance;
    public static Game_Manager instance { get; private set; } //sirve para acceder (get) sea publico y que cuando quiera cambiarlo es privado (private set)
    [SerializeField] public InputActionAsset playerInputs;
    
    public bool isPlaying = true;
    
    void Start()
    {
        
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
