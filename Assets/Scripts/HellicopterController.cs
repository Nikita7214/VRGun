using UnityEngine;
using System.Collections;

public class HelicopterController : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 90f;
    public float radius = 20f;
    public float angleOffset = 0f;
    public float angleIncrement = 45f;
    public float flightDuration = 60f; // Длительность полного цикла полета (60 секунд)

    private bool isFlying = false;
    private bool isReturning = false;
    private float flightTimer = 0f;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        StartCoroutine(StartFlight());
    }

    void Update()
    {
        if (isFlying)
        {
            MoveAlongTrajectory();
            flightTimer += Time.deltaTime;

            if (flightTimer >= flightDuration / 2f)
            {
                isFlying = false;
                isReturning = true;
                flightTimer = 0f;
            }
        }
        else if (isReturning)
        {
            MoveBackToStart();
            flightTimer += Time.deltaTime;

            if (flightTimer >= flightDuration / 2f)
            {
                isReturning = false;
                flightTimer = 0f;
                StartCoroutine(StartFlight());
            }
        }
    }

    void MoveAlongTrajectory()
    {
        Vector3 center = transform.position;
        center.y = transform.position.y; // Сохраняем высоту
        float angle = angleOffset + flightTimer / (flightDuration / 2f) * 360f;
        float x = center.x + radius * Mathf.Cos(Mathf.Deg2Rad * angle);
        float z = center.z + radius * Mathf.Sin(Mathf.Deg2Rad * angle);
        Vector3 targetPosition = new Vector3(x, center.y, z);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
    }

    void MoveBackToStart()
    {
        transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(startPosition - transform.position);
    }

    IEnumerator StartFlight()
    {
        yield return new WaitForSeconds(2f); // Задержка перед началом полета
        isFlying = true;
    }
}