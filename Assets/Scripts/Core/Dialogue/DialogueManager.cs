/*
 * 中药游戏项目 - DialogueManager.cs
 * 
 * 项目概述：
 * 这是一个基于Unity开发的中药主题游戏，融合了视觉小说(GAL)元素，
 * 具有完整的游戏机制和数据管理系统，包含任务、物品、对话等完整的RPG游戏要素。
 * 
 * 模块功能：
 * - 对话系统的核心管理器，采用单例模式
 * - 负责对话的流程控制和管理
 * - 支持设置要读取的对话文件和开始对话
 * - 与对话系统和文件管理器交互
 */
using UnityEngine;
using System.Collections.Generic;

namespace DIALOGUE
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private TextAsset fileToRead = null;
        // 添加public属性，用于访问当前脚本文件
        public TextAsset FileToRead => fileToRead;
        public static DialogueManager instance { get; private set; }

        private DialogueLoaderManager dialogueLoaderManager => DialogueLoaderManager.instance;

        private void Awake()
        {
            instance = this;
        }

        public void StartDialogue()
        {
            //dialogueLoaderManager.Open();  // ���˵��ҪstartDialogue�������ú󣬶Ի����������֣���ȥ��ע��
            List<string> lines = FileManager.ReadTextAsset(fileToRead);

            DialogueSystem.instance.Say(lines);
        }

        public void SetFileToRead(string filename)
        {
            //fileToRead = AssetDatabase.LoadAssetAtPath<TextAsset>(FilePaths.GetPathToResource(FilePaths.resources_gamescript, filename));
            fileToRead = Resources.Load<TextAsset>(FilePaths.GetPathToResource(FilePaths.resources_gamescript, filename));
            if (fileToRead == null)
            {
                Debug.LogError("No file found");
            }
        }

        public void EndDialogue()
        {
            dialogueLoaderManager.Close();
        }
        
    }
}