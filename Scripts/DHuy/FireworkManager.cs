using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkcupGames;
using DG.Tweening;

public class FireworkManager : MonoBehaviour
{
    public static FireworkManager Instance;

    public List<Transform> positions;
    public Canvas canvas;
    public List<Firework> fireworks;

    float fireworkTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        enabled = false;
    }

    private void Update()
    {
        if (Time.realtimeSinceStartup > fireworkTime && Gameplay.Instance.won)
        {
            fireworkTime = Time.realtimeSinceStartup + Random.Range(0.5f, 1f);
            SpawnFirework();
        }
    }

    IEnumerator IESpawnFirework()
    {
        for (int i = 0; i < 4; i++)
        {
            SpawnFirework();
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    public void DoEffect()
    {
        StartCoroutine(IESpawnFirework());
        enabled = true;
    }

    public void SpawnFirework()
    {
        var spawned = ObjectPool.Instance.GetGameObjectFromPool<Firework>("Fireworks/" + fireworks.RandomElement().name, Vector2.zero);
        spawned.transform.SetParent(canvas.transform);
        spawned.transform.position = positions.RandomElement().position;
    }
}
