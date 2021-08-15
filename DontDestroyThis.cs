using UnityEngine;

namespace Gitmanik.BaseCode
{
    public class DontDestroyThis : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
