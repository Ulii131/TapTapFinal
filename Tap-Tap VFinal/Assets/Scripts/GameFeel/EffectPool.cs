using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance;
    public HitEffect prefab;
    public int initialSize = 10;

    Queue<HitEffect> pool = new Queue<HitEffect>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(prefab, transform);
            go.gameObject.SetActive(false);
            pool.Enqueue(go);
        }
    }

    public HitEffect Spawn()
    {
        HitEffect item;
        if (pool.Count == 0)
        {
            item = Instantiate(prefab, transform);
        }
        else
        {
            item = pool.Dequeue();
        }

        item.transform.SetParent(null);
        item.gameObject.SetActive(true);
        return item;
    }

    public void Return(HitEffect item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(transform);
        pool.Enqueue(item);
    }
}
