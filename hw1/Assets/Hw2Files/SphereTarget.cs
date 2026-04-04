using UnityEngine;
using Oculus.Interaction;

public class SphereTarget : MonoBehaviour
{
    private TrialManager trialManager;
    private string requiredInteraction;
    private bool selected = false;

    public void Initialize(TrialManager manager, string interaction)
    {
        trialManager = manager;
        requiredInteraction = interaction;
    }

    public void OnHoverEnter()
    {
        if (selected) return;
        GetComponent<Renderer>().material.color = Color.green;
    }

    public void OnHoverExit()
    {
        if (selected) return;
        GetComponent<Renderer>().material.color = Color.blue;
    }

    public void RaySelected()
    {
        if (selected) return;
        selected = true;
        trialManager.OnSphereSelected(true);
        gameObject.SetActive(false);
    }
}