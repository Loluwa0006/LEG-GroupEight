using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float rotateSpeed;

    /*
    private void FixedUpdate()
    {
        this.transform.Rotate(0, -rotateSpeed, 0);
    }
    */

    private void Update()
    {
        this.transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
    }
}
