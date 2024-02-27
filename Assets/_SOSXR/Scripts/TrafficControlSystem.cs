using UnityEngine;
using UnityStandardAssets.Utility;


[RequireComponent(typeof(WaypointProgressTracker))]
[RequireComponent(typeof(AICar))]
public class TrafficControlSystem : MonoBehaviour
{
    [SerializeField] private Renderer m_paintRenderer;
    [SerializeField] private Color m_randomColor = Color.red;
    [SerializeField] private Transform m_rayStartPoint;
    [Range(0, 50)] [SerializeField] private float m_rayLength = 10.0f; // Length of the ray
    [SerializeField] private string m_targetTag = "ManualCar"; // Tag of the collider you are looking for

    private AICar m_aiCar;
    private WaypointProgressTracker m_tracker;

    private RaycastHit m_hit;
    [SerializeField] [Range(0.1f, 5f)] private float m_sphereRadius = 1f;


    private void Awake()
    {
        m_aiCar = GetComponent<AICar>();
        m_tracker = GetComponent<WaypointProgressTracker>();

        if (m_paintRenderer == null)
        {
            m_paintRenderer = GetComponentInChildren<Renderer>();
        }

        if (m_rayStartPoint == null)
        {
            m_rayStartPoint = transform;
        }
    }


    private void Start()
    {
        ActivatWPT();
        m_tracker.Init(m_tracker.Circuit);

        m_randomColor = Random.ColorHSV();
        m_paintRenderer.sharedMaterial.color = m_randomColor;
    }


    public void ActivatWPT()
    {
        m_aiCar.enabled = true;
        m_tracker.enabled = true;

        foreach (var waypoint in m_tracker.Circuit.Waypoints)
        {
            var speedSettings = waypoint.GetComponent<SpeedSettings>();

            if (speedSettings != null)
            {
                speedSettings.targetAICar = m_aiCar;
            }
        }
    }


    private void FixedUpdate()
    {
        if (Physics.SphereCast(m_rayStartPoint.position, m_sphereRadius, transform.forward, out m_hit, m_rayLength))
        {
            // Check if the collider has the correct tag
            if (m_hit.collider.CompareTag(m_targetTag))
            {
                //Debug.Log("Hit a target with tag " + m_targetTag + ". Distance: " + m_hit.distance);
                // RecalcSpeed(m_hit.distance);

                var otherCar = m_hit.transform.gameObject.GetComponent<AICar>();

                if (otherCar != null)
                {
                    AdjustSpeedBasedOnFrontCar(m_hit.distance, otherCar);
                }
            }
            else
            {
                m_hit = new RaycastHit();
                m_aiCar.acceleration = Defaults.Acceleration;
            }
        }
        else
        {
            m_hit = new RaycastHit();
            m_aiCar.acceleration = Defaults.Acceleration;
        }
    }


    private void AdjustSpeedBasedOnFrontCar(float hitDistance, AICar otherCar)
    {
        if (hitDistance < 10)
        {
            Debug.LogWarning("TEN METERS");
            m_aiCar.acceleration = -5;
            m_aiCar.speed = 0;
        }
        else if (hitDistance is >= 10 and < 25)
        {
            Debug.LogWarning("10-25 METERS");
            m_aiCar.acceleration = -2;
            m_aiCar.speed = (float) (otherCar.speed * 0.95);
        }
        else
        {
            Debug.LogWarning("25+");
            m_aiCar.acceleration = -1;
            m_aiCar.speed = otherCar.speed;
        }
    }


    private void RecalcSpeed(float hitDistance)
    {
        if (hitDistance < 2)
        {
            m_aiCar.acceleration = -5;
            m_aiCar.speed = 0;
        }

        if (hitDistance is >= 2 and < 10)
        {
            m_aiCar.acceleration = -2;
            m_aiCar.speed = 10;
        }
        else
        {
            m_aiCar.acceleration = -1;
            m_aiCar.speed = 20;
        }
    }


    // Draw a line in the Scene view for the ray
    private void OnDrawGizmos()
    {
        if (m_hit.transform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(m_hit.transform.position, m_sphereRadius);
        }
        else
        {
            Gizmos.color = Color.red;
        }

        var direction = m_rayStartPoint.TransformDirection(Vector3.forward) * m_rayLength; // Calculate the direction of the ray
        Gizmos.DrawRay(m_rayStartPoint.position, direction); // Draw the ray in the Scene view
    }
}