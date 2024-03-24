using UnityEngine;

public class TargetRandomMovement : MonoBehaviour
{
    public float speed = 5.0f;
    private Vector3 targetPosition;

    private void Start()
    {
        SetNewRandomPosition();
    }

    private void Update()
    {
        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewRandomPosition();
        }
    }

    void SetNewRandomPosition()
    {
        float screenX = Random.Range(0, Screen.width);
        float screenY = Random.Range(0, Screen.height);

        Vector3 screenPosition = new Vector3(screenX, screenY, 10);
        targetPosition = Camera.main.ScreenToWorldPoint(screenPosition);
    }
}
