using System.Collections;
using System.Collections.Generic;
using CHARACTERS;
using UnityEngine;

namespace DIALOGUE
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField]private DialogueSystemConfigurationSO _config;
        public DialogueSystemConfigurationSO config =>_config;

        public DialogueContainer dialogueContainer = new DialogueContainer();
        private ConversationManager conversationManager;
        private TextArchitect architect;

        private const string narratorName = "narrator";

        public static DialogueSystem instance { get; private set; }

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPrompt_Next;

        public bool isRunningConversation => conversationManager.isRunning;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject);
        }

        bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) return;

            architect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager = new ConversationManager(architect);
        }

        public void OnUserPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();
        }

        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            Character character = CharacterManager.instance.GetCharacter(speakerName);
            ItemConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);
            ApplySpeakerDataToDialogueContainer(config);
        }

        public void ApplySpeakerDataToDialogueContainer(ItemConfigData config)
        {
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            dialogueContainer.nameContainer.SetNameColor(config.nameColor);
            dialogueContainer.nameContainer.SetNameFont(config.nameFont);
        }

        public void ShowSpeakerName(string speakerName = "")
        {
            if (speakerName.ToLower() != narratorName)
            {
                dialogueContainer.nameContainer.Show(speakerName);
            }
            else
                HideSpeakerName();
        }
        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\" " };
            return Say(conversation);
        }

        public Coroutine Say(List<string> conversation)
        {
            return conversationManager.StartConversation(conversation);
        }

        // 空格键长按相关变量
        private float spaceKeyPressTime = 0f;
        private bool isLongPressActivated = false;
        private bool isAutoForwarding = false;
        private const float longPressThreshold = 2f; // 长按阈值（秒）
        
        // 开发后门相关变量
        private string inputBuffer = "";
        private const string CHEAT_CODE = "==="; // 作弊码
        private const string SKIP_SCRIPT_CODE = "+++"; // 跳转到下一个脚本的作弊码

        private void Update()
        {
            // 检测空格键输入
            if (Input.GetKey(KeyCode.Space))
            {
                // 空格键被按住，增加计时
                spaceKeyPressTime += Time.deltaTime;
                
                // 如果正在显示对话文本
                if (architect != null && architect.isBuilding)
                {
                    // 设置hurryUp为true，使文本快进显示
                    architect.hurryUp = true;
                }
                else
                {
                    // 如果文本已经显示完成
                    if (!isLongPressActivated)
                    {
                        // 短按触发下一段对话
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            OnUserPrompt_Next();
                        }
                    }
                }
                
                // 检测是否达到长按阈值
                if (spaceKeyPressTime >= longPressThreshold && !isLongPressActivated)
                {
                    isLongPressActivated = true;
                    isAutoForwarding = true;
                }
            }
            else
            {
                // 空格键松开，重置计时和状态
                spaceKeyPressTime = 0f;
                isLongPressActivated = false;
                isAutoForwarding = false;
            }
            
            // 自动快进逻辑
            if (isAutoForwarding && architect != null && !architect.isBuilding)
            {
                // 如果当前没有正在显示的文本，自动触发下一段对话
                OnUserPrompt_Next();
            }
            
            // 开发后门：检测连续输入 ===
            if (Input.anyKeyDown)
            {
                // 获取按下的键
                string key = Input.inputString;
                if (!string.IsNullOrEmpty(key))
                {
                    // 添加到输入缓冲区
                    inputBuffer += key;
                    
                    // 只保留最后3个字符
                    if (inputBuffer.Length > 3)
                    {
                        inputBuffer = inputBuffer.Substring(inputBuffer.Length - 3);
                    }
                    
                    // 检测作弊码
                    if (inputBuffer == CHEAT_CODE)
                    {
                        Debug.Log("Cheat code detected! Jumping to Chapter 4...");
                        // 跳转到第四章
                        CommandManager.instance.Execute("startdialogue", "-f", "41");
                        // 清空输入缓冲区
                        inputBuffer = "";
                    }
                    // 检测跳转到下一个脚本的作弊码
                    else if (inputBuffer == SKIP_SCRIPT_CODE)
                    {
                        Debug.Log("Skip script code detected! Jumping to next script...");
                        
                        // 获取当前脚本ID
                        if (DialogueManager.instance != null && DialogueManager.instance.FileToRead != null)
                        {
                            string currentScript = DialogueManager.instance.FileToRead.name;
                            if (!string.IsNullOrEmpty(currentScript))
                            {
                                try
                                {
                                    // 去掉文件扩展名，如"41.txt" → "41"
                                    string scriptId = System.IO.Path.GetFileNameWithoutExtension(currentScript);
                                    
                                    // 直接将当前脚本ID + 1，实现X1→X2跳转
                                    int scriptNumber = int.Parse(scriptId);
                                    string nextScript = (scriptNumber + 1).ToString();
                                    
                                    // 执行跳转命令
                                    CommandManager.instance.Execute("startdialogue", "-f", nextScript);
                                }
                                catch (System.Exception e)
                                {
                                    Debug.LogError($"Failed to skip script: {e.Message}");
                                    Debug.LogError($"Current script name: {currentScript}");
                                }
                            }
                        }
                        // 清空输入缓冲区
                        inputBuffer = "";
                    }
                }
            }
        }
    }
}