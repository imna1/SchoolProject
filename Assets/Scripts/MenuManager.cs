using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _createNewScenePanel;
    [SerializeField] private GameObject _panelPrefab;
    [SerializeField] private Transform _contentOfPanels;
    [SerializeField] private TMP_InputField _inputField;
    [ContextMenu("DelPrefs")]
    public void DelPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    static public void DeleteJson(TMP_Text field)
    {
        File.Delete(Application.persistentDataPath + $"/{field.text}.json");
        Destroy(field.transform.parent.gameObject);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void OpenPanel()
    {
        _createNewScenePanel.SetActive(true);
    }
    public void ClosePanel()
    {
        _createNewScenePanel.SetActive(false);
    }
    public void CreateNewScene()
    {
        InstantiateNewScenePanel(_inputField.text);
    }
    private void InstantiateNewScenePanel(string name)
    {
        var obj = Instantiate(_panelPrefab);
        obj.transform.SetParent(_contentOfPanels);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<ScenePanel>().Text.text = name;
        ClosePanel();
    }
    private void Start()
    {
        
        if(PlayerPrefs.GetInt("IsNewbie", 0) == 0)
        {
            foreach (string s1 in Directory.GetFiles(Application.streamingAssetsPath + "/"))
            {
                if (!s1.EndsWith(".json") || s1.EndsWith("nityServicesProjectConfiguration.json"))
                    continue;
                string s2 = Application.persistentDataPath + "/" + Path.GetFileName(s1);
                if (File.Exists(s2))
                    continue;
                File.Copy(s1, s2);
            }
            PlayerPrefs.SetInt("IsNewbie", 1);
        }
        _inputField.onValueChanged.AddListener(delegate { CheckSymbols(); });
        string[] files = Directory.GetFiles(Application.persistentDataPath + "/");
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].EndsWith(".json"))
                continue;
            int index = files[i].LastIndexOf('/');
            files[i] = files[i].Replace(".json", "");
            InstantiateNewScenePanel(files[i].Remove(0, index + 1));
        }
    }
    public void CheckSymbols()
    {
        _inputField.text = _inputField.text.TrimEnd('/');
    }
}
