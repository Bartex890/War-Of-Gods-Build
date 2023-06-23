using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class AlertManager : MonoBehaviour
{
    private RectTransform _exampleAlert;
    private TMP_Text _exampleAlertText;

    private List<RectTransform> _alerts = new List<RectTransform>();

    private static AlertManager _instance;
    public static AlertManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("AlertManager is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void Start() {
        _exampleAlert = transform.Find("ExampleMessage").GetComponent<RectTransform>();
        _exampleAlertText = _exampleAlert.Find("Message").GetComponent<TMP_Text>();
    }

    public static void AddAlert(string message)
    {
        float startingY = -40;
        float offset = -80;
        float length = 0.1f;

        _instance._exampleAlertText.text = message;
        RectTransform newAlert = Instantiate(_instance._exampleAlert);
        newAlert.SetParent(_instance._exampleAlert.parent);
        newAlert.position = _instance._exampleAlert.position;
        newAlert.LeanMoveLocalY(startingY, length).setEaseOutCubic();

        for (int i = _instance._alerts.Count - 1; i >= 0; i--)
        {
            if (_instance._alerts[i] == null)
            {
                _instance._alerts.RemoveAt(i);
                continue;
            }

            _instance._alerts[i].LeanMoveLocalY(startingY + offset * (i + 1), length).setEaseOutCubic();
        }

        _instance._alerts.Insert(0, newAlert);

        _instance.StartCoroutine(_WaitAndFade(newAlert.GetComponent<CanvasGroup>()));
    }

    private static IEnumerator _WaitAndFade(CanvasGroup canvasGroup)
    {
        yield return new WaitForSeconds(3);
        canvasGroup.LeanAlpha(0, 1f).setOnComplete(() => { Destroy(canvasGroup.gameObject); });
    }

    private void OnEnable()
    {
        foreach(RectTransform alert in _alerts)
        {
            _instance.StartCoroutine(_WaitAndFade(alert.GetComponent<CanvasGroup>()));
        }
    }
}
