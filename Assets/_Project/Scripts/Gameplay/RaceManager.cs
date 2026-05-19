using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;

namespace MienTayDaiChien.Gameplay
{
    public enum RaceState { Waiting, Countdown, Racing, Finished }

    public class RaceManager : NetworkBehaviour
    {
        public static RaceManager Instance { get; private set; }

        [Header("Race Config")]
        public RaceRulesConfig raceRulesConfig;
        public int totalLaps = 3;
        public float countdownDuration = 3f;
        public RiverSpline riverSpline;

        [Header("Runtime State")]
        public NetworkVariable<RaceState> currentState = new NetworkVariable<RaceState>(RaceState.Waiting);
        public NetworkVariable<float> raceTimer = new NetworkVariable<float>(0f);
        public NetworkVariable<float> countdownTimer = new NetworkVariable<float>(0f);

        private List<RaceProgress> _racers = new List<RaceProgress>();
        private List<RaceCheckpoint> _checkpoints = new List<RaceCheckpoint>();

        public List<RaceProgress> Rankings => _racers.OrderByDescending(r => r.Lap)
                                                     .ThenByDescending(r => r.LastCheckpointIndex)
                                                     .ThenByDescending(r => r.DistanceOnSpline)
                                                     .ToList();

        private void Awake()
        {
            Instance = this;
            if (raceRulesConfig != null)
            {
                totalLaps = raceRulesConfig.totalLaps;
                countdownDuration = raceRulesConfig.countdownDuration;
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                currentState.Value = RaceState.Waiting;
            }
        }

        public void RegisterRacer(RaceProgress racer)
        {
            if (!_racers.Contains(racer)) _racers.Add(racer);
        }

        public void RegisterCheckpoint(RaceCheckpoint cp)
        {
            if (!_checkpoints.Contains(cp)) _checkpoints.Add(cp);
            _checkpoints = _checkpoints.OrderBy(c => c.checkpointIndex).ToList();
        }

        public int GetCheckpointCount() => _checkpoints.Count;
        public RaceCheckpoint GetCheckpoint(int index) => (index >= 0 && index < _checkpoints.Count) ? _checkpoints[index] : null;

        private void Update()
        {
            if (!IsServer) return;

            switch (currentState.Value)
            {
                case RaceState.Waiting:
                    // Auto-start countdown if players are ready or after delay
                    if (_racers.Count > 0) StartCountdown();
                    break;
                case RaceState.Countdown:
                    countdownTimer.Value -= Time.deltaTime;
                    if (countdownTimer.Value <= 0)
                    {
                        currentState.Value = RaceState.Racing;
                        raceTimer.Value = 0f;
                    }
                    break;
                case RaceState.Racing:
                    raceTimer.Value += Time.deltaTime;
                    // Check if all racers finished
                    if (_racers.All(r => r.isFinished.Value))
                    {
                        currentState.Value = RaceState.Finished;
                    }
                    break;
            }
        }

        public void StartCountdown()
        {
            if (!IsServer || currentState.Value != RaceState.Waiting) return;
            countdownTimer.Value = countdownDuration;
            currentState.Value = RaceState.Countdown;
        }

        public bool IsWrongWay(Transform racerTransform, float t)
        {
            if (riverSpline == null) return false;
            Vector3 forward = racerTransform.forward;
            Vector3 tangent = riverSpline.Container.EvaluateTangent(t);
            float threshold = raceRulesConfig != null ? raceRulesConfig.wrongWayDotThreshold : -0.5f;
            return Vector3.Dot(forward, tangent) < threshold; 
        }
    }
}
