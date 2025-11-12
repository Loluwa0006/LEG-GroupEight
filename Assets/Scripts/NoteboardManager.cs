using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class NoteboardManager : MonoBehaviour
{

    public const int NOTE_POOL_SIZE = 40;

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

    [SerializeField] Transform clearerRotator;
    [SerializeField] Transform staff;
    [SerializeField] Transform rotator;



    [Header("Default Note")]
    [SerializeField] float defaultNoteMinCooldown = 0.1f;
    [SerializeField] float defaultNoteMaxCooldown = 0.2f;

    [Header("False Note")]
    [SerializeField] float falseNoteMinCooldown = 0.6f;
    [SerializeField] float falseNoteMaxCooldown = 0.7f;

    /*
    float baseNotecooldownTracker = 0.0f;
    float falseNoteCooldownTracker = 0.0f;
    */

    [SerializeField] float audioStartDelayInSeconds;
    [SerializeField] double bpm;
    [SerializeField] bool Song1 = true;
    Queue<float> noteValues;
    Queue<float> noteValuesInQuarterNotesSong1 = new Queue<float>(new[] { 
        1f, 0.5f, 0.5f, 1, 1,  
        1, 1, 1, 1, 
        1, 1, 1, 0.5f, 0.5f, 
        0.5f, 0.5f, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 1, 1, 0.5f, 0.5f, 
        1, 1, 1, 1, 
        1, 0.5f, 0.5f, 0.5f, 0.5f, 1, 
        1, 1, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 1, 1, 1, 
        1, 1, 0.5f, 0.5f, 1, 
        1, 1, 1, 1, 
        1, 1, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 1, 1, 0.5f, 0.5f, 
        0.5f, 1, 0.5f, 1, 
        1, 1, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 0.5f, 1, 0.5f, 1, 
        1, 1, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 1, 0.5f, 0.5f, 0.5f, 0.5f, 
        1, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 
        1, 0.5f, 0.5f, 0.5f, 1, 0.5f,  // 0:41
        1, 0.5f, 0.5f, 1, 0.5f, 0.5f, 
        1, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 
        1, 0.5f, 0.5f, 1, 0.5f, 0.5f, 
        1, 0.5f, 1, 0.5f, 0.5f, 0.5f, 
        1, 0.5f, 0.5f, 1, 0.5f, 0.5f, 
        1, 1, 0.5f, 0.5f, 0.5f, 1, 
        0.5f, 1, 1, 0.5f, 1, 
        0.5f, 0.5f, 1, 0.5f, 1, 1, 
        1, 1, 0.5f, 0.5f, 1, 
        1, 0.5f, 0.5f, 1, 0.5f, 0.5f, 
        0.5f, 0.5f, 1, 0.5f, 0.5f, 0.5f, 0.5f, 
        1, 0.5f, 0.5f, 0.5f, 1, 0.5f, 
        0.5f, 0.5f, 1, 0.5f, 0.5f, 1, 
        1, 0.5f, 1, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 1, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 1, 0.5f, 1, 1,  // 1:07
        1, 0.5f, 0.5f, 0.5f, 0.5f, 1, 
        1, 0.5f, 0.5f, 1, 0.5f, 0.5f,   // 1:12 second last 0.5 cymbals
        0.5f, 0.5f, 1, 0.5f, 0.5f, 1, 
        1, 0.5f, 0.5f, 0.5f, 1, 0.5f, 
        0.5f, 0.5f, 1, 0.5f, 0.5f, 1, 
        1, 0.5f, 0.5f, 1, 0.5f, 0.5f, 
        0.5f, 0.5f, 1, 1, 0.5f, 0.5f, 
        0.5f, 1, 0.5f, 1, 1, // 1:21
        1, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 1, 
        1, 1, 1, 1, 
        1, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 1,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 0.5f, 1, 0.5f, 1, // 1:35
        0.5f, 0.5f, 1, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 
        0.5f, 0.5f, 1, 0.5f, 0.5f, 1, 
        1, 0.5f, 0.5f, 0.5f, 0.5f, 1,
        0.5f, 0.5f, 1, 0.5f, 0.5f, 0.5f, 0.5f, 
        1, 0.5f, 0.5f, 0.5f, 1, 0.5f, 1,
        1, 0.5f, 0.5f, 1
    });

    Queue<float> noteValuesInQuarterNotesSong2 = new Queue<float>(new[] {
        1f, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 0.5f, 0.5f, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 0.5f, 0.5f, 
        4,
        4,
        4, 
        4, 
        4, 
        4, 
        4, 
        4, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 0.5f, 0.5f, 1, 1, 
        0.5f, 0.5f, 1, 1, 0.5f, 0.5f, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 0.5f, 0.5f, 0.5f, 0.5f, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f,
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1, 
        1, 1, 1, 1,
        1, 1, 1, 1,
        1, 1, 1, 1,
        1, 1, 1, 1,
        0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f
    });
    Queue<NoteEntity> noteList = new();
    List<FalseNoteEntity> activeFalseNotes = new();
    double spawnCooldown = 0f;
    AudioSource audioSource;

    int score = 0;
    int streak = 0;

    private void Awake()
    {
        for (int i = 0; i < NOTE_POOL_SIZE; i++)
        {
            NoteEntity newNote = Instantiate(notePrefab, staff);
            noteList.Enqueue(newNote);
            newNote.InitNote(this);
            newNote.gameObject.SetActive(false);
        }

        float distanceBetweenColumns = (1.0f / (numberOfColumns - 1));

        for (int i = 0; i < numberOfColumns; i++)
        {
            NoteClearer newClearer = Instantiate(clearerPrefab, clearerRotator);
            newClearer.InitClearer(this, i);
            float lerpAmount = distanceBetweenColumns * i;
            Vector3 spawnPoint = Vector3.Lerp(farSpawn.position, closeSpawn.position, lerpAmount);
            newClearer.transform.position = spawnPoint;
        }

        clearerRotator.transform.Rotate(0, 170, 0);

        audioSource = GetComponent<AudioSource>();

        StartCoroutine(audioStartDelay());

        if (Song1)
        {
            noteValues = noteValuesInQuarterNotesSong1;
        }
        else
        {
            noteValues = noteValuesInQuarterNotesSong2;
        }
    }

    private void Update()
    {
        /*
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
        */

        
        spawnCooldown -= Time.deltaTime;

        if (spawnCooldown <= 0f)
        {
            if (noteValues.Count > 0)
            {
                spawnCooldown = 60 / bpm * noteValues.Dequeue();
                GenerateRandomNote();
            }
            else
            {
                // go to main menu /////////////////////////////
            }
        }
    }

    IEnumerator audioStartDelay()
    {
        yield return new WaitForSeconds(audioStartDelayInSeconds);
        audioSource.Play();
    }

    void GenerateRandomNote()
    {
        int tempNum = Random.Range(0,10);
        if (tempNum <= 8)
        {
            CreateNewNote();
        }
        else
        {
            CreateNewFalseNote();
        }
    }

    void CreateNewNote()
    {
        var newNote = noteList.Dequeue();
        newNote.gameObject.SetActive(true);
        int columnIndex = Random.Range(0, numberOfColumns);
        int columnCount = numberOfColumns - 1;
        if (columnCount == 0) columnCount = 1; //can't divide by 0
        float distanceBetweenColumns = (1.0f / columnCount);
        float lerpAmount = distanceBetweenColumns * columnIndex;
        Vector3 spawnPos = Vector3.Lerp(farSpawn.position, closeSpawn.position, lerpAmount);
        newNote.Drop(spawnPos);
        newNote.transform.SetLocalPositionAndRotation(newNote.transform.localPosition, Quaternion.Euler(0, rotator.localEulerAngles.y, 0));
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
        FalseNoteEntity falseNote = Instantiate(falseNotePrefab, staff);
        falseNote.transform.position = spawnPos;
        falseNote.Drop(spawnPos);
        activeFalseNotes.Add(falseNote);
        falseNote.strumDirection = strumDir;
        falseNote.transform.SetLocalPositionAndRotation(falseNote.transform.localPosition, Quaternion.Euler(0, rotator.localEulerAngles.y, 0));
    }

    public void OnNoteFailed(NoteEntity note)
    {
        UpdateStreak(true);
        uiManager.OnNoteMissed(note);
        if (note.TryGetComponent(out FalseNoteEntity falseNote))
        {
            if (activeFalseNotes.Contains(falseNote))
            {
                activeFalseNotes.Remove(falseNote);
                Destroy(falseNote.gameObject);
            }
        }
        else
        {
            ResetNote(note);
        }
    }

    public void OnNoteSuccessful(NoteEntity note, bool isPerfect)
    {
        UpdateScore(note.value);
        UpdateStreak(false);
        uiManager.OnNotePlayed(note, isPerfect);


        if (note.TryGetComponent(out FalseNoteEntity falseNote))
        {
            if (activeFalseNotes.Contains(falseNote))
            {
                activeFalseNotes.Remove(falseNote);
                Destroy(falseNote.gameObject);
            }
        }
        else
        {
            ResetNote(note);
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
            if (falseNote.strumDirection == -1)
            {
                Vector3 newPos = falseNote.transform.localPosition;
                newPos.y = farSpawn.transform.localPosition.y + rotator.transform.localPosition.y;
                falseNote.transform.localPosition = newPos;
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
                Vector3 newPos = falseNote.transform.localPosition;
                newPos.y = closeSpawn.transform.localPosition.y + rotator.transform.localPosition.y;
                falseNote.transform.localPosition = newPos;
            }
        }
    }

}
