using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class SoundOnHover : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        var interactable = GetComponent<XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(onHoverExited);

    }

    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void onHoverExited(HoverExitEventArgs args)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}