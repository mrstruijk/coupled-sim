using System;
using UnityEngine;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;


[RequireComponent(typeof(WaypointProgressTracker))]
[RequireComponent(typeof(AICar))]
public class TrafficControlSystem : MonoBehaviour
{
    [SerializeField] private Renderer m_paintRenderer;
    [SerializeField] private Color m_randomColor = Color.red;
    [SerializeField] private Transform m_rayStartPoint;
    [Range(0, 50)] [SerializeField] private float m_rayLength = 10.0f;
    [SerializeField] private string m_targetTag = "ManualCar";
    [SerializeField] [Range(0.1f, 5f)] private float m_sphereRadius = 1f;
    [SerializeField] private LayerMask m_layerMask; // Set to Car
    [SerializeField] private Vector2 m_closeMedium = new(2, 10);
    private AICar m_aiCar;
    private WaypointProgressTracker m_tracker;
    private float _desiredSpeed;
    private float _desiredAcceleration;
    [SerializeField] private float m_lerpSpeed = 10f;
    private RaycastHit m_hit;


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
        //   CheckForOtherCarInFront();
    }


    private void CheckForOtherCarInFront()
    {
        if (Physics.SphereCast(m_rayStartPoint.position, m_sphereRadius, transform.forward, out m_hit, m_rayLength, m_layerMask))
        {
            if (m_hit.collider.CompareTag(m_targetTag))
            {
                Debug.LogFormat("Hit a target {0}", m_hit.transform.name);

                var otherCar = m_hit.transform.root.gameObject.GetComponentInChildren<AICar>();

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
        if (hitDistance < m_closeMedium.x)
        {
            Debug.LogFormat("Less than {0}", m_closeMedium.x);
            _desiredAcceleration= -1;
            _desiredAcceleration = 0;
        }
        else if (hitDistance >= m_closeMedium.x && hitDistance < m_closeMedium.y)
        {
            Debug.LogFormat("More than {0} and less than {1}", m_closeMedium.x, m_closeMedium.y);

            _desiredAcceleration = -1;

            if (otherCar.speed != 0)
            {
                _desiredSpeed = (float) (otherCar.speed * 0.9);
            }
            else
            {
                _desiredSpeed = 5;
            }
        }
        else
        {
            Debug.LogFormat("More than {0}", m_closeMedium.y);

            if (otherCar.speed != 0)
            {
                _desiredAcceleration = otherCar.acceleration * 0.9f;
                _desiredSpeed = otherCar.speed;
            }
            else
            {
                _desiredAcceleration = -1f;
                _desiredSpeed = 15;
            }
        }

        m_aiCar.speed = Mathf.Lerp(m_aiCar.speed, _desiredSpeed, Time.deltaTime * m_lerpSpeed);
        m_aiCar.acceleration = _desiredAcceleration;
    }
    

    // Draw a line in the Scene view for the ray
    private void OnDrawGizmos()
    {
        Vector3 direction;

        if (m_hit.transform != null)
        {
            Gizmos.color = Color.green;
            direction = m_rayStartPoint.TransformDirection(Vector3.forward) * m_hit.distance;
            Gizmos.DrawSphere(m_hit.transform.position, m_sphereRadius);
        }
        else
        {
            Gizmos.color = Color.red;
            direction = m_rayStartPoint.TransformDirection(Vector3.forward) * m_rayLength;
        }

        Gizmos.DrawRay(m_rayStartPoint.position, direction);
    }
}