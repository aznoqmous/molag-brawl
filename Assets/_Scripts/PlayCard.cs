using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    float _targetScale = 1f;
    [SerializeField] Transform _container;

    private void Update()
    {
        _container.localPosition = Vector3.Lerp(_container.localPosition, _targetPosition, Time.unscaledDeltaTime * 5f);
        _container.transform.localScale = Vector3.Lerp(_container.transform.localScale, Vector3.one * _targetScale, Time.unscaledDeltaTime * 5f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Arm.Instance.ResetTargetPosition();
        SceneLoader.Instance.LoadGameScene();
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
}
