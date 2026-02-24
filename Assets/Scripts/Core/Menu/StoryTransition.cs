using System;
using System.Collections;
using UnityEngine;

public class StoryTransition : MonoBehaviour
{
    public static StoryTransition Instance { get; private set; }
    
    // 【恢复原状】绝不再碰 CrossFade，只专注您的云朵
    [Header("把云朵的 Animator 拖进来")]
    public Animator cloudAnimator;
    
    [Header("云朵遮住屏幕需要的时间 (秒)")]
    public float coverTime = 1f;

    private void Awake()
    {
        // 您的原版单例逻辑，原封不动
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Play(Action actionDuringCover)
    {
        StartCoroutine(ProcessTransition(actionDuringCover));
    }

    IEnumerator ProcessTransition(Action actionDuringCover)
    {
        // 1. 【借鉴 SceneLoad 的安全触发逻辑】
        // 如果云朵被您禁用了，我们需要唤醒它才能播动画
        if (cloudAnimator != null && !cloudAnimator.gameObject.activeSelf)
        {
            cloudAnimator.gameObject.SetActive(true);
            // 核心修复：刚唤醒的 Animator 内部状态机需要一帧来就位
            // 不加这行，Trigger 会被吞掉，导致直接闪现“合拢”然后消失
            yield return null; 
        }

        // 2. 播放合拢动画 (您原版的 Trigger 名字)
        if (cloudAnimator != null)
        {
            cloudAnimator.SetTrigger("Start");
        }

        // 3. 等待云朵完全遮住屏幕
        yield return new WaitForSeconds(coverTime);

        // 4. 【核心】执行您的软跳转逻辑（UI切换、推剧本）
        Debug.Log("☁️ 云雾已遮蔽，正在偷偷切换剧情...");
        actionDuringCover?.Invoke();

        // 5. 稍微停顿一下（防闪烁，您原版代码的精髓）
        yield return new WaitForSeconds(0.5f);

        // 6. 播放散开动画
        if (cloudAnimator != null)
        {
            cloudAnimator.SetTrigger("End"); 
        }

        // 7. 【善后清理】等云朵散开后，再次将其禁用，不浪费一丁点性能
        yield return new WaitForSeconds(coverTime);
        if (cloudAnimator != null)
        {
            cloudAnimator.gameObject.SetActive(false);
        }
    }
}