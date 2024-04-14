using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    //[SerializeField] Enemy _enemyPrefab;
    [SerializeField] List<Enemy> _enemyList;

    float _credits = 0f;
    public float Credits { get { return _credits;  } }


    List<Enemy> _enemies = new List<Enemy>();
    public List<Enemy> Enemies { get { return _enemies; } }

    public void AddCredits(float amount=1f)
    {
        _credits += amount * GameManager.Instance.Difficulty;
    }
    public Enemy PickEnemy()
    {
        List<Enemy> available = _enemyList.Where((Enemy e)=> e.Cost < _credits).ToList();
        if (available.Count <= 0) return _enemyList[0];
        return available.PickRandom();
    }
    public Enemy SpawnEnemy(Vector3 position)
    {
        Enemy prefab = PickEnemy();
        Enemy enemy = Instantiate(prefab, position, Quaternion.identity, transform);
        _enemies.Add(enemy);
        _credits -= enemy.Cost;
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

    public void Clear()
    {
        foreach (Enemy enemy in _enemies) Destroy(enemy.gameObject);
        _enemies.Clear();
    }
}
