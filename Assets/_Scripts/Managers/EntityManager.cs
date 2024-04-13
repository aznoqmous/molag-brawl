using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] Enemy _enemyPrefab;
    List<Enemy> _enemies = new List<Enemy>();
    public List<Enemy> Enemies { get { return _enemies; } }

    public Enemy SpawnEnemy(Vector3 position)
    {
        Enemy enemy = Instantiate(_enemyPrefab, position, Quaternion.identity, transform);
        _enemies.Add(enemy);
        return enemy;
    }
    public Enemy SpawnEnemyAroundPlayer(float distance = 5f)
    {
        return SpawnEnemy((Vector3)Vector2.left.RotateRandom() * distance + Player.Instance.transform.position);
    }
    public void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
    }

    public Enemy GetNearestEnemy(Vector3 position)
    {
        if(_enemies.Count == 0) return null;
        if (_enemies.Count == 1) return _enemies[0];
        List<Enemy> sorted = new List<Enemy>(_enemies);
        sorted.Sort((Enemy a, Enemy b) => (int) (a.transform.position.DistanceTo(position) - b.transform.position.DistanceTo(position)));
        return sorted[0];
    }
}
