using System;
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
            string keyName = GetLocalizedKeyName(_config.skipKey);
            dialogueContainer.skipTipText.text = $"按{keyName}跳过";
        }

        private string GetLocalizedKeyName(KeyCode skipKey)
        {
            switch (skipKey)
            {
                case KeyCode.Space: return "空格";
                case KeyCode.Return: return "回车";
                case KeyCode.Escape: return "ESC";
                case (KeyCode.LeftControl or KeyCode.RightControl): return "Ctrl";
                default: return skipKey.ToString().ToUpper();
            }
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

        private float skipKeyPressTime = 0f;
        private bool isLongPressActivated = false;
        private bool isAutoForwarding = false;
        private const float longPressThreshold = 2f;
        
        private string inputBuffer = "";
        private const string CHEAT_CODE = "===";
        private const string SKIP_SCRIPT_CODE = "+++"; // 跳转到下一个脚本的作弊码

        private void Update()
        {
            if (Input.GetKey(_config.skipKey))
            {
                skipKeyPressTime += Time.deltaTime;
                
                if (architect != null && architect.isBuilding)
                {
                    architect.hurryUp = true;
                }
                else
                {
                    if (!isLongPressActivated)
                    {
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            OnUserPrompt_Next();
                        }
                    }
                }
                
                if (skipKeyPressTime >= longPressThreshold && !isLongPressActivated)
                {
                    isLongPressActivated = true;
                    isAutoForwarding = true;
                }
            }
            else
            {
                skipKeyPressTime = 0f;
                isLongPressActivated = false;
                isAutoForwarding = false;
            }
            
            if (isAutoForwarding && architect != null && !architect.isBuilding)
            {
                OnUserPrompt_Next();
            }
            
            if (Input.anyKeyDown)
            {
                string key = Input.inputString;
                if (!string.IsNullOrEmpty(key))
                {
                    inputBuffer += key;
                    
                    if (inputBuffer.Length > 3)
                    {
                        inputBuffer = inputBuffer.Substring(inputBuffer.Length - 3);
                    }
                    
                    if (inputBuffer == CHEAT_CODE)
                    {
                        Debug.Log("Cheat code detected! Jumping to Chapter 4...");
                        CommandManager.instance.Execute("startdialogue", "-f", "41");
                        inputBuffer = "";
                    }
                    else if (inputBuffer == SKIP_SCRIPT_CODE)
                    {
                        Debug.Log("Skip script code detected! Jumping to next script...");
                        
                        if (DialogueManager.instance != null && DialogueManager.instance.FileToRead != null)
                        {
                            string currentScript = DialogueManager.instance.FileToRead.name;
                            if (!string.IsNullOrEmpty(currentScript))
                            {
                                try
                                {
                                    string scriptId = System.IO.Path.GetFileNameWithoutExtension(currentScript);
                                    
                                    int scriptNumber = int.Parse(scriptId);
                                    string nextScript = (scriptNumber + 1).ToString();
                                    
                                    CommandManager.instance.Execute("startdialogue", "-f", nextScript);
                                }
                                catch (System.Exception e)
                                {
                                    Debug.LogError($"Failed to skip script: {e.Message}");
                                    Debug.LogError($"Current script name: {currentScript}");
                                }
                            }
                        }
                        inputBuffer = "";
                    }
                }
            }
        }
    }
}