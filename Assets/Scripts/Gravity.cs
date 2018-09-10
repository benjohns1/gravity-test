using UnityEngine;

public class Gravity : MonoBehaviour
{
    public bool useGravity = true;

    public bool autoOrientDownwards;

    public float autoOrientSqrMagThreshold;

    public Vector3 gravityVelocity;

    private void OnDestroy()
    {
        GravitySystem.Instance.Unregister(this);
    }
}
