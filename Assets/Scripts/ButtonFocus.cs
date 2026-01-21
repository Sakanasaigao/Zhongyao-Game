using UnityEngine;
using UnityEngine.EventSystems; 

public class ButtonFocus : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the object");
        }
    }

    
    public void OnPointerEnter()
    {
        if (animator != null)
        {
            animator.SetBool("ButtonFocus", true);
            Debug.Log("Mouse entered the button area.");
        }
    }

    public void OnPointerExit()
    {
        if (animator != null)
        {
            animator.SetBool("ButtonFocus", false);
            Debug.Log("Mouse exited the button area.");
        }
    }
}
