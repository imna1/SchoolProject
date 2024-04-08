using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DeleteButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    public void DeleteScene()
    {
        MenuManager.DeleteJson(_text);
    }
}
