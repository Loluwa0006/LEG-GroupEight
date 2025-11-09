using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NoteboardManager : MonoBehaviour
{

    public const int NOTE_POOL_SIZE = 25;


    [SerializeField] int numberOfColumns = 4;
    [Header("Prefabs")]
    [SerializeField] NoteEntity notePrefab;
    [SerializeField] FalseNoteEntity falseNotePrefab;
    [SerializeField] NoteClearer clearerPrefab;
    [SerializeField] GameObject noteboardObject;
    [Header("Spawn Points")]
    [SerializeField] Transform farSpawn;
    [SerializeField] Transform closeSpawn;
    [SerializeField] Transform clearerSpawn;
    [Header("UI")]
    [SerializeField] TMP_Text streakTracker;
    [SerializeField] TMP_Text scoreTracker;
    [SerializeField] UIManager uiManager;



    [Header("Default Note")]
    [SerializeField] float defaultNoteMinCooldown = 0.1f;
    [SerializeField] float defaultNoteMaxCooldown = 0.2f;

    [Header("False Note")]
    [SerializeField] float falseNoteMinCooldown = 0.6f;
    [SerializeField] float falseNoteMaxCooldown = 0.7f;

    float baseNotecooldownTracker = 0.0f;
    float falseNoteCooldownTracker = 0.0f;

    Queue<NoteEntity> noteList = new();
    List<FalseNoteEntity> activeFalseNotes = new();

    int score = 0;
    int streak = 0;
    private void Awake()
    {
        for (int i = 0; i < NOTE_POOL_SIZE; i++)
        {
            NoteEntity newNote = Instantiate(notePrefab);
            noteList.Enqueue(newNote);
            newNote.InitNote(this);
            newNote.gameObject.SetActive(false);
        }
        float distanceBetweenColumns = (1.0f / (numberOfColumns - 1));
        for (int i = 0; i < numberOfColumns;i++)
        {
            NoteClearer newClearer = Instantiate(clearerPrefab);
            newClearer.InitClearer(this, i);   
            float lerpAmount = distanceBetweenColumns * i;
            Vector3 spawnPoint = Vector3.Lerp(farSpawn.position, closeSpawn.position, lerpAmount);
            spawnPoint.z = clearerSpawn.position.z;
            newClearer.transform.position = spawnPoint;
        }
    }

    private void Update()
    {
        if (baseNotecooldownTracker <= 0.0f)
        {
            baseNotecooldownTracker = Random.Range(defaultNoteMinCooldown, defaultNoteMaxCooldown);
            CreateNewNote();
        }
        if (falseNoteCooldownTracker <= 0.0f)
        {
            falseNoteCooldownTracker = Random.Range(falseNoteMinCooldown, falseNoteMaxCooldown);
            CreateNewFalseNote();
            Debug.Log("making false note");
        }

        baseNotecooldownTracker -= Time.deltaTime;
        falseNoteCooldownTracker -= Time.deltaTime;
    }

    void CreateNewNote()
    {
        var newNote = noteList.Dequeue();
        newNote.gameObject.SetActive(true);
        int columnIndex = Random.Range(0, numberOfColumns );
        int columnCount = numberOfColumns - 1;
        if (columnCount == 0) columnCount = 1; //can't divide by 0
        float distanceBetweenColumns = (1.0f / columnCount);
        float lerpAmount = distanceBetweenColumns * columnIndex; 
        Vector3 spawnPos = Vector3.Lerp(farSpawn.position, closeSpawn.position, lerpAmount);
        newNote.Drop(spawnPos);
    }

    void CreateNewFalseNote()
    {
        int columnCount = numberOfColumns - 1;
        if (columnCount == 0) columnCount = 1; //can't divide by 0
        float distanceBetweenColumns = (1.0f / columnCount);
        int dir = Random.Range(0, 2);
        Vector3 spawnPos;
        int strumDir;
        if (dir == 1)
        {
            spawnPos = Vector3.LerpUnclamped(farSpawn.position, closeSpawn.position, -0.25f);
            strumDir = -1;
        }
        else
        {
            spawnPos = Vector3.LerpUnclamped(farSpawn.position, closeSpawn.position, 1.25f);
            strumDir = 1;

        }
        FalseNoteEntity falseNote = Instantiate(falseNotePrefab);
        falseNote.transform.position = spawnPos;
        falseNote.Drop(spawnPos);
        activeFalseNotes.Add(falseNote);
        falseNote.strumDirection = strumDir;

    }

public void OnNoteFailed(NoteEntity note)
    {
        UpdateStreak(true);
        ResetNote(note);
        uiManager.OnNoteMissed(note);
        if (note.TryGetComponent(out FalseNoteEntity falseNote))
        {
            if (activeFalseNotes.Contains(falseNote))
            {
                activeFalseNotes.Remove(falseNote);
                Destroy(falseNote.gameObject);
            }
        }
    }

    public void OnNoteSuccessful(NoteEntity note, bool isPerfect)
    {
        UpdateScore(note.value);
        UpdateStreak(false);
        ResetNote(note);
        uiManager.OnNotePlayed(note, isPerfect);

        
        if (note.TryGetComponent(out FalseNoteEntity falseNote))
        {
            if (activeFalseNotes.Contains(falseNote))
            {
                activeFalseNotes.Remove(falseNote);
                Destroy(falseNote.gameObject);
            }
        }
    }

    void ResetNote(NoteEntity note)
    {
        note.rb.linearVelocity = Vector3.zero;
        noteList.Enqueue(note);
        note.gameObject.SetActive(false);
    }

    public void UpdateStreak(bool reset)
    {
        if (reset)
        {
            streak = 0;
        }
        else
        {
            streak += 1;
        }
        streakTracker.text = "Streak: " + streak.ToString();
    }

    public void UpdateScore(int amount)
    {
        score += 1;
        scoreTracker.text = "Score: " + score.ToString();
    }

   
    public void StrumLeft()
    {
        Debug.Log("Strumming Left");
      
        foreach (var falseNote in activeFalseNotes)
        {
            if (falseNote. strumDirection == -1)
            {
                Vector3 newPos = falseNote.transform.position;
                newPos.x = farSpawn.transform.position.x;
                falseNote.transform.position = newPos;
            }
        }
    }

    public void StrumRight()
    {
        Debug.Log("Strumming Right");
       
        foreach (var falseNote in activeFalseNotes)
        {
            if (falseNote.strumDirection == 1)
            {
                Vector3 newPos = falseNote.transform.position;
                newPos.x = closeSpawn.transform.position.x;
                falseNote.transform.position = newPos;
            }
        }
    }

}
