using UnityEngine;

namespace Gitmanik.BaseCode.Tab
{
    public class Tab : MonoBehaviour
    {
        public GameObject[] Tabs;
        public TabButton[] Buttons;

        [SerializeField] private int DefaultTabIndex;
        void Start()
        {
            for (int a = 0; a < Buttons.Length; a++)
            {
                Buttons[a].Setup(this, a);
            }
            SelectTab(DefaultTabIndex);
        }
        public void SelectTab(int n)
        {
            for (int a = 0; a < Tabs.Length; a++)
            {
                Tabs[a].SetActive(a == n);
                Buttons[a].button.interactable = a != n;
            }
        }
    }
}
