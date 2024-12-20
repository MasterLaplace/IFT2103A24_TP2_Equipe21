using UnityEngine;
using UnityEngine.UI;

public class SizeManager : MonoBehaviour
{
    public Slider headSizeSlider;
    public Slider bodySizeSlider;
    public Transform headTransform;
    public Transform bodyTransform;

    private void Start()
    {
        headSizeSlider.onValueChanged.AddListener(value => AdjustSize(headTransform, value));
        bodySizeSlider.onValueChanged.AddListener(value => AdjustSize(bodyTransform, value));
    }

    void AdjustSize(Transform target, float scale)
    {
        target.localScale = new Vector3(scale, scale, scale);
    }
}
