using System.Collections;
using System.Collections.Generic;
using ITEMS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ExplorePot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("��������")]
    [SerializeField] private float hoverScaleMultiplier = 1.1f;
    [SerializeField] private float animationDuration = 0.2f;

    [Header("�����ȡ����Ʒ")]
    public string medicineName;

    private Vector3 originalScale;
    private bool isAnimating = false;
    private Coroutine currentCoroutine;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 targetScale = originalScale * hoverScaleMultiplier;
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ScaleTo(targetScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ScaleTo(originalScale));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //onClickEvent?.Invoke();
        MedicineDisplayManager.instance.InitializeMedicineDisplay(medicineName);

        ItemsManager.instance.AddToWarehouse(medicineName);
    }

    private System.Collections.IEnumerator ScaleTo(Vector3 targetScale)
    {
        isAnimating = true;
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            float t = elapsed / animationDuration;
            t = Mathf.SmoothStep(0, 1, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        isAnimating = false;
    }


}
