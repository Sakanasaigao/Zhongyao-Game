using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class PlayerInputManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
            {
                PromptAdvance();
            }
            
            // 检测Ctrl键按下/释放以控制对话快进
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                SetHurryUp(true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                SetHurryUp(false);
            }
        }

        public void PromptAdvance()
        {
            DialogueSystem.instance.OnUserPrompt_Next();
            Debug.Log("Next Line");
        }
        
        private void SetHurryUp(bool value)
        {
            // 通过反射获取DialogueSystem中的architect实例
            // 注意：这假设DialogueSystem中有一个名为architect的TextArchitect字段
            // 如果字段名称不同，需要相应调整
            var architectField = typeof(DialogueSystem).GetField("architect", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (architectField != null)
            {
                TextArchitect architect = architectField.GetValue(DialogueSystem.instance) as TextArchitect;
                if (architect != null)
                {
                    architect.hurryUp = value;
                }
            }
        }
    }
}