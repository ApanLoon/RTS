using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RTS.UI
{
    public class SelectedUnit : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _deflectorImage;
        [SerializeField] private Image _healthImage;
        private UnitController _controller;

        public void SetProperties(UnitController controller, UnityAction click, UnityAction close)
        {
            if (controller == null)
            {
                Debug.LogError("UI SelectedUnit.SetProperties: No UnitController");
                return;
            }

            _controller = controller;

            _text.text = _controller.Name;
            _deflectorImage.gameObject.SetActive(_controller.UnitDefinition.HasDeflector);

            _button.onClick.AddListener(click);
            _closeButton.onClick.AddListener(close);
        }

        public void Update()
        {
            if (_controller == null)
            {
                return;
            }
            _healthImage.fillAmount = _controller.Health;

            if (_controller.UnitDefinition.HasDeflector)
            {
                _deflectorImage.fillAmount = _controller.DeflectorStrength;
            }
        }
    }
}
