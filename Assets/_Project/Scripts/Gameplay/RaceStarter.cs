using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

namespace MienTayDaiChien.Gameplay
{
    /// <summary>
    /// Handles spawning and starting the race.
    /// Runs on the Server/Host.
    /// </summary>
    public class RaceStarter : NetworkBehaviour
    {
        [Header("Prefabs")]
        public GameObject playerPrefab;
        public GameObject aiPrefab;

        [Header("Spawn Points")]
        public List<Transform> spawnPoints = new List<Transform>();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                SpawnRacers();
            }
        }

        private void SpawnRacers()
        {
            // For a demo/test: Spawn local player at point 0, AI at others
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                GameObject prefab = (i == 0) ? playerPrefab : aiPrefab;
                GameObject instance = Instantiate(prefab, spawnPoints[i].position, spawnPoints[i].rotation);
                
                var networkObj = instance.GetComponent<NetworkObject>();
                networkObj.Spawn();

                if (i > 0) // Setup AI personality
                {
                    var ai = instance.GetComponent<RiverAIRacer>();
                    if (ai != null)
                    {
                        ai.personality = (AIPersonality)(i % 4); // Variety of personalities
                    }
                }
                
                Debug.Log($"Spawned Racer {i} at {spawnPoints[i].name}");
            }
        }
    }
}
