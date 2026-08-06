using UnityEngine;
using UnityEngine.UI;

namespace SubVive.Minigame
{
    [RequireComponent(typeof(Button))]
    public class RestartButton : MonoBehaviour
    {
        void Start()
        {
            var btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => {
                    if (MinigameManager.Instance != null)
                    {
                        MinigameManager.Instance.RestartMinigame();
                    }
                });
            }
        }
    }
}