using UnityEngine; // <--- 之前就是缺了这一行！

[System.Serializable]
public class LevelData {
    public int levelId;         
    public string chapterName;  
    public string levelName;    
    public Sprite coverImage;   
    public bool isLocked;       


    public string scriptFileName; 
}