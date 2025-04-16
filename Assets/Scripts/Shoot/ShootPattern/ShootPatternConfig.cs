using UnityEngine;

// 발사 패턴 설정 (추상 클래스)
public abstract class ShootPatternConfig : ScriptableObject
{
    public abstract void ApplyPattern(Mana mana);
}
