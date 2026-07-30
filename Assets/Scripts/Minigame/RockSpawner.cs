using UnityEngine;
using System.Collections;

namespace SubVive.Minigame
{
    public class RockSpawner : MonoBehaviour
    {
        public static RockSpawner Instance { get; private set; }

        [SerializeField] private GameObject[] rockPrefabs;
        [SerializeField] private float spawnInterval = 1.2f;
        [SerializeField] private float spawnWidth = 4f;
        [SerializeField] private float spawnHeight = 6f;
        [SerializeField] private float fallSpeed = 3f;

        public int Score { get; private set; }

        void Awake() => Instance = this;

        void Start() => StartCoroutine(SpawnLoop());

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);
                SpawnRock();
            }
        }

        private void SpawnRock()
        {
            var prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
            float x = Random.Range(-spawnWidth, spawnWidth);
            var rock = Instantiate(prefab, new Vector3(x, spawnHeight, 0), Quaternion.identity);
            rock.tag = "Rock";
            var rb = rock.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.down * fallSpeed;
        }

        public void OnRockDestroyed() => Score += 10;
    }
}