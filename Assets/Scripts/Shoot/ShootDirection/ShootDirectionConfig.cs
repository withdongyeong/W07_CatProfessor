using UnityEngine;

// 발사 방향 설정 (추상 클래스)
public abstract class ShootDirectionConfig : ScriptableObject
{
    public abstract Vector2[] GetShootDirections();
}
