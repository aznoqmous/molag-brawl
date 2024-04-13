using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class Glyph : MonoBehaviour
{

    [SerializeField] Image _image;
    [SerializeField] Image _fillImage;
    float _life = 0f;
    float _lifeTime = 5f;
    float _size = 1f;

    public bool IsAlive
    {
        get { return _life < 1f; }
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        _life += Time.deltaTime / _lifeTime;
        _fillImage.fillAmount = _life;

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * _size, Time.deltaTime * 5f) ;

        if (!IsAlive) Die();
    }

    public void SetSize(float size)
    {
        _size = size;
    }
    public void SetLifeTime(float lifeTime)
    {
        _lifeTime = lifeTime;
    }

    void Die()
    {
        Player.Instance.Glyphs.Remove(this);
        Destroy(gameObject);
    }
}
