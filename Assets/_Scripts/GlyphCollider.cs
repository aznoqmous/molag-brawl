using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlyphCollider : MonoBehaviour
{
    [SerializeField] PolygonCollider2D _polygonCollider;
    public PolygonCollider2D PolygonCollider { get { return _polygonCollider; } }
    List<Enemy> _enemies = new List<Enemy>();
    public List<Enemy> Enemies { get { return _enemies; } }

    private void Update()
    {
        transform.parent = null;
        transform.position = Vector3.zero;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null) Player.Instance.IsCasting = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Player.Instance.BreakGlyphs();
            Player.Instance.IsCasting = false;
        }
    }

}
