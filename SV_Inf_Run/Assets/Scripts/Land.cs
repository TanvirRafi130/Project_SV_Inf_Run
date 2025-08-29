using System;
using System.Collections;
using UnityEngine;

public class Land : MonoBehaviour
{
    public MeshRenderer groundMeshRenderer;
    [SerializeField] bool isActiveAtStart = false;

    private void Start()
    {
        if (isActiveAtStart)
        {
            GameManager.Instance.AddLandToMove(this.gameObject);
        }

    }

/*     void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            hasExited = false;
        }
    }
    bool hasExited = false; */
    void OnTriggerExit(Collider other)
    {
   /*      if (hasExited) return;
        hasExited = true; */
        if (other.TryGetComponent<Player>(out Player player))
        {
           
            GameManager.Instance.onLandNeed?.Invoke(this.gameObject);
        }
    }
}
