using UnityEngine;
using System.Collections.Generic;

namespace SubVive.Minigame
{
    public class SubmarineController : MonoBehaviour
    {
        [SerializeField] private FaceInputController faceInput;
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float horizontalLimit = 4f;
        [SerializeField] private Transform gunPoint;
        [SerializeField] private GameObject projectilePrefab;

        void Start()
        {
            faceInput.OnBlinkShoot += Shoot;
        }

        void OnDestroy()
        {
            faceInput.OnBlinkShoot -= Shoot;
        }

        void Update()
        {
            if (MinigameManager.Instance != null && MinigameManager.Instance.IsGameOver) return;

            float move = faceInput.SteerValue * moveSpeed * Time.deltaTime;
            Vector3 pos = transform.position + new Vector3(move, 0, 0);
            pos.x = Mathf.Clamp(pos.x, -horizontalLimit, horizontalLimit);
            transform.position = pos;
        }

        private void Shoot()
        {
            if (MinigameManager.Instance != null && MinigameManager.Instance.IsGameOver) return;
            if (projectilePrefab == null || gunPoint == null) return;
            Instantiate(projectilePrefab, gunPoint.position, gunPoint.rotation);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Rock"))
            {
                Destroy(other.gameObject);
                MinigameManager.Instance?.TakeDamage();
            }
        }
    }
}