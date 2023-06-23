using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class MenuSettings : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private AudioMixer _audioMixer;
    [SerializeField]
    private TMP_Dropdown _resolution;
    [SerializeField]
    private TMP_Dropdown _quality;
    private Resolution[] _resolutions;
    [SerializeField]
    private Toggle _fullScreen;
    [SerializeField]
    private TextMeshProUGUI _percentVolumeMainSound;
    [SerializeField]
    private TextMeshProUGUI _percentVolumeMusic;
    [SerializeField]
    private TextMeshProUGUI _percentVolumeBackgroundSounds;
    private static bool _isFirstSetBool=true;
    private float _levelVolumeMainSound;
    private float _levelVolumeMusic;
    private float _levelVolumeBackgroundSound;

    [Header("Menu Factions")]
    [SerializeField]
    private TMP_InputField _inputFieldName;
    private string _nameGame;
    [SerializeField]
    private TMP_Dropdown _map;
    private string _nameMap;
    private ListMap _listMap;
    private int _numberOfTeams;
    private List<Gods> _faction;
    private SaveSettings _saveSettings;
    private int _currentTeam;

    
    //////////////////////////////////////
    [SerializeField]
    private RectTransform _baseButton;

    private List<GameObject> _buttons = new List<GameObject>();

    [SerializeField]
    private GameObject _factionTeamSelectionMenu;
    [SerializeField]
    private GameObject _factionSelectionMenu;

    [Header("Menu Factions Buttons")]
    [SerializeField]
    private Button _swarogButton;
    [SerializeField]
    private Button _swietowidButton;
    [SerializeField]
    private Button _perunButton;
    [SerializeField]
    private Button _dziewannaButton;
    [SerializeField]
    private Button _marzannaButton;
    [SerializeField]
    private Button _swarogButton1;
    [SerializeField]
    private Button _swietowidButton1;
    [SerializeField]
    private Button _perunButton1;
    [SerializeField]
    private Button _dziewannaButton1;
    [SerializeField]
    private Button _marzannaButton1;
    [Header("Menu Factions Buttons Ticks")]
    [SerializeField]
    private GameObject _swarogButtonTick;
    [SerializeField]
    private GameObject _swietowidButtonTick;
    [SerializeField]
    private GameObject _perunButtonTick;
    [SerializeField]
    private GameObject _dziewannaButtonTick;
    [SerializeField]
    private GameObject _marzannaButtonTick;
    [Header("Information")]
    [SerializeField]
    private TextMeshProUGUI _informationSection;
    [SerializeField]
    private TextMeshProUGUI _informationBuffs;
    [SerializeField]
    private Image _informationPortait;
    [SerializeField]
    private List <MenuInformation> _menuInformation;

    [SerializeField]
    private List<AudioClip> listMusics;
    private AudioSource _audioSource;


    void Start()
    {
        _saveSettings = SaveSettings.Instance;
        _resolutions = Screen.resolutions;
        _faction = new List<Gods>();
        _listMap = ListMap.Instance;
        _audioSource = FindObjectOfType<AudioSource>();
        _audioSource.loop = false;

        _resolution.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);

            if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        _resolution.AddOptions(options);
        _resolution.value = currentResolutionIndex;
        _resolution.RefreshShownValue();
        Screen.fullScreen = _fullScreen.isOn;

        //_audioMixer.GetFloat("main_volume", out float mainV);
        //_mainSoundSlider.value = mainV;

        //_audioMixer.GetFloat("back_volume", out float backV);
        //_backgroundSoundsSlider.value = backV;

        //_audioMixer.GetFloat("music_volume", out float musicV);
        //_musicSlider.value = musicV;
        if (_isFirstSetBool)
        {
            _audioMixer.SetFloat("music_volume", 0f);
            _audioMixer.SetFloat("back_volume", 0f);
            _audioMixer.SetFloat("main_volume", 0f);
            _isFirstSetBool = false;
        }
        
        _audioMixer.GetFloat("back_volume", out _levelVolumeMusic);
        _percentVolumeBackgroundSounds.text = (_levelVolumeMusic + 80).ToString() + "%";
        _audioMixer.GetFloat("main_volume", out _levelVolumeMainSound);
        _percentVolumeMainSound.text = (_levelVolumeMainSound + 80).ToString() + "%";
        _audioMixer.GetFloat("music_volume", out _levelVolumeBackgroundSound);
        _percentVolumeMusic.text = (_levelVolumeBackgroundSound + 80).ToString() + "%";


        //menu factions
        //_inputFieldName.onEndEdit.AddListener(delegate { setNameGame(_inputFieldName); });
    }

    public void Update()
    {
#if DEBUG
        //only for testing purposes
        if (Input.GetKeyDown(KeyCode.T))
        {
            _nameGame = "testGame";

            _nameMap = _map.options[0].text;
            _numberOfTeams = _listMap.typeMap.GetValueOrDefault(_nameMap).numberOfTeams;

            _faction.Add(new Gods { gods = (Gods.Factions)0 });
            _faction.Add(new Gods { gods = (Gods.Factions)1 });


            _saveSettings.Save(_nameGame, _nameMap, _numberOfTeams, _faction);

            SceneManager.LoadScene("MapSelectionTest", LoadSceneMode.Single);
        }
#endif
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = GetRandomClip();
            _audioSource.Play();
        }
    }

    private AudioClip GetRandomClip()
    {
        return listMusics[UnityEngine.Random.Range(0, listMusics.Count)];
    }
    public void setResolution(int resolutionIn)
    {
        Resolution res = _resolutions[resolutionIn];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
    public void setFullscreen()
    {
        Screen.fullScreen = _fullScreen.isOn;
    }
    public void setQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }
    
    public void setMainSoundButton(float volume)
    {
        float now = _levelVolumeMainSound + volume;
        if (now>=-80 && now <= 20)
        {
            _audioMixer.SetFloat("main_volume", now);
            _percentVolumeMainSound.text = (now+80).ToString()+"%";
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

    //functions menu factions
    public void setNameGame()
    {
        _nameGame = _inputFieldName.text;
    }
    public void SetMap()
    {
        //_nameMap = name;
        _nameMap = _map.options[_map.value].text;
        _numberOfTeams = _listMap.typeMap.GetValueOrDefault(_nameMap).numberOfTeams;
        //
    }
    public void setFaction(int god)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#6C5252", out color);
        Color color1;
        ColorUtility.TryParseHtmlString("FFFFFF", out color1);
        switch (_faction[--_currentTeam].gods)
        {
            case Gods.Factions.dziewanna:
                _dziewannaButton.interactable = true;
                _dziewannaButton.gameObject.GetComponent<Image>().color = color1;
                break;
            case Gods.Factions.marzanna:
                _marzannaButton.interactable = true;
                _marzannaButton.gameObject.GetComponent<Image>().color = color1;
                break;
            case Gods.Factions.perun:
                _perunButton.interactable = true;
                _perunButton.gameObject.GetComponent<Image>().color = color1;
                break;
            case Gods.Factions.swarog:
                _swarogButton.interactable = true;
                _swarogButton.gameObject.GetComponent<Image>().color = color1;
                break;
            case Gods.Factions.swietowid:
                _swietowidButton.interactable = true;
                _swietowidButton.gameObject.GetComponent<Image>().color = color1;
                break;
        }
        _faction[_currentTeam] = new Gods {gods=(Gods.Factions)god };
        switch (_faction[_currentTeam].gods)
        {
            case Gods.Factions.dziewanna: 
                _dziewannaButton.interactable = false; 
                _dziewannaButton.gameObject.GetComponent<Image>().color = color;
                break;
            case Gods.Factions.marzanna:
                _marzannaButton.interactable = false;
                _marzannaButton.gameObject.GetComponent<Image>().color = color;
                break;
            case Gods.Factions.perun:
                _perunButton.interactable = false;
                _perunButton.gameObject.GetComponent<Image>().color = color;
                break;
            case Gods.Factions.swarog:
                _swarogButton.interactable = false;
                _swarogButton.gameObject.GetComponent<Image>().color = color;
                break;
            case Gods.Factions.swietowid:
                _swietowidButton.interactable = false;
                _swietowidButton.gameObject.GetComponent<Image>().color = color;
                break;
        }
        _informationSection.text = _menuInformation[5].informationSection;
        _informationBuffs.text = _menuInformation[5].informationBuffs;
        _informationPortait.sprite = _menuInformation[5].informationPortrait;

    }
    //public void test()
    //{
    //    Debug.Log("Nazwa: " + _nameGame + " Iloœæ: " + _numberOfTeams + " Druzyna: " + _teams[0]);
    //    Debug.Log("Nazwa: " + _nameGame + " Iloœæ: " + _numberOfTeams + " Druzyna: " + _teams[1]);
    //}
    public void Save()
    {
        _saveSettings.Save(_nameGame, _nameMap, _numberOfTeams,_faction);
    }

    public void GenerateButtonsTeams()
    {
        for (int i = _buttons.Count - 1; i >= 0; i--)
        {
            Destroy(_buttons[i]);
        }

        _buttons.Clear();

        Vector3 lastPosition = _baseButton.transform.position;

        if (_nameMap==null)
        {
            _nameMap = _map.options[0].text;
            _numberOfTeams = _listMap.typeMap.GetValueOrDefault(_nameMap).numberOfTeams;
        }
        for(int i = 0; i < _numberOfTeams; i++)
        {
            _faction.Add(new Gods { gods = Gods.Factions.empty });
        }
        

        for (int i=0;i<_numberOfTeams;i++)
        {
           
            GameObject temp = Instantiate(_baseButton.gameObject);
            temp.transform.SetParent(_baseButton.transform.parent);
            temp.transform.position = lastPosition;

            temp.GetComponent<Button>().onClick.AddListener(() => _factionSelectionMenu.SetActive(true));
            


            if (i%2==0)
                lastPosition += new Vector3(500, 0, 0);
            else if(i % 2 == 1)
                lastPosition += new Vector3(-500, -400, 0);
            temp.SetActive(true);
            temp.gameObject.GetComponent<IdButton>().id = i+1;
            temp.gameObject.GetComponentInChildren<TMP_Text>().text = "Team " + (i + 1);
            temp.GetComponent<Button>().onClick.AddListener(() => SetCurrentTeam(temp.gameObject.GetComponent<IdButton>().id));
            _buttons.Add(temp);
        }
    }
    public void SetCurrentTeam(int number)
    {
        _currentTeam = number;
    }
    public bool isAllGodsChoosen()
    {
        for(int i = 0; i < _numberOfTeams; i++)
        {
            if (_faction[i].gods == Gods.Factions.empty)
            {
                return false;
            }
        }
        return true;
        
    }

    public void SetInformation()
    {
        GameObject clickedObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        int id=clickedObject.GetComponent<IdButton>().id;
        _informationSection.text = _menuInformation[id].informationSection;
        _informationBuffs.text = _menuInformation[id].informationBuffs;
        _informationPortait.sprite = _menuInformation[id].informationPortrait;
        switch((Gods.Factions)id)
        {
            case Gods.Factions.dziewanna:
                if (_dziewannaButton.interactable)
                    _dziewannaButtonTick.SetActive(true);
                break;
            case Gods.Factions.marzanna:
                if (_marzannaButton.interactable)
                    _marzannaButtonTick.SetActive(true);
                break;
            case Gods.Factions.perun:
                if (_perunButton.interactable)
                    _perunButtonTick.SetActive(true);
                break;
            case Gods.Factions.swarog:
                if (_swarogButton.interactable)
                    _swarogButtonTick.SetActive(true);
                break;
            case Gods.Factions.swietowid:
                if (_swietowidButton.interactable)
                    _swietowidButtonTick.SetActive(true);
                break;
        }
    }
}
