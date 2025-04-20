using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManaPool : MonoBehaviour
{
    public static ManaPool Instance;
    public GameObject manaPrefab;
    public int initialPoolSize = 30;
    public float poolCleanupInterval = 10f;

    private List<Mana> manaPool = new List<Mana>();
    private HashSet<Mana> activeManas = new HashSet<Mana>();

    private float lastActivityTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        InitializePool();
        InvokeRepeating(nameof(TrimPool), poolCleanupInterval, poolCleanupInterval);
    }

    private void Start()
    {
        GameManager.Instance.OnReset += ResetManaPool;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.RegisterReset();
        }
    }

    public void ResetManaPool()
    {
        foreach (var mana in activeManas
                     .Where(m => m != null && m.gameObject != null)
                     .ToArray())
        {
            mana.CancelInvoke(nameof(mana.ReturnToPool));
            mana.CancelInvoke(nameof(mana.EnableDetection));
            mana.ReturnToPool();
        }
        TrimPool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            Mana mana = CreateNewMana();
            if (mana != null)
            {
                mana.gameObject.SetActive(false);
                manaPool.Add(mana);
            }
        }
    }

    private Mana CreateNewMana()
    {
        if (manaPrefab == null) return null;

        GameObject manaObject = Instantiate(manaPrefab, transform);
        Mana mana = manaObject.GetComponent<Mana>();
        if (mana == null) return null;

        mana.SetPool(this);
        mana.gameObject.SetActive(false);
        manaPool.Add(mana);
        return mana;
    }

    public Mana GetMana(Vector3 position, Vector2 direction, ManaProperties.ManaType type, int output = 5)
    {
        Mana mana = manaPool.FirstOrDefault(m => !m.gameObject.activeSelf);

        if (mana == null)
        {
            mana = CreateNewMana();
            if (mana == null)
            {
                Debug.LogError("Mana 생성 실패");
                return null;
            }
        }

        mana.transform.position = position;
        mana.direction = direction;
        mana.currentType = type;
        mana.SetColorByType();
        mana.SetOutput(output);
        mana.gameObject.SetActive(true);
        activeManas.Add(mana);

        lastActivityTime = Time.time;

        mana.Invoke(nameof(mana.ReturnToPool), mana.maxLifetime);
        mana.Invoke(nameof(mana.EnableDetection), mana.detectDelay);

        return mana;
    }

    public void ReturnMana(Mana mana)
    {
        if (mana == null || !mana.gameObject.activeSelf)
            return;

        mana.CancelInvoke(nameof(mana.ReturnToPool));
        mana.ResetMana();
        mana.gameObject.SetActive(false);

        activeManas.Remove(mana);
        lastActivityTime = Time.time;
    }

    private void TrimPool()
    {
        if (manaPool.Any(m => m.gameObject.activeSelf)) return;

        if (Time.time - lastActivityTime < poolCleanupInterval) return;

        int excess = manaPool.Count - initialPoolSize;
        if (excess <= 0) return;

        for (int i = 0; i < excess; i++)
        {
            Mana manaToRemove = manaPool.FirstOrDefault(m => !m.gameObject.activeSelf);
            if (manaToRemove != null)
            {
                manaPool.Remove(manaToRemove);
                activeManas.Remove(manaToRemove);
                Destroy(manaToRemove.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnReset -= ResetManaPool;
        }
    }
    
    public void UnregisterMana(Mana mana)
    {
        manaPool.Remove(mana);
        activeManas.Remove(mana);
    }

}
