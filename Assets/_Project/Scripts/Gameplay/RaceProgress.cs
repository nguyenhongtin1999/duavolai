using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using Unity.Netcode;

namespace MienTayDaiChien.Gameplay
{
    public class RaceProgress : NetworkBehaviour
    {
        [Header("Synced State")]
        public NetworkVariable<int> currentLap = new NetworkVariable<int>(0);
        public NetworkVariable<int> lastCheckpointIndex = new NetworkVariable<int>(-1);
        public NetworkVariable<float> distanceOnSpline = new NetworkVariable<float>(0f);
        public NetworkVariable<bool> isFinished = new NetworkVariable<bool>(false);
        public NetworkVariable<bool> isWrongWay = new NetworkVariable<bool>(false);
        public NetworkVariable<float> finishTime = new NetworkVariable<float>(0f);

        private RiverSpline _river;
        private BoatController _boat;

        public int Lap => currentLap.Value;
        public int LastCheckpointIndex => lastCheckpointIndex.Value;
        public float DistanceOnSpline => distanceOnSpline.Value;

        public override void OnNetworkSpawn()
        {
            _boat = GetComponent<BoatController>();
            if (RaceManager.Instance != null)
                RaceManager.Instance.RegisterRacer(this);
            
            _river = RaceManager.Instance.riverSpline;
        }

        private void Update()
        {
            if (!IsServer) return; 
            if (isFinished.Value || _river == null) return;

            // Authoritative Spline Tracking
            float t;
            SplineUtility.GetNearestPoint(_river.Container.Spline, 
                _river.Container.transform.InverseTransformPoint(transform.position), 
                out float3 nearest, out t);
            
            distanceOnSpline.Value = t;
            
            // Wrong Way Detection
            isWrongWay.Value = RaceManager.Instance.IsWrongWay(transform, t);

            // Handle Boat Control based on Race State
            if (RaceManager.Instance.currentState.Value == RaceState.Countdown || RaceManager.Instance.currentState.Value == RaceState.Waiting)
            {
                // Freeze boat during countdown
                _boat.SetInput(0, 0, 0, false);
            }
        }

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
            if (isFinished.Value) return;

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
                        finishTime.Value = RaceManager.Instance.raceTimer.Value;
                        _boat.SetInput(0, 0, 1, false); // Auto-brake on finish
                    }
                }
            }
        }

        public void ResetToLastCheckpoint()
        {
            if (!IsServer)
            {
                ResetToLastCheckpointServerRpc();
                return;
            }

            RaceCheckpoint lastCP = RaceManager.Instance.GetCheckpoint(lastCheckpointIndex.Value);
            if (lastCP != null)
            {
                _boat.Respawn(lastCP.transform.position + Vector3.up * 2, lastCP.transform.rotation);
            }
            else
            {
                // Fallback to start
                _boat.Respawn(Vector3.up * 5, Quaternion.identity);
            }
        }

        [ServerRpc]
        private void ResetToLastCheckpointServerRpc()
        {
            ResetToLastCheckpoint();
        }
    }
}
