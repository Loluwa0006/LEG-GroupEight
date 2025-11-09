using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] int rotateSpeed;

    private void FixedUpdate()
    {
        this.transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
    }
}
