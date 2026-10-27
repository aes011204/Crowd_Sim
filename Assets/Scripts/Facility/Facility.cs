using UnityEngine;

public class Facility : MonoBehaviour
{
    [SerializeField]
    private Transform usePoint;

    [SerializeField, Min(0f)]
    private float useTime = 3f;

    public Vector3 UsePosition => usePoint.position;
    public float UseTime =>useTime;
}
