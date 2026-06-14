using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ARCHIVE;
using DIALOGUE;
using ITEMS;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneAMenu : MonoBehaviour
{
    public static SceneAMenu Instance { get; private set; }

    public static PrescriptionDataSO CurrentPrescription { get; private set; }
    public static string PendingDialogueScript { get; set; }

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject mountain;
    [SerializeField] private string transationStyle;

    [Header("Decoct Entry Guard")]
    [SerializeField] private Button startDecoctingBtn;
    [SerializeField] private TMP_Text unlockTooltip;
    [SerializeField] private GameObject tooltipPanel;

    private PrescriptionDataSO[] allPrescriptions;
    private bool decoctBtnInitialized;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadAllPrescriptions();
        UpdateDecoctButtonState();

        if (ItemWarehouse.Instance != null)
            ItemWarehouse.Instance.OnItemAdded += OnItemAdded;

        // 检查是否有从Decoct场景返回的待触发对话
        if (!string.IsNullOrEmpty(PendingDialogueScript))
        {
            var script = PendingDialogueScript;
            PendingDialogueScript = null;
            StartCoroutine(TriggerPendingDialogue(script));
        }
    }

    private IEnumerator TriggerPendingDialogue(string script)
    {
        yield return null; // 等一帧，确保对话系统就绪
        if (CommandManager.instance != null)
            CommandManager.instance.Execute("StartDialogue", "-f", script);
    }

    private void OnDestroy()
    {
        if (ItemWarehouse.Instance != null)
            ItemWarehouse.Instance.OnItemAdded -= OnItemAdded;
    }

    private void LoadAllPrescriptions()
    {
        allPrescriptions = Resources.LoadAll<PrescriptionDataSO>("Prescriptions");
    }

    private void OnItemAdded(string itemName)
    {
        UpdateDecoctButtonState();
    }

    private void UpdateDecoctButtonState()
    {
        PrescriptionDataSO matched = FindMatchingPrescription();
        CurrentPrescription = matched;

        if (startDecoctingBtn != null)
            startDecoctingBtn.interactable = matched != null;

        if (matched != null && tooltipPanel != null && unlockTooltip != null)
        {
            tooltipPanel.SetActive(true);
            unlockTooltip.text = $"已解锁 {matched.prescriptionName}";
        }
        else if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    private PrescriptionDataSO FindMatchingPrescription()
    {
        if (allPrescriptions == null || allPrescriptions.Length == 0)
            return null;

        foreach (var prescription in allPrescriptions)
        {
            if (prescription.requiredHerbs.All(herb => ItemWarehouse.Instance.HasItem(herb)))
                return prescription;
        }

        return null;
    }

    public void OpenMenuPanel()
    {
        menuPanel.SetActive(true);
    }
    public void CloseMenuPanel()
    {
        menuPanel.SetActive(false);
    }

    public void TurnToMainMenu()
    {
        bool flowControl = TurnToScene(0);
        if (!flowControl)
        {
            return;
        }
    }

    public void TurnToDecoct()
    {
        if (CurrentPrescription == null)
        {
            Debug.LogWarning("No matching prescription found, cannot enter Decoct scene");
            return;
        }

        bool flowControl = TurnToScene(5);
        if (!flowControl)
        {
            return;
        }
    }

    public bool TurnToScene(int scene)
    {
        if (SceneLoaderManager.Instance == null)
        {
            Debug.LogWarning("SceneLoaderManager instance not found, skipping scene transition");
            return false;
        }

        if (string.IsNullOrEmpty(transationStyle))
        {
            Debug.LogWarning("Transition style not set, skipping scene transition");
            return false;
        }

        SceneLoaderManager.Instance.TransitionToScene(transationStyle, scene);

        if (ArchivingManager.Instance != null)
        {
            ArchivingManager.Instance.Save();
        }
        else
        {
            Debug.LogWarning("ArchivingManager instance not found, skipping save");
        }

        return true;
    }

    public void TurnToMountain()
    {
        if (SceneLoaderManager.Instance == null)
        {
            Debug.LogWarning("SceneLoaderManager instance not found, skipping scene transition");
            return;
        }

        if (string.IsNullOrEmpty(transationStyle))
        {
            Debug.LogWarning("Transition style not set, skipping scene transition");
            return;
        }

        SceneLoaderManager.Instance.TransitionToScene(transationStyle, 2);

        if (ArchivingManager.Instance != null)
        {
            ArchivingManager.Instance.Save();
        }
        else
        {
            Debug.LogWarning("ArchivingManager instance not found, skipping save");
        }
    }

    public void CloseMountain()
    {
        mountain.SetActive(false);
    }

    public void OpenMountain()
    {
        mountain.SetActive(true);
    }
}