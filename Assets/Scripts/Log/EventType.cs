public enum EventType
{
    // 세션 흐름
    SessionStart,
    SessionEnd,

    // 레벨 진행
    LevelStart,
    LevelComplete,
    LevelFail,
    LevelRestart,          // ‘Retry’ 버튼

    // 퍼즐 행동
    FireStart,             // 스타트 블럭 클릭
    ProjectileRedirect,    // 변환 블럭 통과
    HintUsed,
    Pause,
    Resume,

    // 옵션
    SettingsChanged,
    DataReset
}