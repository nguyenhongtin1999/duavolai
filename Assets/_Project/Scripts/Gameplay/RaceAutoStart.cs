using UnityEngine;
using Unity.Netcode;

namespace MienTayDaiChien.Gameplay
{
    /// <summary>
    /// Automatically starts the NetworkManager as a Host in the editor for faster iteration.
    /// </summary>
    public class RaceAutoStart : MonoBehaviour
    {
        public bool autoStartHost = true;

        private void Start()
        {
            if (autoStartHost && NetworkManager.Singleton != null)
            {
                if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
                {
                    NetworkManager.Singleton.StartHost();
                    Debug.Log("[RaceAutoStart] Started Host automatically.");
                }
            }
        }
    }
}
