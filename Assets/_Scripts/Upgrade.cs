using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    ScriptableUpgrade _scriptableUpgrade;
    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _descriptionText;
    [SerializeField] Transform _container;

    private void Start()
    {
        _targetPosition = Vector3.zero;
    }

    public void Load(ScriptableUpgrade su)
    {
        _scriptableUpgrade = su;
        _image.sprite = su.Sprite;
        _titleText.text = su.Name;
        _descriptionText.text = su.Description;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Player.Instance.PickNextUpgrade(_scriptableUpgrade);
        GameManager.Instance.ShowUpgrades(false);
        Arm.Instance.ResetTargetPosition();
    }

    Vector3 _targetPosition;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _targetPosition = _container.localPosition + Vector3.up * 20f;
        _targetScale = 1.2f;
        Arm.Instance.SetTargetPosition(transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _targetPosition = Vector3.zero;
        _targetScale = 1f;
        Arm.Instance.ResetTargetPosition();
    }

    float _targetScale = 1f;

    private void Update()
    {
        _container.localPosition = Vector3.Lerp(_container.localPosition , _targetPosition, Time.unscaledDeltaTime * 5f);
        _container.transform.localScale = Vector3.Lerp(_container.transform.localScale, Vector3.one * _targetScale, Time.unscaledDeltaTime * 5f);
    }
}
