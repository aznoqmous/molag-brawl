using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foliage : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] List<Sprite> _sprites;
    [SerializeField] Animator _animator;
    [SerializeField] float _scale = 0.5f;
    void Start()
    {
        _spriteRenderer.sprite = _sprites.PickRandom();
        transform.localScale = new Vector3(Random.value > 0.5f ? 1f : -1f, 1f, 1f) * _scale;
        GameManager.Instance.FoliageCount++;
    }

    void Update()
    {
        if (Player.Instance.transform.position.DistanceTo(transform.position) > 5f)
        {
            _animator.SetTrigger("Remove");
        }
    }

    public void Remove()
    {
        Destroy(gameObject);
        GameManager.Instance.FoliageCount--;
    }
}
