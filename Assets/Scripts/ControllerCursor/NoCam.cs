using UnityEngine;

public class IgnoreLayerForCamera : MonoBehaviour
{
    [Tooltip("The name of the layer to ignore")]
    public string layerToIgnore = "IgnoreCamera";

    private void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("This script must be attached to a Camera object.");
            return;
        }

        int layerIndex = LayerMask.NameToLayer(layerToIgnore);
        if (layerIndex == -1)
        {
            Debug.LogError($"Layer '{layerToIgnore}' does not exist. Please create it in Project Settings > Tags and Layers.");
            return;
        }

        // Remove the layer from the camera's culling mask
        cam.cullingMask &= ~(1 << layerIndex);
    }
}
