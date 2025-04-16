using System.Collections.Generic;
using UnityEngine;

public class ManaPool : MonoBehaviour
{
    public static ManaPool Instance;
    public GameObject manaPrefab;
    public int initialPoolSize = 10;
    public float poolCleanupInterval = 10f;

    private Queue<Mana> manaPool = new Queue<Mana>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePool();
        InvokeRepeating(nameof(TrimPool), poolCleanupInterval, poolCleanupInterval);
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            Mana mana = CreateNewMana();
            if (mana != null)
            {
                mana.gameObject.SetActive(false);
                manaPool.Enqueue(mana);
            }
            else
            {
                Debug.LogError("ManaPool: 초기화 중 CreateNewMana()가 null을 반환함!");
            }
        }
    }

    private Mana CreateNewMana()
    {
        if (manaPrefab == null)
        {
            return null;
        }

        GameObject manaObject = Instantiate(manaPrefab);
        if (manaObject == null)
        {
            return null;
        }

        Mana mana = manaObject.GetComponent<Mana>();
        if (mana == null)
        {
            return null;
        }

        mana.SetPool(this);
        return mana;
    }

    public Mana GetMana(Vector3 position, Vector2 direction, ManaProperties.ManaType type, int output = 5)
    {
        Mana mana;

        if (manaPool.Count == 0) 
        {
            mana = CreateNewMana();
        }
        else
        {
            mana = manaPool.Dequeue();
            if (mana == null || mana.gameObject == null)
            {
                mana = CreateNewMana();
            }
        }

        if (mana == null)
        {
            return null;
        }

        mana.transform.position = position;
        mana.direction = direction;
        mana.currentType = type;
        mana.SetColorByType();
        mana.SetOutput(output);
        mana.gameObject.SetActive(true);

        mana.Invoke(nameof(mana.ReturnToPool), mana.maxLifetime);
        mana.Invoke(nameof(mana.EnableDetection), mana.detectDelay);

        return mana;
    }

    public void ReturnMana(Mana mana)
    {
        if (mana == null)
        {
            return;
        }

        if (manaPool.Contains(mana)) 
        {
            return;
        }

        mana.CancelInvoke(nameof(mana.ReturnToPool));

        mana.ResetMana();
        manaPool.Enqueue(mana);
    }

    private void TrimPool()
    {
        int excessManas = manaPool.Count - initialPoolSize;
        for (int i = 0; i < excessManas; i++)
        {
            if (manaPool.Count > initialPoolSize)
            {
                Destroy(manaPool.Dequeue().gameObject);
            }
        }
    }
}
