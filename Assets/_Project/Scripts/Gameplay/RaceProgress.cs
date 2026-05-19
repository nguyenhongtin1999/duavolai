using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using Unity.Netcode;

namespace MienTayDaiChien.Gameplay
{
    /// <summary>
    /// Tracks individual racer progress. Synchronized across the network.
    /// Only the server can validate and update lap/checkpoint state.
    /// </summary>
    public class RaceProgress : NetworkBehaviour
    {
        [Header("Synced State")]
        public NetworkVariable<int> currentLap = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<int> lastCheckpointIndex = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<float> distanceOnSpline = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<bool> isFinished = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<bool> isWrongWay = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private RiverSpline _river;

        public int Lap => currentLap.Value;
        public int LastCheckpointIndex => lastCheckpointIndex.Value;
        public float DistanceOnSpline => distanceOnSpline.Value;

        public override void OnNetworkSpawn()
        {
            if (RaceManager.Instance != null)
                RaceManager.Instance.RegisterRacer(this);
            
            _river = RaceManager.Instance.riverSpline;
        }

        private void Update()
        {
            if (!IsServer) return; // Only server calculates progress
            if (isFinished.Value || _river == null) return;

            // Authoritative Spline Tracking
            float t;
            SplineUtility.GetNearestPoint(_river.Container.Spline, 
                _river.Container.transform.InverseTransformPoint(transform.position), 
                out float3 nearest, out t);
            
            distanceOnSpline.Value = t;
            
            // Authoritative Wrong Way Detection
            isWrongWay.Value = RaceManager.Instance.IsWrongWay(transform, t);
        }

        /// <summary>
        /// Called when racer enters a checkpoint trigger.
        /// Client detects, server validates.
        /// </summary>
        public void OnCheckpointReached(int index, bool isFinishLine)
        {
            if (!IsServer)
            {
                SubmitCheckpointServerRpc(index, isFinishLine);
                return;
            }

            ProcessCheckpoint(index, isFinishLine);
        }

        [ServerRpc]
        private void SubmitCheckpointServerRpc(int index, bool isFinishLine)
        {
            ProcessCheckpoint(index, isFinishLine);
        }

        private void ProcessCheckpoint(int index, bool isFinishLine)
        {
            int expected = lastCheckpointIndex.Value + 1;
            int totalCPs = RaceManager.Instance.GetCheckpointCount();
            
            if (expected >= totalCPs) expected = 0;

            if (index == expected)
            {
                lastCheckpointIndex.Value = index;
                
                if (isFinishLine)
                {
                    currentLap.Value++;
                    if (currentLap.Value >= RaceManager.Instance.totalLaps)
                    {
                        isFinished.Value = true;
                        Debug.Log($"Server: {gameObject.name} finished!");
                    }
                }
            }
        }
    }
}
