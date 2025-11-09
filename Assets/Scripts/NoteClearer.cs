using UnityEngine;

public class NoteClearer : MonoBehaviour
{

    [SerializeField] NoteboardManager manager;
    [SerializeField] Collider detector;
    [SerializeField] MeshRenderer mesh;

    KeyCode button = KeyCode.Escape;

    private void Awake()
    {
        if (detector == null)
        {
            detector = GetComponent<Collider>();
        }

        if (mesh == null)
        {
            mesh = GetComponent<MeshRenderer>();
        }
  
    }
    public void InitClearer(NoteboardManager manager, int index)
    {
        this.manager = manager;

        switch (index)
        {
            case 0:
                button = KeyCode.G;
                break;
            case 1:
                button = KeyCode.F;
                break;
            case 2:
                button = KeyCode.D;
                break;
            case 3:
                button = KeyCode.S;
                break;
            case 4:
                button = KeyCode.A;
                break;
            default:
                Debug.Log("Don't have button for index " + index);
                Destroy(gameObject);
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NoteEntity note))
        {
            manager.OnNoteFailed(note);
        }
    }

    private void Update()
    {
        if (button != KeyCode.Escape)
        {
            detector.enabled = !Input.GetKey(button);
            mesh.enabled = detector.enabled;
        }
    }
}
