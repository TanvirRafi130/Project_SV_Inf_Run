using UnityEngine;

public class Barrier : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.TryGetComponent<Player>(out Player player))
        {
            if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
              //  Debug.LogError("Player hit the barrier!");
                // Calculate direction from barrier to player, then reverse it
                Vector3 direction = (other.transform.position - transform.position).normalized;
                rb.AddForce(direction * 30f, ForceMode.Impulse); // 30f is the force magnitude, adjust as needed
            }
        }
    }
}
