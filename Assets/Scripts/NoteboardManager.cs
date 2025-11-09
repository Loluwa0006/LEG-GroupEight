using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteboardManager : MonoBehaviour
{

    public const int NOTE_POOL_SIZE = 25;

    [SerializeField] int numberOfColumns = 4;
    [Header("Prefabs")]
    [SerializeField] NoteEntity notePrefab;
    [SerializeField] NoteClearer clearerPrefab;
    [SerializeField] GameObject noteboardObject;
    [Header("Spawn Points")]
    [SerializeField] Transform farSpawn;
    [SerializeField] Transform closeSpawn;
    [SerializeField] Transform clearerSpawn;
    [Header("UI")]
    [SerializeField] TMP_Text streakTracker;
    [SerializeField] TMP_Text scoreTracker;

    [SerializeField] Transform clearerRotator;
    [SerializeField] Transform staff;
    [SerializeField] Transform rotator;




    [SerializeField] float minCooldown = 0.1f;
    [SerializeField] float maxCooldown = 0.2f;

    float cooldownTracker = 0.0f;

    Queue<NoteEntity> noteList = new();

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
            Debug.Log($"Loop {i} in clearerspawner");
            NoteClearer newClearer = Instantiate(clearerPrefab, clearerRotator);
            newClearer.InitClearer(this, i);
            float lerpAmount = distanceBetweenColumns * i;
            Vector3 spawnPoint = Vector3.Lerp(farSpawn.position, closeSpawn.position, lerpAmount);
            //spawnPoint.z = clearerSpawn.position.z;
            newClearer.transform.position = spawnPoint;
        }

        clearerRotator.transform.Rotate(0, 170, 0);
    }

    private void Update()
    {
        if (cooldownTracker <= 0.0f)
        {
            cooldownTracker = Random.Range(minCooldown, maxCooldown);
            CreateNewNote();
        }
        cooldownTracker -= Time.deltaTime;
    }

    void CreateNewNote()
    {
        var newNote = noteList.Dequeue();
        newNote.gameObject.SetActive(true);
        int columnIndex = Random.Range(0, numberOfColumns);
        float distanceBetweenColumns = (1.0f / (numberOfColumns - 1));
        float lerpAmount = distanceBetweenColumns * columnIndex;
        Vector3 spawnPos = Vector3.Lerp(farSpawn.position, closeSpawn.position, lerpAmount);
        newNote.Drop(spawnPos);
        newNote.transform.SetLocalPositionAndRotation(newNote.transform.localPosition, Quaternion.Euler(0, rotator.localEulerAngles.y, 0));
    }

    public void OnNoteFailed(NoteEntity note)
    {
        UpdateStreak(true);
        ResetNote(note);
    }

    public void OnNoteSuccessful(NoteEntity note)
    {
        UpdateScore(note.value);
        UpdateStreak(false);
        ResetNote(note);
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

}
