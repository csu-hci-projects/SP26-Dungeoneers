using UnityEngine;

public class SphereTarget : MonoBehaviour
{
    private TrialManager trialManager;
    private bool selected;

    public void Initialize(TrialManager manager, string interaction)
    {
        trialManager = manager;
    }

    public void OnHoverEnter()
    {
        if (selected)
        {
            return;
        }

        Renderer rendererRef = GetComponent<Renderer>();
        if (rendererRef != null)
        {
            rendererRef.material.color = Color.green;
        }
    }

    public void OnHoverExit()
    {
        if (selected)
        {
            return;
        }

        Renderer rendererRef = GetComponent<Renderer>();
        if (rendererRef != null)
        {
            rendererRef.material.color = Color.blue;
        }
    }

    public void RaySelected()
    {
        if (selected)
        {
            return;
        }

        selected = true;
        trialManager.OnSphereSelected(true);
        gameObject.SetActive(false);
    }
}
