using UnityEngine;

public class NoteClearer : MonoBehaviour
{

    [SerializeField] NoteboardManager manager;
    [SerializeField] Collider detector;
    [SerializeField] MeshRenderer mesh;
    [SerializeField] ParticleSystem perfectPlayParticles;

    KeyCode button = KeyCode.Escape;


    //Using ints to measure by frame for precision
    [SerializeField] int perfectDuration = 8; 
    [SerializeField] int perfectCooldown = 45; // to prevent mashing

    int perfectPlayTracker = 0;
    int cooldownTracker = 0;

    bool playing = false;
    bool playedNotePerfectly = false;
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

        var main = perfectPlayParticles.main;
            
        main.loop = false;

        perfectPlayParticles.Stop();
  
    }
    public void InitClearer(NoteboardManager manager, int index)
    {
        this.manager = manager;

        switch (index)
        {
            case 0:
                button = KeyCode.F;
                break;
            case 1:
                button = KeyCode.D;
                break;
            case 2:
                button = KeyCode.S;
                break;
            case 3:
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
            if (!playing)
            {
                manager.OnNoteFailed(note);
                return;
            }
            else if (PlayedPerfect() && !PerfectPlayOnCooldown())
            {
                Debug.Log("Played note perfectly");
                cooldownTracker = 0;
                playedNotePerfectly = true;
                perfectPlayParticles.Play();
                manager.OnNoteSuccessful(note, true);

            }
            else
            {
                Debug.Log("Played note normally");
                manager.OnNoteSuccessful(note, false);

            }
        }
    }

    private void Update()
    {
       if (Input.GetKeyDown(button))
        {
            OnButtonDown();
        }
       if (Input.GetKeyUp(button))
        {
            OnButtonReleased();
        }
    }

    private void FixedUpdate()
    {
        if (perfectPlayTracker > 0)
        {
            perfectPlayTracker--;
        }
        if (cooldownTracker > 0)
        {
            cooldownTracker--;
        }

        if (button == KeyCode.A)
        {
            Debug.Log("Perfect play tracker is " + perfectPlayTracker);
            Debug.Log("Cooldown tracker is " + cooldownTracker);
        }
    }

    void OnButtonDown()
    {
        playing = true;
        mesh.enabled = false;
        perfectPlayTracker = perfectDuration;
    }
    void OnButtonReleased()
    {
        mesh.enabled = true;
        playing = false;
        if (!playedNotePerfectly) cooldownTracker = perfectCooldown;
        playedNotePerfectly = false;
    }
    public bool PlayedPerfect()
    {
        return perfectPlayTracker > 0;
    }
    public bool PerfectPlayOnCooldown()
    {
        return cooldownTracker > 0;
    }
}
