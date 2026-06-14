using UnityEngine;
using System.Collections;
using CHARACTERS;

namespace DIALOGUE
{
    public class DialogueLoaderManager : MonoBehaviour
    {
        public static DialogueLoaderManager instance { get; private set; }

        public GameObject root;
        public GameObject dialogueLoader;
        public GameObject charactersContainer;
        public CanvasGroup rootCG => root != null ? root.GetComponent<CanvasGroup>() : null;

        protected Coroutine co_revealing;
        protected Coroutine co_closing;

        public bool isVisible => rootCG != null && rootCG.alpha != 0;
        public bool isRevealing => co_revealing != null;
        public bool isClosing => co_closing != null;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // 如果场景引用为空，尝试从场景中找到它们
            if (root == null)
            {
                var dialogueSystemGO = GameObject.Find("DialogueSystem");
                if (dialogueSystemGO != null)
                {
                    var ds = dialogueSystemGO.GetComponent<DialogueSystem>();
                    if (ds != null && ds.dialogueContainer.root != null)
                    {
                        root = ds.dialogueContainer.root;
                        dialogueLoader = root.transform.Find("DialogueLoader")?.gameObject;
                        charactersContainer = root.transform.Find("CharactersPanel")?.gameObject;
                    }
                }
            }
        }

        public Coroutine Open(float speedMultiplier = 1f)
        {
            if (root == null || rootCG == null)
                return null;

            if (isRevealing)
                return co_revealing;

            if (isClosing)
                StopCoroutine(co_closing);

            co_revealing = StartCoroutine(OpeningOrClosing(true, speedMultiplier));
            return co_revealing;
        }

        public Coroutine Close(float speedMultiplier = 1f)
        {
            if (root == null || rootCG == null)
                return null;

            if (isClosing)
                return co_closing;

            if (isRevealing)
                StopCoroutine(co_revealing);

            co_closing = StartCoroutine(OpeningOrClosing(false, speedMultiplier));
            return co_closing;
        }

        public void ResetDialogueLoader()
        {
            if (charactersContainer == null)
                return;

            // 清除CharacterManager中的角色缓存
            if (CharacterManager.instance != null)
                CharacterManager.instance.ClearCharacters();

            Transform characters = charactersContainer.transform;

            if (characters != null)
            {
                foreach (Transform character in characters)
                {
                    if (character != null)
                        Destroy(character.gameObject);
                }
            }
        }

        private IEnumerator OpeningOrClosing(bool show, float speedMultiplier)
        {
            CanvasGroup self = rootCG;
            if (self == null)
                yield break;

            float targetAlpha = show ? 1f : 0f;

            if (show)
            {
                self.alpha = 0f;
                root.SetActive(true);
                self.interactable = true;
                self.blocksRaycasts = true;
            }

            while (!Mathf.Approximately(self.alpha, targetAlpha))
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime * speedMultiplier);
                yield return null;
            }

            if (!show)
            {
                root.SetActive(false);
                self.interactable = false;
                self.blocksRaycasts = false;
            }

            co_revealing = co_closing = null;
        }
    }
}