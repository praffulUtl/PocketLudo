using UnityEngine;

public class LudoCamera : MonoBehaviour
{
    private Vector3 desiredPosition;

    [SerializeField] private float scrollSpeedX = 0.25f;
    [SerializeField] private float scrollSpeedY = 0.25f;
    [SerializeField] private Vector2 boundX = new Vector2(-10, 10);
    [SerializeField] private Vector2 boundY = new Vector2(-10, 10);

    private void Start()
    {
        desiredPosition = transform.position;
    }

    private void Update()
    {
        Vector3 pos = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        desiredPosition += (pos * scrollSpeedX) * Time.deltaTime;

        if (desiredPosition.x <= boundX[0] || desiredPosition.x >= boundX[1])
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, boundX[0], boundX[1]);
        if (desiredPosition.z <= boundY[0] || desiredPosition.z >= boundY[1])
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, boundY[0], boundY[1]);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.25f);
    }
}
