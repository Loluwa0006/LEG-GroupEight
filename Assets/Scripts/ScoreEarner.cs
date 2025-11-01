using UnityEngine;

public class ScoreEarner : MonoBehaviour
{
    [SerializeField] NoteboardManager manager;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out NoteEntity note)) { return; }
        manager.OnNoteSuccessful(note);
    }
}
