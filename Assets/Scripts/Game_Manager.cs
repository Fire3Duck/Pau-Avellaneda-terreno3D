using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager instance { get; private set; } //sirve para acceder (get) sea publico y que cuando quiera cambiarlo es privado (private set)
    int _stars = 0;
    int _coins = 0;
    [SerializeField] public InputActionAsset playerInputs;
    public InputAction _pauseInput;
    public bool _isPaused = false;
    
    private AudioManager _audioManager;
    public bool isPlaying = true;

    public Text starText;
    public Text coinText;
    
    void Start()
    {
        starText.text = _stars.ToString();
    }

    void Awake()
    {
        
    }
}
