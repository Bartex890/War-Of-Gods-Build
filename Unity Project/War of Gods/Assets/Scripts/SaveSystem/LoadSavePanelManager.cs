using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class LoadSavePanelManager : MonoBehaviour
{
    private RectTransform _scrollContainer;
    private Button _exampleSaveButton;
    private RectTransform _exampleSaveTransform;
    private TMP_Text _exampleSaveName;

    public void Start()
    {
        _scrollContainer = transform.Find("SaveSelectionPanel/ScrollableContent").GetComponent<RectTransform>();
        _exampleSaveButton = transform.Find("SaveSelectionPanel/ScrollableContent/ExampleSave").GetComponent<Button>();
        _exampleSaveTransform = _exampleSaveButton.GetComponent<RectTransform>();

        _exampleSaveName = _exampleSaveButton.GetComponentInChildren<TMP_Text>();

        _exampleSaveButton.gameObject.SetActive(false);

        GenerateButtons();
    }

    public void GenerateButtons()
    {
        string saveFolderPath = Application.persistentDataPath + "/Saves";
        string[] fileDirectories = Directory.GetFiles(saveFolderPath);

        string[] fileNames = new string[fileDirectories.Length];
        int i = 0;
        foreach (string fileDirectory in fileDirectories)
        {
            string fileName = fileDirectory.Remove(0, saveFolderPath.Length + 1);
            fileName = fileName.Remove(fileName.Length - 5, 5);

            fileNames[i] = fileName;
            i++;
        }

        _scrollContainer.sizeDelta = new Vector2(1000, Mathf.Max(800, fileNames.Length*120));

        Vector3 nextItemPosition = new Vector3(0, _scrollContainer.sizeDelta.y / 2 - 60, 0);

        foreach (string fileName in fileNames)
        {
            _exampleSaveName.text = fileName;

            RectTransform newButton = Instantiate(_exampleSaveTransform);
            newButton.SetParent(_scrollContainer);
            newButton.localPosition = nextItemPosition;
            newButton.localScale = Vector3.one;
            newButton.GetComponent<Button>().onClick.AddListener(() => SaveSettings.Instance.SetSaveFile(fileName));
            newButton.gameObject.SetActive(true);

            nextItemPosition += new Vector3(0, -120, 0);
            _exampleSaveButton.onClick.RemoveAllListeners();
        }
    }
}
