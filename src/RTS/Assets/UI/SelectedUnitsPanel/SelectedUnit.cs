using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectedUnit : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TMP_Text _text;

    public void SetProperties(string text, UnityAction click, UnityAction close)
    {
        _text.text = text;
        _button.onClick.AddListener (click);
        _closeButton.onClick.AddListener(close);
    }
}
