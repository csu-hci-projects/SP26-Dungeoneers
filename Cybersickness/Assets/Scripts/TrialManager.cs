using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class TrialManager : MonoBehaviour
{
    [Header("Sphere")]
    public GameObject spherePrefab;
    public Material blueMaterial;
    public Material greenMaterial;

    [Header("HUD")]
    public TextMeshPro trialNumberText;
    public TextMeshPro interactionMethodText;
    public TextMeshPro trialResultText;
    public TextMeshPro completionMessageText;

    [Header("Experiment Origin")]
    public Transform experimentOrigin;

    [Header("Hand References")]
    public OVRHand leftHand;
    public OVRHand rightHand;

    private bool wasPinchingLeft;
    private bool wasPinchingRight;
    private bool trialActive;
    private bool processingTrial;

    private GameObject currentSphere;
    private float trialStartTime;
    private int currentTrialIndex;
    private readonly List<TrialData> results = new List<TrialData>();
    private readonly List<Trial> trials = new List<Trial>();

    private struct Trial
    {
        public string interaction;
        public float distance;
        public float diameter;
        public Vector3 direction;
        public string directionName;
        public int repetition;
    }

    private void Start()
    {
        BuildTrials();
        SpawnNextSphere();
    }

    private void Update()
    {
        if (!trialActive || processingTrial)
        {
            return;
        }

        Trial trial = trials[currentTrialIndex];

        if (trial.interaction == "Controller Ray" && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            StartCoroutine(CheckMissDelay());
            return;
        }

        if (trial.interaction == "Pinch Ray")
        {
            bool isPinchingLeft = leftHand != null && leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
            bool isPinchingRight = rightHand != null && rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

            bool pinchStarted = (!wasPinchingLeft && isPinchingLeft) || (!wasPinchingRight && isPinchingRight);

            if (pinchStarted && currentSphere != null && currentSphere.activeSelf)
            {
                StartCoroutine(CheckMissDelay());
            }

            wasPinchingLeft = isPinchingLeft;
            wasPinchingRight = isPinchingRight;
        }
    }

    private IEnumerator CheckMissDelay()
    {
        yield return null;

        if (!processingTrial && trialActive && currentSphere != null && currentSphere.activeSelf)
        {
            OnSphereSelected(false);
        }
    }

    private void BuildTrials()
    {
        string[][] latinSquare =
        {
            new[] { "Controller Ray", "Pinch Ray" },
            new[] { "Pinch Ray", "Controller Ray" },
        };

        float[] distances = { 0.3f, 0.6f };
        float[] diameters = { 0.05f, 0.10f };

        string[] directionNames = { "Left", "Right", "Up", "Down" };
        Vector3[] directions =
        {
            Vector3.left * 0.4f,
            Vector3.right * 0.4f,
            Vector3.up * 0.3f,
            Vector3.down * 0.2f,
        };

        int dirIndex = 0;
        for (int rep = 0; rep < 2; rep++)
        {
            foreach (string interaction in latinSquare[rep])
            {
                foreach (float distance in distances)
                {
                    foreach (float diameter in diameters)
                    {
                        trials.Add(new Trial
                        {
                            interaction = interaction,
                            distance = distance,
                            diameter = diameter,
                            direction = directions[dirIndex % directions.Length],
                            directionName = directionNames[dirIndex % directionNames.Length],
                            repetition = rep + 1,
                        });
                        dirIndex++;
                    }
                }
            }
        }
    }

    private void SpawnNextSphere()
    {
        if (currentTrialIndex >= trials.Count)
        {
            EndExperiment();
            return;
        }

        trialActive = true;
        Trial trial = trials[currentTrialIndex];

        if (currentSphere != null)
        {
            Destroy(currentSphere);
        }

        Vector3 spawnPos = experimentOrigin.position + trial.direction + Vector3.forward * trial.distance;
        currentSphere = Instantiate(spherePrefab, spawnPos, Quaternion.identity);
        currentSphere.transform.localScale = Vector3.one * trial.diameter;
        currentSphere.GetComponent<Renderer>().material = blueMaterial;

        SphereTarget target = currentSphere.GetComponent<SphereTarget>();
        target.Initialize(this, trial.interaction);

        var rayInteractable = currentSphere.GetComponent<Oculus.Interaction.RayInteractable>();
        if (rayInteractable != null)
        {
            rayInteractable.WhenStateChanged += args =>
            {
                if (args.NewState == Oculus.Interaction.InteractableState.Select)
                {
                    target.RaySelected();
                }
                else if (args.NewState == Oculus.Interaction.InteractableState.Hover)
                {
                    target.OnHoverEnter();
                }
                else if (args.PreviousState == Oculus.Interaction.InteractableState.Hover &&
                         args.NewState != Oculus.Interaction.InteractableState.Select)
                {
                    target.OnHoverExit();
                }
            };
        }

        trialNumberText.text = "<color=white><b>Trial: " + (currentTrialIndex + 1) + " / " + trials.Count + "</b></color>";

        if (trial.interaction == "Controller Ray")
        {
            interactionMethodText.text = "<color=#00FFFF><b>Use: Controller Ray</b></color>";
        }
        else
        {
            interactionMethodText.text = "<color=#FF6600><b>Use: Pinch Ray</b></color>";
        }

        trialResultText.text = string.Empty;
        completionMessageText.text = string.Empty;

        trialStartTime = Time.time;
    }

    public void OnSphereSelected(bool isHit)
    {
        if (processingTrial)
        {
            return;
        }

        processingTrial = true;
        trialActive = false;

        float mt = Time.time - trialStartTime;
        Trial trial = trials[currentTrialIndex];

        results.Add(new TrialData
        {
            trialNumber = currentTrialIndex + 1,
            interaction = trial.interaction,
            distance = trial.distance,
            diameter = trial.diameter,
            direction = trial.directionName,
            repetition = trial.repetition,
            movementTime = mt,
            hit = isHit,
        });

        trialResultText.text = isHit
            ? "<color=#00FF00><b>HIT!</b></color>"
            : "<color=#FF0000><b>MISS!</b></color>";

        currentTrialIndex++;
        StartCoroutine(NextTrialDelay());
    }

    private IEnumerator NextTrialDelay()
    {
        yield return new WaitForSeconds(0.5f);
        processingTrial = false;
        SpawnNextSphere();
    }

    private void EndExperiment()
    {
        if (currentSphere != null)
        {
            Destroy(currentSphere);
        }

        SaveCsv();

        completionMessageText.text = "Experiment Complete!\nCSV saved to: " + GetCsvPath();
        trialNumberText.text = string.Empty;
        interactionMethodText.text = string.Empty;
        trialResultText.text = string.Empty;
    }

    private void SaveCsv()
    {
        string path = GetCsvPath();
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("Trial,Interaction,Distance,Diameter,Direction,Repetition,MT,Hit");
            foreach (TrialData d in results)
            {
                writer.WriteLine(
                    d.trialNumber + "," +
                    d.interaction + "," +
                    d.distance + "," +
                    d.diameter + "," +
                    d.direction + "," +
                    d.repetition + "," +
                    d.movementTime.ToString("F4") + "," +
                    (d.hit ? "1" : "0"));
            }

            int totalTrials = results.Count;
            int misses = results.FindAll(r => !r.hit).Count;
            float errorRate = totalTrials == 0 ? 0f : (float)misses / totalTrials * 100f;

            writer.WriteLine();
            writer.WriteLine("Total Trials," + totalTrials);
            writer.WriteLine("Misses," + misses);
            writer.WriteLine("Error Rate %," + errorRate.ToString("F2"));
        }

        Debug.Log("CSV saved to: " + path);
    }

    private string GetCsvPath()
    {
        return Application.dataPath + "/Dungeoneers_Outputfile.csv";
    }
}

[System.Serializable]
public class TrialData
{
    public int trialNumber;
    public string interaction;
    public float distance;
    public float diameter;
    public string direction;
    public int repetition;
    public float movementTime;
    public bool hit;
}
