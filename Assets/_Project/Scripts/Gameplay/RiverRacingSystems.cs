using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

namespace MienTayDaiChien.Gameplay
{
    /// <summary>
    /// Basic AI Boat controller that follows the river spline.
    /// Supports multiplayer sync via ghosting or simple transform interpolation.
    /// </summary>
    public class RiverAIController : MonoBehaviour
    {
        public RiverSpline targetRiver;
        public float speed = 10f;
        public float currentDistance = 0f;
        public float steeringSensitivity = 2f;
        
        [Range(-1f, 1f)]
        public float laneOffset = 0f; // Offset from river center for "racing lines"

        private void Update()
        {
            if (targetRiver == null) return;

            currentDistance += speed * Time.deltaTime;
            float t = currentDistance / targetRiver.Container.CalculateLength();
            
            if (t >= 1.0f) t = 0; // Lap logic

            Vector3 basePos = targetRiver.Container.EvaluatePosition(t);
            Vector3 tangent = targetRiver.Container.EvaluateTangent(t);
            Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;

            Vector3 targetPos = basePos + (right * laneOffset * (targetRiver.defaultWidth * 0.5f));
            
            // Smoothly move towards targetPos
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * steeringSensitivity);
            transform.rotation = Quaternion.LookRotation(tangent);
        }
    }

    /// <summary>
    /// Checkpoint for multiplayer race progress tracking.
    /// </summary>
    public class RaceCheckpoint : MonoBehaviour
    {
        public int checkpointIndex;
        public bool isFinishLine;

        [Header("Feedback")]
        public ParticleSystem passVFX;
        public AudioClip passSFX;
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            if (RaceManager.Instance != null)
                RaceManager.Instance.RegisterCheckpoint(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<RaceProgress>(out var progress))
            {
                int oldIndex = progress.LastCheckpointIndex;
                progress.OnCheckpointReached(checkpointIndex, isFinishLine);
                
                // Play feedback if progress was made
                if (progress.LastCheckpointIndex != oldIndex)
                {
                    PlayFeedback();
                }
                Debug.Log($"Checkpoint {checkpointIndex} reached by {other.name}");
            }
        }

        private void PlayFeedback()
        {
            if (passVFX != null) passVFX.Play();
            if (passSFX != null && _audioSource != null) _audioSource.PlayOneShot(passSFX);
        }
    }
}
