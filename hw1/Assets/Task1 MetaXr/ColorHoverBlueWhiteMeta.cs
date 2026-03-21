using UnityEngine;

public class ColorHoverBlueWhiteMeta : MonoBehaviour
{
    private Material mat;
    public Color hoverColor = Color.white;
    private Color originalColor;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        originalColor = mat.GetColor("_BaseColor");
    }

    void OnTriggerEnter(Collider other)
    {
        mat.SetColor("_BaseColor", hoverColor);
    }

    void OnTriggerExit(Collider other)
    {
        mat.SetColor("_BaseColor", originalColor);
    }
}