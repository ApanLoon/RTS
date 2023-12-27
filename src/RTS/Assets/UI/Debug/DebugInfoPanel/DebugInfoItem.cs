using TMPro;
using UnityEngine;

public class DebugInfoItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _keyText;
    [SerializeField] private TMP_Text _valueText;

    public void SetKey(string key)
    {
        _keyText.text = key;
    }
    public void SetValue(string value)
    {
        _valueText.text = value;
    }
}
