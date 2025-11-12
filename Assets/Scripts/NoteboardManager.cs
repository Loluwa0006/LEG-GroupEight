using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;


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
    [SerializeField] UIManager uiManager;

    [SerializeField] Transform clearerRotator;
    [SerializeField] Transform staff;
    [SerializeField] Transform rotator;


    [SerializeField] float audioStartDelayInSeconds;
    [SerializeField] float bpm;
    Queue<float> noteValuesInQuarterNotes = new Queue<float>(new[] { 1f, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 2 });
    Queue<NoteEntity> noteList = new();
    List<FalseNoteEntity> activeFalseNotes = new();
    float spawnCooldown = 0f;
    AudioSource audioSource;



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
            Debug.Log($"Loop {i} in clearerspawner");
            NoteClearer newClearer = Instantiate(clearerPrefab, clearerRotator);
            newClearer.InitClearer(this, i);
            float lerpAmount = distanceBetweenColumns * i;
            Vector3 spawnPoint = Vector3.Lerp(farSpawn.position, closeSpawn.position, lerpAmount);
            newClearer.transform.position = spawnPoint;
            newClearer.onFailedPlay.AddListener(uiManager.OnStreakLost);
        }

        clearerRotator.transform.Rotate(0, 170, 0);

        audioSource = GetComponent<AudioSource>();

        StartCoroutine(audioStartDelay());
    }

    private void Update()
    {
        spawnCooldown -= Time.deltaTime;

        if (spawnCooldown <= 0f)
        {
            if (noteValuesInQuarterNotes.Count > 0)
            {
                CreateNewNote();
                spawnCooldown = 60 / bpm * noteValuesInQuarterNotes.Dequeue();
            }
        }
    }

    IEnumerator audioStartDelay()
    {
        yield return new WaitForSeconds(audioStartDelayInSeconds);
        audioSource.Play();
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
    public void OnNoteFailed(NoteEntity note)
    {
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
