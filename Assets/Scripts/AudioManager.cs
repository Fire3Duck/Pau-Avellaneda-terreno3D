using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    private AudioSource _audioSource;

    void Awake()
    {
        if (instance != null & instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

    }

    public void ReproduceSound(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    public void ChangeBGM(AudioClip bgmClip)
    {
        _bgmSource.clip = bgmClip;
        _bgmSource.Play();
    }

   
}
