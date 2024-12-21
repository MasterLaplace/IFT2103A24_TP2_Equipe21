using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomFolley : MonoBehaviour
{
    private bool start = false;
    private readonly Queue<uint> uids = new();
    private readonly Dictionary<uint, GameObject> objects = new();
    private uint uid = 0;

    public void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Simulation")
        {
            start = true;
        }
    }

    public void FixedUpdate()
    {
        if (!start)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject randomObject = new("RandomFoleyObject");
            SetRandomPosition(randomObject, 100f);
            uid = SoundManager.Instance.PlaySpatializedFoley(randomObject, SoundManager.ChooseRandomTrackFoley(), 1f, 20f, false);
            randomObject.name = $"RandomFoley_track-{uid}";
            uids.Enqueue(uid);
            objects.Add(uid, randomObject);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            bool ok = SoundManager.Instance.DeactivateTrack(uid);
            if (ok && uids.Count > 0)
            {
                uids.Dequeue();
                if (uids.Count > 0)
                {
                    uid = uids.Peek();
                    SoundManager.Instance.ActivateTrack(uid);
                }
            } else
            {
                objects.Remove(uid);
            }
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            bool ok = SoundManager.Instance.ActivateTrack(uid);
            if (ok && uids.Count > 0)
            {
                uid = uids.Peek();
            }
            else
            {
                objects.Remove(uid);
            }
        }
    }

    public void SetRandomPosition(GameObject obj, float maxDistance)
    {
        Vector3 position = new(Random.Range(-maxDistance, maxDistance), 0f, Random.Range(-maxDistance, maxDistance));
        obj.transform.position = position;
    }
}
