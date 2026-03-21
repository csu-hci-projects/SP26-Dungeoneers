using UnityEngine;

public class DisappearOnHoverMeta : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}