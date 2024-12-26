using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset;

    public void OnMouseDown()
    {
        offset = transform.position - BuildingSystem.GetMouseWorldPosition();
    }

    public void OnMouseDrag()
    {
        Vector3 pos = BuildingSystem.GetMouseWorldPosition() + offset;
        transform.position = BuildingSystem.Instance.SnapCoordinateToGrid(pos);
    }
}