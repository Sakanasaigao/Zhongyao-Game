using UnityEngine;
using System.Collections.Generic;

namespace DIALOGUE
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private TextAsset fileToRead = null;
        [SerializeField] private GameObject dialogueItemPrefab = null;
        public TextAsset FileToRead => fileToRead;
        public GameObject DialogueItemPrefab => dialogueItemPrefab;
        public static DialogueManager instance { get; private set; }

        private DialogueLoaderManager dialogueLoaderManager => DialogueLoaderManager.instance;
        private Transform contentContainer;

        private void Awake()
        {
            instance = this;
        }

        public void Initialize()
        {
            // 初始化逻辑
        }

        public void SetContentContainer(Transform container)
        {
            contentContainer = container;
        }

        public Transform ContentContainer => contentContainer;

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