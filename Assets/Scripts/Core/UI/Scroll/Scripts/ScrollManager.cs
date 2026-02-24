using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : MonoBehaviour
{
    public void OpenScroll()
    {
        UIManager.Instance.OpenPanel<ScrollSystem>();
    }
}
