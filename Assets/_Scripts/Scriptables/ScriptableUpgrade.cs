using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Upgrade", menuName="Upgrade")]
public class ScriptableUpgrade : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Sprite;
    public UpgradeType UpgradeType;
    
}

public enum UpgradeType
{
    MoveSpeed,
    TickTime,
    TickDamage,
    BulletDamage,
    BulletTime,
    MaxGlyphDistance,
    LifeSteal
}