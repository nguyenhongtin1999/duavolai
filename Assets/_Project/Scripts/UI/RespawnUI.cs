using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MienTayDaiChien.Gameplay;

namespace MienTayDaiChien.UI
{
    public class RespawnUI : MonoBehaviour
    {
        public Image fadeOverlay;
        public float fadeDuration = 0.5f;

        private void Start()
        {
            if (fadeOverlay != null)
            {
                Color c = fadeOverlay.color;
                c.a = 0;
                fadeOverlay.color = c;
                fadeOverlay.gameObject.SetActive(false);
            }

            // Hook into local player respawn
            StartCoroutine(FindPlayerAndHook());
        }

        private IEnumerator FindPlayerAndHook()
        {
            RespawnHandler playerRespawn = null;
            while (playerRespawn == null)
            {
                var handlers = Object.FindObjectsByType<RespawnHandler>(FindObjectsSortMode.None);
                foreach (var h in handlers)
                {
                    if (h.IsLocalPlayer)
                    {
                        playerRespawn = h;
                        break;
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }

            playerRespawn.OnRespawnStart += TriggerFade;
        }

        private void TriggerFade()
        {
            StopAllCoroutines();
            StartCoroutine(FadeSequence());
        }

        private IEnumerator FadeSequence()
        {
            fadeOverlay.gameObject.SetActive(true);
            
            // Fade In
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                SetAlpha(t / fadeDuration);
                yield return null;
            }
            SetAlpha(1);

            yield return new WaitForSeconds(0.2f); // Hold at black

            // Fade Out
            t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                SetAlpha(1 - (t / fadeDuration));
                yield return null;
            }
            SetAlpha(0);
            fadeOverlay.gameObject.SetActive(false);
        }

        private void SetAlpha(float alpha)
        {
            if (fadeOverlay == null) return;
            Color c = fadeOverlay.color;
            c.a = alpha;
            fadeOverlay.color = c;
        }
    }
}
