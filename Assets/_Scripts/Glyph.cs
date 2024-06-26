using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Glyph : MonoBehaviour
{

    [SerializeField] Image _image;
    [SerializeField] Image _fillImage;
    [SerializeField] Image _triangleImage;
    [SerializeField] Image _outerImage;
    [SerializeField] Image _outerAnimated;
    [SerializeField] Image _backgroundImage;

    float _life = 0f;
    float _lifeTime = 5f;
    float _size = 1f;

    float _charge = 0f;
    float _chargeTime = 5f;

    public bool IsCharged
    {
        get { return _charge >= 1f; }
    }

    [SerializeField] ParticleSystem _particleSystem;

    public float Life
    {
        get
        {
            return _life;
        }
        set
        {
            _life = value;
        }
    }
    public float LifeTime
    {
        get { return _lifeTime; }
    }

    public bool IsAlive
    {
        get { return _life > 0f; }
    }

    private void Start()
    {
        _life = 0.5f;
        transform.localScale = Vector3.zero;
        _backgroundImage.color = GameManager.Instance.GroundColor;
        AudioManager.Instance.PlaySFX(_placeAudioClip, Random.Range(0.9f, 1.1f));
    }

    void Update()
    {
        if (_isVibrating)
        {
            _outerAnimated.transform.localScale += Vector3.one * Time.deltaTime;
            Color color = _outerAnimated.color;
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * 2f);
            _outerAnimated.color = color;
        }

        _life = Mathf.Clamp01(_life);

        _fillImage.fillAmount = _life;

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * _size, Time.deltaTime * 5f) ;

        if (!IsAlive) Die();

        var mps = _particleSystem.emission;
        mps.enabled = Player.Instance.IsActivated;
    }

    public void SetSize(float size)
    {
        _size = size;
    }
    public void SetLifeTime(float lifeTime)
    {
        _lifeTime = lifeTime;
    }

    public void Die()
    {
        Player.Instance.RemoveGlyph(this);
        Destroy(gameObject);
        EntityManager.Instance.AddCredits();
        EntityManager.Instance.SpawnEnemy(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null) Player.Instance.IsCasting = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null) Player.Instance.IsCasting = false;
    }

    public void Charge()
    {
        if (IsCharged) return;
        _fillImage.color = GameManager.Instance.SoulColor;
        _triangleImage.color = GameManager.Instance.SoulColor;

        _charge += Time.deltaTime / _chargeTime;
        _triangleImage.fillAmount = _charge;
        
        if (IsCharged)
        {
            transform.localScale = Vector3.one * 1.5f * _size;
            Vibrate();
        }
    }

    bool _isVibrating = false;
    public void Vibrate()
    {
        _isVibrating = true;
        AudioManager.Instance.PlaySFX(_chargedAudioClips.PickRandom());
        _fillImage.color = Color.white;
        _triangleImage.color = Color.white;
        _outerAnimated.transform.localScale = Vector3.one;
        _outerAnimated.color = Color.white;
    }

    [SerializeField] AudioClip _placeAudioClip;
    [SerializeField] List<AudioClip> _chargedAudioClips;
}
