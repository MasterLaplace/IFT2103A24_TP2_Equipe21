using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public Slider volumeSlider;

    public void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        else
        {
            Debug.LogError("Slider n'est pas assigné dans l'inspecteur !");
        }
    }

    private void OnSliderValueChanged(float value)
    {
        if (gameObject.name == "SliderHead")
        {
            PlayerDetailsManager.Instance.AddScale("Head", value);
        }
        else if (gameObject.name == "SliderBody")
        {
            PlayerDetailsManager.Instance.AddScale("Body", value);
        }
        else if (gameObject.name == "SliderMusic")
        {
            SoundManager.Instance.SetLayerVolume(SoundManager.Layer.Melody, value);
        }
        else if (gameObject.name == "SliderAmbiance")
        {
            SoundManager.Instance.SetLayerVolume(SoundManager.Layer.Folley, value);
        }
        else if (gameObject.name == "SliderSound")
        {
            SoundManager.Instance.SetLayerVolume(SoundManager.Layer.SoundEffect, value);
        }
    }
}
