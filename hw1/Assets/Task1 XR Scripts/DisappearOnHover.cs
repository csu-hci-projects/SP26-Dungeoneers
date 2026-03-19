using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DisappearOnHover : MonoBehaviour
{
    void Start()
    {
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
        {
            //Debug.LogError("XRSimpleInteractable not found " + gameObject.name);
            return;
        }
        interactable.hoverEntered.AddListener(OnHoverEntered);
        //Debug.Log("DisappearOnHover listener added " + gameObject.name);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        //Debug.Log("Hover detected should disappear ");
        gameObject.SetActive(false);
    }
}