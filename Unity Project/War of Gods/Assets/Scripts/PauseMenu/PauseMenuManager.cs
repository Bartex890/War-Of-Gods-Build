using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    private RectTransform _menuTransform;
    private bool _isMenuOpen = false;

    //Audio
    [SerializeField]
    private AudioMixer _audioMixer;
    [SerializeField]
    private TextMeshProUGUI _percentVolumeMainSound;
    [SerializeField]
    private TextMeshProUGUI _percentVolumeMusic;
    [SerializeField]
    private TextMeshProUGUI _percentVolumeBackgroundSounds;
    private float _levelVolumeMainSound;
    private float _levelVolumeMusic;
    private float _levelVolumeBackgroundSound;
    [SerializeField]
    private List<AudioClip> listMusics;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _menuTransform = GetComponent<RectTransform>();
        _audioSource = FindObjectOfType<AudioSource>();
        _audioSource.loop = false;
        float current;
        _audioMixer.GetFloat("back_volume", out _levelVolumeMusic);
        _percentVolumeBackgroundSounds.text = (_levelVolumeMusic + 80).ToString() + "%";
        _audioMixer.GetFloat("main_volume", out _levelVolumeMainSound);
        _percentVolumeMainSound.text = (_levelVolumeMainSound + 80).ToString() + "%";
        _audioMixer.GetFloat("music_volume", out _levelVolumeBackgroundSound);
        _percentVolumeMusic.text = (_levelVolumeBackgroundSound + 80).ToString() + "%";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isMenuOpen)
            {
                HideMenu();
            }
            else
            {
                ShowMenu();
            }
        }
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = GetRandomClip();
            _audioSource.Play();
        }
    }

    public void ShowMenu()
    {
        _isMenuOpen = true;
        _menuTransform.LeanMove(new Vector3(0, 0, 0), 0.2f).setEaseInOutQuad();
        Time.timeScale = 0.0f;
    }

    public void HideMenu()
    {
        _isMenuOpen = false;
        _menuTransform.LeanMove(new Vector3(0, -1200, 0), 0.2f).setEaseInOutQuad();
        Time.timeScale = 1.0f;
    }

    public void QuitToMainMenu()
    {
        GameManager.onTurnEnded = null;
        GameManager.onRoundEnded = null;
        Destroy(SaveSettings.Instance.gameObject);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    private AudioClip GetRandomClip()
    {
        return listMusics[UnityEngine.Random.Range(0, listMusics.Count)];
    }

    public void setMainSoundButton(float volume)
    {
        float now = _levelVolumeMainSound + volume;
        if (now >= -80 && now <= 20)
        {
            _audioMixer.SetFloat("main_volume", now);
            _percentVolumeMainSound.text = (now + 80).ToString() + "%";
            Debug.Log(now);
            _levelVolumeMainSound = now;
        }

    }
    public void setBSoundButton(float volume)
    {
        float now = _levelVolumeBackgroundSound + volume;
        if (now >= -80 && now <= 20)
        {
            _audioMixer.SetFloat("back_volume", now);
            _percentVolumeBackgroundSounds.text = (now + 80).ToString() + "%";
            _levelVolumeBackgroundSound = now;
        }

    }
    public void setMSoundButton(float volume)
    {
        float now = _levelVolumeMusic + volume;
        if (now >= -80 && now <= 20)
        {
            _audioMixer.SetFloat("music_volume", now);
            _percentVolumeMusic.text = (now + 80).ToString() + "%";
            _levelVolumeMusic = now;
        }

    }


}
