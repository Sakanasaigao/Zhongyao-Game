using UnityEngine;

[System.Serializable]
public class LevelData {
    public int levelId;         // ID
    public string chapterName;  // 【新增】对应“次序”，比如写 "第一章"
    public string levelName;    // 对应“关卡名”，比如写 "桃源问津"
    public Sprite coverImage;   // 插画
    public bool isLocked;       // 是否锁定
}