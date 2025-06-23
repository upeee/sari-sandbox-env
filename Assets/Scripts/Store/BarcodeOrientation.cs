using UnityEngine;

public class BarcodeOrientation : MonoBehaviour
{
    public enum OrientationVector
    {
        Blue,
        Green,
        Red
    }

    // Public field to select the orientation vector in the Inspector
    public OrientationVector selectedVector = OrientationVector.Blue;

    // Method to get the selected vector for orientation comparison
    public Vector3 GetComparisonVector()
    {
        switch (selectedVector)
        {
            case OrientationVector.Green:
                return transform.up;
            case OrientationVector.Red:
                return transform.right;
            case OrientationVector.Blue:
            default:
                return transform.forward;
        }
    }
}