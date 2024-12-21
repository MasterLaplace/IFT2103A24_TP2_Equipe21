using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionOutline;
    private CanvasGroup outlineCanvasGroup;
    private static ButtonSelectionManager currentSelected;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        outlineCanvasGroup = selectionOutline.GetComponent<CanvasGroup>();
        Deselect();
    }

    void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (currentSelected != null && currentSelected != this)
        {
            currentSelected.Deselect();
        }

        Select();
        currentSelected = this;
    }

    public void Select()
    {
        if (selectionOutline != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }
    }

    public void Deselect()
    {
        if (selectionOutline != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float duration = 0.2f;
        float elapsed = 0f;
        outlineCanvasGroup.alpha = 0f;
        selectionOutline.SetActive(true);

        while (elapsed < duration)
        {
            outlineCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        outlineCanvasGroup.alpha = 0.5f;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            outlineCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        outlineCanvasGroup.alpha = 0f;
        selectionOutline.SetActive(false);
    }
}
