using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gitmanik.BaseCode.Tab
{
    public class TabButton : MonoBehaviour
    {
        private Tab parent;
        private int a;

        public UnityEvent OnClicked;

        public Button button;

        public void Setup(Tab p, int a)
        {
            parent = p;
            this.a = a;
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            parent.SelectTab(a);
            OnClicked?.Invoke();
        }
    }
}