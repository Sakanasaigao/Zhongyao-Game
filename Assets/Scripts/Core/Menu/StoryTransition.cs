using System;
using System.Collections;
using UnityEngine;

public class StoryTransition : MonoBehaviour
{
    public static StoryTransition Instance { get; private set; }
    
    [Header("把云朵的 Animator 拖进来")]
    public Animator cloudAnimator;
    
    [Header("云朵遮住屏幕需要的时间 (秒)")]
    public float coverTime = 3f;

    // 防抖锁：防止同一时间多次触发导致动画抽搐
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Play(Action actionDuringCover)
    {
        // 拦截并发触发
        if (isTransitioning) return;
        
        StartCoroutine(ProcessTransition(actionDuringCover));
    }

    IEnumerator ProcessTransition(Action actionDuringCover)
    {
        isTransitioning = true; // 上锁

        // 1. 唤醒云朵并等待一帧
        if (cloudAnimator != null && !cloudAnimator.gameObject.activeSelf)
        {
            cloudAnimator.gameObject.SetActive(true);
            yield return null; 
        }

        // 2. 【已为您彻底回退】恢复原版的 Trigger 触发方式
        if (cloudAnimator != null)
        {
            cloudAnimator.SetTrigger("Start");
        }

        // 3. 等待遮挡
        yield return new WaitForSecondsRealtime(coverTime);

        // 4. 执行软跳转逻辑
        actionDuringCover?.Invoke();

        // 5. 停顿防闪烁
        yield return new WaitForSecondsRealtime(0.5f);

        // 6. 播放散开动画
        if (cloudAnimator != null)
        {
            cloudAnimator.SetTrigger("End"); 
        }

        // 7. 善后清理
        yield return new WaitForSecondsRealtime(coverTime);
        
        if (cloudAnimator != null)
        {
            cloudAnimator.gameObject.SetActive(false);
        }

        isTransitioning = false; // 解锁
    }
}