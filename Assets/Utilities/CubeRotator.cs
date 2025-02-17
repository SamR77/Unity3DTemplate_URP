using UnityEngine;

public class CubeRotator : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // Rotation speed in degrees per second

    void Update()
    {
        // Rotate the object based on Time.deltaTime and Time.timeScale
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
