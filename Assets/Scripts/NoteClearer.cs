using UnityEngine;
using UnityEngine.InputSystem;

public class NoteClearer : MonoBehaviour
{

    [SerializeField] NoteboardManager manager;
    [SerializeField] Collider detector;
    [SerializeField] MeshRenderer mesh;
    [SerializeField] ParticleSystem perfectPlayParticles;
    [SerializeField] PlayerInput playerInput;

    InputAction button;

    //KeyCode button = KeyCode.Escape;     //From old code

    //Using ints to measure by frame for precision
    [SerializeField] int perfectDuration = 8;
    [SerializeField] int perfectCooldown = 45; // to prevent mashing

    int perfectPlayTracker = 0;
    int cooldownTracker = 0;
    int index = 0;

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

        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
        var main = perfectPlayParticles.main;

        main.loop = false;

        perfectPlayParticles.Stop();
    }

    public void InitClearer(NoteboardManager manager, int index)
    {
        this.manager = manager;
        this.index = index;

        switch (index)
        {
            case 0:
                button = playerInput.actions["NoteOne"];
                break;
            case 1:
                button = playerInput.actions["NoteTwo"];
                break;
            case 2:
                button = playerInput.actions["NoteThree"];
                break;
            case 3:
                button = playerInput.actions["NoteFour"];
                break;
            case 4:
                button = playerInput.actions["NoteFive"];
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
        if (button.WasPerformedThisFrame())
        {
            OnButtonDown();
        }
        if (button.WasReleasedThisFrame())
        {
            OnButtonReleased();
        }

        if (index == 0) //only one clearer should do this, otherwise we call the function up to 3 extra times for no reason
        {
            if (playerInput.actions["StrumLeft"].WasPerformedThisFrame())
            {
                manager.StrumLeft();
            }
            else if (playerInput.actions["StrumRight"].WasPerformedThisFrame())
            {
                manager.StrumRight();
            }
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
