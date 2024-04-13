using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] Rigidbody2D _rigidBody;
    [SerializeField] Collider2D _collider;

    [SerializeField] float _moveSpeed;
    Vector2 _direction;

    [Header("Glyphs")]
    [SerializeField] Glyph _glyphPrefab;
    List<Glyph> _glyphs = new List<Glyph>();
    public List<Glyph> Glyphs { get { return _glyphs; } }

    [SerializeField] int _maxGlyphCount = 3;
    [SerializeField] float _glyphSize = 1f;
    [SerializeField] float _glyphLifeTime = 10f;

    [SerializeField] LineRenderer _lineRenderer;

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnGlyph();
        }

        HandleGlyphs();
    }

    void HandleMovement() {

        _direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        if (_direction.magnitude < .1f)
        {
            _rigidBody.velocity = Vector2.Lerp(_rigidBody.velocity, Vector2.zero, 0.1f);
            return;
        }
        _direction.Normalize();
        _rigidBody.velocity = Vector2.Lerp(_rigidBody.velocity.normalized, _direction, 1f) * _moveSpeed;
    }
    

    void HandleGlyphs()
    {
        _lineRenderer.gameObject.SetActive(_glyphs.Count > 1);
        if (_glyphs.Count <= 1) {
            return;
        }
        Vector3[] positions = new Vector3[_glyphs.Count];

        _lineRenderer.positionCount = _glyphs.Count;

        int i = 0;
        foreach(Glyph glyph in _glyphs)
        {
            positions[i] = glyph.transform.position;
            i++;
        }
        _lineRenderer.SetPositions(positions);
    }
    void SpawnGlyph()
    {
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition.z = 0f;
        Glyph glyph = Instantiate(_glyphPrefab, spawnPosition, Quaternion.identity);
        glyph.SetLifeTime(_glyphLifeTime);
        glyph.SetSize(_glyphSize);
        _glyphs.Add(glyph);
    }
}

