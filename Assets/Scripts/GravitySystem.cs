using UnityEngine;

public class GravitySystem : MonoBehaviour
{
    public static GravitySystem Instance;

    public float GravityUpdateFrequency = 0.5f;
    public float RotationAdjustmentSpeed = 1f;
    public float MassMultiplier = 1000f;

    private Gravity[] masses;
    private Rigidbody[] rbs;

    private float lastGravityUpdateTime;

    private void Awake()
    {
        masses = GameObject.FindObjectsOfType<Gravity>();
        rbs = new Rigidbody[masses.Length];

        if (Instance != null)
        {
            throw new System.Exception("GravitySystem singleton already exists");
        }
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < masses.Length; i++)
        {
            if (masses[i] == null)
            {
                continue;
            }
            Rigidbody rb = masses[i].gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                masses[i] = null;
                Debug.Log("Can't calculate gravity for " + gameObject.name + " because it doesn't have a Rigidbody");
                continue;
            }
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rbs[i] = rb;
        }
    }

    public void Register(Gravity mass)
    {
        // @TODO: register new mass
    }

    public void Unregister(Gravity mass)
    {
        for (int i = 0; i < masses.Length; i++)
        {
            masses[i] = null;
            rbs[i] = null;
        }
    }

    private void Update()
    {
        if (lastGravityUpdateTime < Time.time - GravityUpdateFrequency)
        {
            RecalculateGravity();
            lastGravityUpdateTime = Time.time;
        }

        AttractMasses();
    }

    private void AttractMasses()
    {
        for (int i = 0; i < masses.Length; i++)
        {
            Vector3 velocity = masses[i].gravityVelocity;
            if (masses[i] == null || velocity == Vector3.zero)
            {
                continue;
            }

            rbs[i].AddForce(velocity * Time.deltaTime);
            Transform body = rbs[i].transform;

            if (masses[i].autoOrientDownwards)
            {
                Debug.Log(masses[i].gravityVelocity.sqrMagnitude);
                if (masses[i].gravityVelocity.sqrMagnitude > masses[i].autoOrientSqrMagThreshold)
                {
                    Quaternion targetRotation = Quaternion.FromToRotation(body.up, -velocity) * body.rotation;
                    body.rotation = Quaternion.Slerp(body.rotation, targetRotation, RotationAdjustmentSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void RecalculateGravity()
    {
        for (int i = 0; i < masses.Length; i++)
        {
            Gravity thisMass = masses[i];
            if (thisMass == null)
            {
                continue;
            }
            if (!thisMass.useGravity)
            {
                thisMass.gravityVelocity = Vector3.zero;
                continue;
            }

            Vector3 pos = thisMass.transform.position;
            Vector3 gravity = new Vector3();

            for (int j = 0; j < masses.Length; j++)
            {
                Gravity otherMass = masses[j];
                if (otherMass == null || otherMass == thisMass)
                {
                    continue;
                }

                Vector3 dir = otherMass.transform.position - pos;
                Vector3 change = dir.normalized * (rbs[j].mass * MassMultiplier / (1f + Vector3.SqrMagnitude(dir)));
                gravity += change;
            }
            thisMass.gravityVelocity = gravity;
        }
    }
}
