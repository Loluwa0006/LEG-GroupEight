using UnityEngine;

public class NoteEntity : MonoBehaviour
{
    public Rigidbody rb;
    public int value = 1;

    [SerializeField] float fallSpeed = 1.0f;

    private void Awake()
    {
        fallSpeed = Mathf.Abs(fallSpeed) * -1;
    }
    public void InitNote(NoteboardManager manager)
    {
        //for future reference
    }

    public void Drop(Vector3 startingPoint)
    {
        transform.position = startingPoint;
        //rb.linearVelocity = new Vector3(0, 0, fallSpeed); // only for the flat scene
    }
}
