using System.Collections.Generic;
using UnityEngine;
public class DebugInfoPanel : MonoBehaviour
{
    private static DebugInfoPanel _instance;

    public Transform Container;
    public GameObject ItemPrefab;

    private readonly Dictionary<string, GameObject> _items = new();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("DebugInfoPanel: Detected multiple instances.");
            gameObject.SetActive(false);
            return;
        }
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public static void Log(string key, string value)
    {
        if (_instance == null)
        {
            return;
        }

        DebugInfoItem item = null;
        if (_instance._items.ContainsKey(key) == false)
        {
            var go = Instantiate(_instance.ItemPrefab, _instance.Container);
            item = go.GetComponent<DebugInfoItem>();
            item.SetKey(key);
            _instance._items.Add(key, go);
        }

        if (item == null)
        {
            item = _instance._items[key].GetComponent<DebugInfoItem>();
        }
        
        item.SetValue(value);
    }

    public static void Remove(string key)
    {
        if (_instance == null || _instance._items.ContainsKey(key) == false)
        {
            return;
        }

        var item = _instance._items[key];
        Destroy(item);
        _instance._items.Remove(key);
    }
}
