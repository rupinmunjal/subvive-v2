using UnityEngine;

namespace SubVive.Minigame
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 3f;

        void Start() => Destroy(gameObject, lifetime);

        void Update() => transform.position += Vector3.up * speed * Time.deltaTime;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Rock"))
            {
                Destroy(other.gameObject);
                Destroy(gameObject);
                RockSpawner.Instance?.OnRockDestroyed();
            }
        }
    }
}