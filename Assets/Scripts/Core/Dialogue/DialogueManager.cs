using UnityEngine;
using System.Collections.Generic;

namespace DIALOGUE
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private TextAsset fileToRead = null;
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