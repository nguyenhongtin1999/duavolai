using UnityEngine;
using Unity.Netcode;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections;

namespace MienTayDaiChien.Gameplay
{
    /// <summary>
    /// Handles authoritative boat respawning with stuck detection and fairness logic.
    /// Works in both Singleplayer and Multiplayer (NGO).
    /// </summary>
    [RequireComponent(typeof(BoatController))]
    public class RespawnHandler : NetworkBehaviour
    {
        [Header("Stuck Detection")]
        public float stuckSpeedThreshold = 1.0f;
        public float stuckTimeThreshold = 4.0f;
        public float offTrackRadius = 40f;

        [Header("Fairness")]
        public float respawnBacktrackDistance = 10f; // Meters to move back from current progress

        private BoatController _boat;
        private RaceProgress _progress;
        private float _stuckTimer;
        private bool _isRespawning;

        public event System.Action OnRespawnStart;

        private void Awake()
        {
            _boat = GetComponent<BoatController>();
            _progress = GetComponent<RaceProgress>();
        }

        private void Update()
        {
            if (!IsServer) return; // Only server handles authoritative respawns

            if (_progress != null && !_progress.isFinished.Value)
            {
                CheckStuck();
                CheckOffTrack();
            }
        }

        private void CheckStuck()
        {
            // If boat is moving very slowly while having input, increment timer
            if (_boat.CurrentSpeed < stuckSpeedThreshold && Time.time > 5f) // Grace period at start
            {
                _stuckTimer += Time.deltaTime;
                if (_stuckTimer >= stuckTimeThreshold)
                {
                    _stuckTimer = 0;
                    RequestRespawn();
                }
            }
            else
            {
                _stuckTimer = 0;
            }
        }

        private void CheckOffTrack()
        {
            if (RaceManager.Instance == null || RaceManager.Instance.riverSpline == null) return;

            // Check distance from spline
            float3 nearest;
            float t;
            SplineUtility.GetNearestPoint(RaceManager.Instance.riverSpline.Container.Spline, 
                RaceManager.Instance.riverSpline.Container.transform.InverseTransformPoint(transform.position), 
                out nearest, out t);

            Vector3 worldNearest = RaceManager.Instance.riverSpline.Container.transform.TransformPoint((Vector3)nearest);
            if (Vector3.Distance(transform.position, worldNearest) > offTrackRadius)
            {
                RequestRespawn();
            }
        }

        /// <summary>
        /// Public entry point for manual or auto respawn.
        /// </summary>
        public void RequestRespawn()
        {
            if (_isRespawning) return;

            if (IsClient && !IsServer)
            {
                RequestRespawnServerRpc();
            }
            else
            {
                StartCoroutine(RespawnSequence());
            }
        }

        [ServerRpc]
        private void RequestRespawnServerRpc()
        {
            StartCoroutine(RespawnSequence());
        }

        private IEnumerator RespawnSequence()
        {
            _isRespawning = true;

            // 1. Notify Clients to show Fade/VFX
            NotifyRespawnClientRpc();

            // 2. Wait for visual transition (e.g., 0.5s fade)
            yield return new WaitForSeconds(0.5f);

            // 3. Authoritative Repositioning
            if (RaceManager.Instance != null && RaceManager.Instance.riverSpline != null)
            {
                // Find respawn point: slightly behind current progress T
                float currentT = _progress.distanceOnSpline.Value;
                float length = RaceManager.Instance.riverSpline.Container.CalculateLength();
                float backtrackT = respawnBacktrackDistance / length;
                float spawnT = Mathf.Max(0, currentT - backtrackT);

                var container = RaceManager.Instance.riverSpline.Container;
                Vector3 pos = container.EvaluatePosition(spawnT);
                Vector3 tan = container.EvaluateTangent(spawnT);

                _boat.Respawn(pos + Vector3.up * 2, Quaternion.LookRotation(tan));
            }

            yield return new WaitForSeconds(0.5f);
            _isRespawning = false;
        }

        [ClientRpc]
        private void NotifyRespawnClientRpc()
        {
            OnRespawnStart?.Invoke();
            Debug.Log("Respawn triggered for " + gameObject.name);
        }
    }
}
