#if GITMANIK_CONSOLE_USED
using Gitmanik.Console;
#endif

using TMPro;
using UnityEngine;

namespace Gitmanik.Base {
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        public static FPSCounter singleton;

        private void Start()
        {
            singleton = this;
        }

        void Update()
        {
            text.text = Mathf.Round(1 / Time.deltaTime) + "FPS";
        }

#if GITMANIK_CONSOLE_USED
        [ConsoleCommand()]
        private static bool FPSCounterToggle(string[] args)
        {
            singleton.transform.parent.gameObject.SetActive(!singleton.transform.parent.gameObject.activeSelf);
            return true;
        }
#endif
    }
}
