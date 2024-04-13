using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour, IPointerClickHandler
{
    ScriptableUpgrade _scriptableUpgrade;
    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _descriptionText;

    public void Load(ScriptableUpgrade su)
    {
        _scriptableUpgrade = su;
        _image.sprite = su.Sprite;
        _titleText.text = su.Name;
        _descriptionText.text = su.Description; 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Player.Instance.SelectUpgrade(_scriptableUpgrade);
        GameManager.Instance.ShowUpgrades(false);
    }
}
