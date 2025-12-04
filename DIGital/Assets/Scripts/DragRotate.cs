using UnityEngine;

// class
public class DragRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;

    // function
    void OnMouseDrag()
    {
        float rotX = Input.GetAxis("Mouse X") * rotationSpeed * Mathf.Deg2Rad;
        transform.Rotate(Vector3.up, -rotX, Space.World);
    }
}