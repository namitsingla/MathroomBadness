using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class ExitDoor : MonoBehaviour
{
    collectedisplay collectedisplay;
    public Renderer targetRenderer;
    private MaterialPropertyBlock mpb;

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    public static float glowIntensity = 1.5f;
    public GameObject lockedIcon;
    AudioSource exitDoorFailSound;
    GameObject exitDoorFailText;
    public static int requiredItems = 3;

    public bool isProcessing = false;
    TextMeshProUGUI exitDoorFailTMP;

    void Awake()
    {
        collectedisplay = ReferencesManager.instance.collecteddisplay;
        exitDoorFailText = ReferencesManager.instance.exitDoorFailText;
        exitDoorFailSound = ReferencesManager.instance.exitDoorFailSound;
        mpb = new MaterialPropertyBlock();
        exitDoorFailTMP = exitDoorFailText.GetComponent<TextMeshProUGUI>();

        if (targetRenderer != null)
            targetRenderer.material.EnableKeyword("_EMISSION");
    }

    void OnEnable()
    {
        isProcessing = false;
        DeactivateExitDoor();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isProcessing) return;

        if (collectedisplay.collected < requiredItems)
        {
            isProcessing = true;
            exitDoorFailSound.Play();
            StartCoroutine(ExitDoorFailText());
        }
        else
        {
            isProcessing = true;
            RoundManager.instance.OnRoundEnd();
        }
    }

    IEnumerator ExitDoorFailText()
    {
        exitDoorFailTMP.text = collectedisplay.collected == 0
            ? "You need at least " + requiredItems + " items"
            : "You need at least " + (requiredItems - collectedisplay.collected) + " more items";

        exitDoorFailText.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        exitDoorFailText.SetActive(false);
        isProcessing = false;
    }

    public void ActivateExitDoor()
    {
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(BaseColorID, Color.green);
        mpb.SetColor(EmissionColorID, Color.green * glowIntensity);
        targetRenderer.SetPropertyBlock(mpb);
        lockedIcon.SetActive(false);
    }

    public void DeactivateExitDoor()
    {
        if (targetRenderer == null) return;
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(BaseColorID, Color.red);
        mpb.SetColor(EmissionColorID, Color.red * glowIntensity);
        targetRenderer.SetPropertyBlock(mpb);
        lockedIcon.SetActive(true);
    }
}