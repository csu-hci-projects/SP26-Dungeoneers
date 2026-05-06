using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using TMPro;

public class TrialManager : MonoBehaviour
{
    [Header("Participant Settings")]
    public int participantNumber = 0; //change this in unity before each session

    [Header("Setup")]
    public GameObject startWall;
    public Renderer tunnel1Renderer;
    public Renderer tunnel2Renderer;
    public Renderer tunnel3Renderer;

    [Header("Between Trial Walls")]
    public GameObject controlToTrial1Wall;
    public GameObject trialWall2;
    public GameObject trialWall3;

    [Header("Centerpiece Renderers")]
    public Renderer controlCenterpieceRenderer;
    public Renderer centerpiece1Renderer;
    public Renderer centerpiece2Renderer;
    public Renderer centerpiece3Renderer;

    [Header("Materials")]
    public Material colorChangingShader1;
    public Material colorChangingShader2;
    public Material colorChangingShader3;

    [Header("HUD Text")]
    public TextMeshPro instructionText;
    public TextMeshPro trialNumberText;
    public TextMeshPro statusText;
    public TextMeshPro completionText;

    [Header("Trials")]
    public LockCylinder[] cylinders;

    //Latin square for 3 conditions (0=shader1, 1=shader2, 2=shader3)
    private static int[][] latinSquare = new int[][]
    {
        new int[] { 0, 1, 2 }, // participant 1
        new int[] { 1, 2, 0 }, // participant 2
        new int[] { 2, 0, 1 }, // participant 3
        new int[] { 0, 2, 1 }, // participant 4
        new int[] { 2, 1, 0 }, // participant 5
        new int[] { 1, 0, 2 }  // participant 6
    };

    private int[] trialOrder;
    private Material[] materials;
    private Renderer[] tunnelRenderers;
    private Renderer[] centerpieceRenderers;

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
    private bool waitingForY = false;
    private bool trialStarted = false;

    void Start()
    {
        participantID = Guid.NewGuid().ToString();
        Debug.Log("Participant ID: " + participantID);

        // Set up latin square order
        trialOrder = latinSquare[participantNumber % latinSquare.Length];
        Debug.Log("Trial order: " + trialOrder[0] + ", " + trialOrder[1] + ", " + trialOrder[2]);

        // Store materials and renderers in arrays for easy indexing
        materials = new Material[] { colorChangingShader1, colorChangingShader2, colorChangingShader3 };
        tunnelRenderers = new Renderer[] { tunnel1Renderer, tunnel2Renderer, tunnel3Renderer };
        centerpieceRenderers = new Renderer[] { centerpiece1Renderer, centerpiece2Renderer, centerpiece3Renderer };

        startWall.SetActive(true);
        controlToTrial1Wall.SetActive(true);
        trialWall2.SetActive(true);
        trialWall3.SetActive(true);

        instructionText.text = "<color=white><b>Press B to begin study</b></color>";
        trialNumberText.text = "<color=white><b>Sensory Overload Study</b></color>";
        statusText.text = "";
        completionText.text = "";

        csvRows.Add("ParticipantID,ParticipantNumber,Trial,MaterialOrder,TrialStartTime,TimeToUnlock,TimeTo180,TimeToLock,TotalTrialTime,UnlockAttempts,LockAttempts,Success");

        waitingForY = true;
    }

    void Update()
    {
        if (waitingForY && OVRInput.GetDown(OVRInput.Button.Two))
        {
            waitingForY = false;
            StartCoroutine(HandleBPress());
        }
    }

    IEnumerator HandleBPress()
    {
        if (currentTrial == 0 && !trialStarted)
        {
            // Control trial — no material swap
            studyStartTime = Time.time;
            startWall.SetActive(false);
            instructionText.text = "<color=white><b>Walk forward to begin</b></color>";
            yield return new WaitForSeconds(2f);
            StartTrial(0);
        }
        else
        {
            int orderIndex = currentTrial - 1;
            int materialIndex = trialOrder[orderIndex];

            // Remove correct wall
            if (currentTrial == 1) controlToTrial1Wall.SetActive(false);
            else if (currentTrial == 2) trialWall2.SetActive(false);
            else if (currentTrial == 3) trialWall3.SetActive(false);

            // Apply material based on latin square order
            tunnelRenderers[orderIndex].material = materials[materialIndex];
            centerpieceRenderers[orderIndex].material = materials[materialIndex];

            instructionText.text = "<color=white><b>Walk forward to begin</b></color>";
            yield return new WaitForSeconds(2f);
            StartTrial(currentTrial);
        }
    }

    public void StartTrial(int trialIndex)
    {
        currentTrial = trialIndex;
        trialStartTime = Time.time;
        unlockTime = -1f;
        reachedTime = -1f;
        lockTime = -1f;
        unlockCount = 0;
        lockAttempts = 0;
        trialStarted = true;

        cylinders[currentTrial].SetTrialManager(this);

        trialNumberText.text = "<color=white><b>Trial: " + (currentTrial + 1) + " / " + cylinders.Length + "</b></color>";
        instructionText.text = "<color=#00FFFF><b>Say 'Unlock' to begin interaction</b></color>";
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

            string materialLabel = currentTrial == 0 ? "Control" : "Material" + (trialOrder[currentTrial - 1] + 1);

            csvRows.Add(
                participantID + "," +
                participantNumber + "," +
                (currentTrial == 0 ? "Control" : "Trial " + currentTrial) + "," +
                materialLabel + "," +
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
                currentTrial++;
                trialStarted = false;
                instructionText.text = "<color=white><b>Remove headset & Wait for instructions to start next trial</b></color>";
                waitingForY = true;
            }
            else
                EndStudy();
        }
        else
        {
            statusText.text = "<color=red><b>Not quite! Keep spinning...</b></color>";
        }
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
        string path = Path.Combine(
            System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.Desktop), fileName);
        File.WriteAllLines(path, csvRows);
        Debug.Log("CSV saved to: " + path);
    }
}