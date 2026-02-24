using System;
using System.Collections;
using UnityEngine;

public class StoryTransition : MonoBehaviour
{
    public static StoryTransition Instance { get; private set; }
    public Animator animator;
    [Header("把云朵的 Animator 拖进来")]
    public Animator cloudAnimator;
    
    [Header("云朵遮住屏幕需要的时间 (秒)")]
    public float coverTime = 1f;

    private void Awake()
    {
        // 单例模式，方便从任何地方调用
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 执行剧情切换：云合拢 -> 执行逻辑 -> 云散开
    /// </summary>
    /// <param name="actionDuringCover">云遮住时要执行的代码（比如加载文字）</param>
    public void Play(Action actionDuringCover)
    {
        StartCoroutine(ProcessTransition(actionDuringCover));
    }

    IEnumerator ProcessTransition(Action actionDuringCover)
    {
        // 1. 播放合拢动画 (复用原来的 Trigger)
        if (cloudAnimator != null)
        {
            cloudAnimator.SetTrigger("Start");
        }

        // 2. 等待云朵完全遮住屏幕
        yield return new WaitForSeconds(coverTime);

        // 3. 【核心】执行你传入的“加载剧情”代码
        Debug.Log("☁️ 云雾已遮蔽，正在偷偷切换剧情...");
        actionDuringCover?.Invoke();

        // 4. 稍微停顿一下（防闪烁）
        yield return new WaitForSeconds(0.5f);

        // 5. 播放散开动画
        // 【注意】：我们需要在 Animator 里加一个叫 "End" 的 Trigger 让云散开
        // 如果你的动画是自动循环的，这步可能不需要，但在同场景切换中通常需要手动散开
        if (cloudAnimator != null)
        {
            cloudAnimator.SetTrigger("End"); 
        }
    }
}