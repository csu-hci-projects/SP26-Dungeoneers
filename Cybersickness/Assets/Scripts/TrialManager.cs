using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using TMPro;

public class TrialManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject startWall;
    public Renderer tunnel1Renderer;
    public Material colorChangingShader1;

    [Header("Between Trial Walls")]
    public GameObject trialWall2;
    public GameObject trialWall3;
    public Renderer tunnel2Renderer;
    public Renderer tunnel3Renderer;
    public Material colorChangingShader2;
    public Material colorChangingShader3;

    [Header("HUD Text")]
    public TextMeshPro instructionText;
    public TextMeshPro trialNumberText;
    public TextMeshPro statusText;
    public TextMeshPro completionText;

    [Header("Trials")]
    public LockCylinder[] cylinders;

    private string participantID;
    private int currentTrial = 0;
    private List<string> csvRows = new List<string>();
    private float studyStartTime;
    private float trialStartTime;
    private float unlockTime = -1f;
    private float reachedTime = -1f;
    private float lockTime = -1f;
    private int unlockCount = 0;
    private int lockAttempts = 0;

    void Start()
    {
        participantID = Guid.NewGuid().ToString();
        Debug.Log("Participant ID: " + participantID);

        startWall.SetActive(true);
        trialWall2.SetActive(true);
        trialWall3.SetActive(true);

        instructionText.text = "<color=white><b>Study starting soon...</b></color>";
        trialNumberText.text = "<color=white><b>Sensory Overload Study</b></color>";
        statusText.text = "";
        completionText.text = "";

        csvRows.Add("ParticipantID,Trial,TrialStartTime,TimeToUnlock,TimeTo180,TimeToLock,TotalTrialTime,UnlockAttempts,LockAttempts,Success");

        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        int countdown = 5;
        while (countdown > 0)
        {
            instructionText.text = "<color=white><b>Study begins in: " + countdown + "</b></color>";
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        StartStudy();
    }

    public void StartStudy()
    {
        studyStartTime = Time.time;
        startWall.SetActive(false);
        tunnel1Renderer.material = colorChangingShader1;
        instructionText.text = "<color=white><b>Walk forward to begin</b></color>";
        StartTrial(0);
    }

    public void StartTrial(int trialIndex)
    {
        currentTrial = trialIndex;
        unlockTime = -1f;
        reachedTime = -1f;
        lockTime = -1f;
        unlockCount = 0;
        lockAttempts = 0;

        cylinders[currentTrial].SetTrialManager(this);
        StartCoroutine(StartTrialDelayed());
    }

    IEnumerator StartTrialDelayed()
    {
        yield return new WaitForSeconds(3f);

        // Start timing only here
        trialStartTime = Time.time;

        trialNumberText.text = "<color=white><b>Trial: " + (currentTrial + 1) + " / " + cylinders.Length + "</b></color>";
        instructionText.text = "<color=#00FFFF><b>When close to dial Say 'Unlock' to begin interaction</b></color>";
        statusText.text = "";
        Debug.Log("Trial " + (currentTrial + 1) + " started");
    }

    public void OnUnlock()
    {
        unlockCount++;
        if (unlockTime < 0)
            unlockTime = Time.time - trialStartTime;
        statusText.text = "<color=#00FF00><b>Unlocked! Spin the dial 180°</b></color>";
        Debug.Log("Trial " + (currentTrial + 1) + " unlocked at " + unlockTime);
    }

    public void On180Reached()
    {
        reachedTime = Time.time - trialStartTime;
        statusText.text = "<color=#FF6600><b>180° reached! Say 'Lock' to complete</b></color>";
        Debug.Log("Trial " + (currentTrial + 1) + " 180 reached at " + reachedTime);
    }

    public void OnLockAttempt(bool success)
    {
        lockAttempts++;
        if (success)
        {
            lockTime = Time.time - trialStartTime;
            float totalTime = Time.time - trialStartTime;

            statusText.text = "<color=#00FF00><b>Trial Complete!</b></color>";
            Debug.Log("Trial " + (currentTrial + 1) + " completed!");

            csvRows.Add(
                participantID + "," +
                (currentTrial + 1) + "," +
                trialStartTime.ToString("F2") + "," +
                unlockTime.ToString("F2") + "," +
                reachedTime.ToString("F2") + "," +
                lockTime.ToString("F2") + "," +
                totalTime.ToString("F2") + "," +
                unlockCount + "," +
                lockAttempts + "," +
                "true"
            );

            if (currentTrial + 1 < cylinders.Length)
            {
                instructionText.text = "<color=white><b>Walk forward to next trial</b></color>";
                StartCoroutine(BetweenTrials(currentTrial + 1));
            }
            else
                EndStudy();
        }
        else
        {
            statusText.text = "<color=red><b>Not quite! Keep spinning...</b></color>";
            Debug.Log("Lock attempted but 180 not reached yet");
        }
    }

    IEnumerator BetweenTrials(int nextTrial)
    {
        int countdown = 5;
        while (countdown > 0)
        {
            instructionText.text = "<color=white><b>Next trial in: " + countdown + "</b></color>";
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        if (nextTrial == 1)
        {
            trialWall2.SetActive(false);
            tunnel2Renderer.material = colorChangingShader2;
        }
        else if (nextTrial == 2)
        {
            trialWall3.SetActive(false);
            tunnel3Renderer.material = colorChangingShader3;
        }

        instructionText.text = "<color=white><b>Walk forward to begin</b></color>";
        StartTrial(nextTrial);
    }

    void EndStudy()
    {
        trialNumberText.text = "";
        instructionText.text = "";
        statusText.text = "";
        completionText.text = "<color=#00FF00><b>Study Complete!\nThank you!</b></color>";
        SaveCSV();
    }

    void SaveCSV()
    {
        string fileName = "Participant_" + participantID + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllLines(path, csvRows);
        Debug.Log("CSV saved to: " + path);
    }
}