using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ColorHoverBlueWhite : MonoBehaviour
{
    private Material mat;
    public Color hoverColor = Color.white;
    private Color originalColor;

    void Start()
    {
        mat = GetComponentInChildren<MeshRenderer>().material;
        originalColor = mat.GetColor("_BaseColor");

        var interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        mat.SetColor("_BaseColor", hoverColor);
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        mat.SetColor("_BaseColor", originalColor);
    }
}