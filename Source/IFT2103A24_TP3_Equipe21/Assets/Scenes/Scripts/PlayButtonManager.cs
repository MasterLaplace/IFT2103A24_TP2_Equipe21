using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButtonManager : MonoBehaviour
{
    public void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => OnClick());
    }

    public void OnClick()
    {
        Debug.Log("Loading simulation scene...");
        _ = Pool.Instance.Get<Particle>(gameObject.transform);
        SoundManager.Instance.PlaySpatialSoundEffect("squalala");
        SceneManager.LoadScene("Simulation");
    }
}
