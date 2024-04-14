using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{

    [SerializeField] Image _frontImage;
    [SerializeField] Image _backImage;
    [SerializeField] Transform _container;
    [SerializeField] List<Sprite> _letterSprites = new List<Sprite>();
    [SerializeField] TextMeshProUGUI _countText;

    public Sprite Sprite { get { return _frontImage.sprite; } }

    void Start()
    {
        _letterSprites.Shuffle();
        transform.parent = null;
        transform.position = Vector3.zero;
        Empty();
    }

    void Update()
    {
        float size = Time.time - _lastFillTime < 0.1f ? 1f : 0.8f;
        _container.transform.localScale = Vector3.Lerp(_container.transform.localScale, Vector3.one * size, Time.deltaTime * 3f);
    }

    float _lastFillTime = 0;
    public bool IsFilled { get { return _frontImage.fillAmount >= 1f; } }
    public void Fill()
    {
        _frontImage.fillAmount += Time.deltaTime / Player.Instance.MaxSouls;
        _lastFillTime = Time.time;
    }

    public void SetSprite(Sprite sprite)
    {
        _frontImage.sprite = sprite;
        _backImage.sprite = sprite;
    }
    public void Show(bool state = true)
    {
        _container.gameObject.SetActive(state);
    }
    public void ShowCount(bool state = true)
    {
        _countText.gameObject.SetActive(state);
        _countText.text = $"{Player.Instance.Glyphs.Count}/{Player.Instance.MaxGlyphCount}";
    }

    public void Empty()
    {
        _frontImage.fillAmount = 0f;
    }
}
