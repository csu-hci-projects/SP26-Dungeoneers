using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

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
    public Transform experimentOrigin; //center point in front of player where spheres will spawn to always be in view


    [Header("Hand References")]
    public OVRHand leftHand;
    public OVRHand rightHand;

    private bool wasPinchingLeft = false;
    private bool wasPinchingRight = false;

    private bool trialActive = false;
    private bool processingTrial = false;


    void Update()
    {
        if (!trialActive || processingTrial) return;

        Trial trial = trials[currentTrialIndex];

        //controller ray miss wait one frame to let RayInteractable
        if (trial.interaction == "Controller Ray" &&
            OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            StartCoroutine(CheckMissDelay());
            return;
        }

        //pinch ray miss
        if (trial.interaction == "Pinch Ray")
        {
            bool isPinchingLeft = leftHand != null &&
                leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
            bool isPinchingRight = rightHand != null &&
                rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

            bool pinchStarted = (!wasPinchingLeft && isPinchingLeft) ||
                                (!wasPinchingRight && isPinchingRight);

            if (pinchStarted && currentSphere != null && currentSphere.activeSelf)
                StartCoroutine(CheckMissDelay());

            wasPinchingLeft = isPinchingLeft;
            wasPinchingRight = isPinchingRight;
        }
    }

    //IEnumerator for unity coroutine so that method can pause without freezing the rest of the program
    IEnumerator CheckMissDelay()
    {
        yield return null; //wait one frame for RayInteractable
        if (!processingTrial && trialActive && currentSphere != null && currentSphere.activeSelf)
        {
            Debug.Log("Miss detected!");
            OnSphereSelected(false);
        }
    }

    private GameObject currentSphere;
    private float trialStartTime;
    private int currentTrialIndex = 0;
    private List<TrialData> results = new List<TrialData>();

    //trial defs
    private struct Trial
    {
        public string interaction;
        public float distance;
        public float diameter;
        public Vector3 direction;
        public string directionName;
        public int repetition;
    }

    private List<Trial> trials = new List<Trial>();

    void Start()
    {
        Debug.Log("TrialNumberText is: " + (trialNumberText == null ? "NULL" : "OK"));
        Debug.Log("InteractionMethodText is: " + (interactionMethodText == null ? "NULL" : "OK"));
        Debug.Log("TrialResultText is: " + (trialResultText == null ? "NULL" : "OK"));
        Debug.Log("CompletionMessageText is: " + (completionMessageText == null ? "NULL" : "OK"));
        Debug.Log("SpherePrefab is: " + (spherePrefab == null ? "NULL" : "OK"));
        Debug.Log("ExperimentOrigin is: " + (experimentOrigin == null ? "NULL" : "OK"));
        Debug.Log("BlueMaterial is: " + (blueMaterial == null ? "NULL" : "OK"));
        Debug.Log("datapath: "+ GetCSVPath());

        BuildTrial();
        Debug.Log("Trials built: " + trials.Count);
        SpawnNextSphere();
        Debug.Log("SpawnNextSphere called");
    }


    void BuildTrial()
    {
        //Latin square order for 2 interactions x 2 distances x 2 sizes x 2 reps
        //Latin square order
        //Rep1:Controller Ray then Pinch Ray
        //Rep2:Pinch Ray then Controller Ray (counterbalanced)
        
        string[][] latinSquare = {
            new string[] { "Controller Ray", "Pinch Ray" }, //rep 1 order
            new string[] { "Pinch Ray", "Controller Ray" }  //rep 2 order (swapped)
        };

        float[] distances = { 0.3f, 0.6f };
        float[] diameters = { 0.05f, 0.10f };

        string[] directionNames = { "Left", "Right", "Up", "Down" };
        Vector3[] directions = {
            Vector3.left * 0.4f,
            Vector3.right * 0.4f,
            Vector3.up * 0.3f,
            Vector3.down * 0.2f
        };

        int dirIndex = 0;
        //each interaction is paired with every distance/diameter combination
        //directions cycle through Left, Right, Up, Down across trials
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
                            //% to wrap around so dirIndex 0 % 4 = 0 is Left
                            //dirIndex 1 % 4 = 1 is Right
                            //dirIndex 2 % 4 = 2 is Up
                            //dirIndex 3 % 4 = 3 is Down
                            //dirIndex 4 % 4 = 0 is Left
                            direction = directions[dirIndex % directions.Length],
                            //same applies to names array
                            directionName = directionNames[dirIndex % directionNames.Length],
                            repetition = rep + 1
                        });
                        dirIndex++;
                    }
                }
            }
        }
    }
    void SpawnNextSphere()
    {
        if (currentTrialIndex >= trials.Count)
        {
            EndExperiment();
            return;
        }

        trialActive = true;

        Trial trial = trials[currentTrialIndex];

        if (currentSphere != null)
            Destroy(currentSphere);

        Vector3 spawnPos = experimentOrigin.position + trial.direction + Vector3.forward * trial.distance;
        //create copy of sphere in scene at spawnPos without any rotation 
        currentSphere = Instantiate(spherePrefab, spawnPos, Quaternion.identity);
        //change sphere size 
        currentSphere.transform.localScale = Vector3.one * trial.diameter;
        //ensure color is blue
        currentSphere.GetComponent<Renderer>().material = blueMaterial;

        SphereTarget target = currentSphere.GetComponent<SphereTarget>();
        target.Initialize(this, trial.interaction);

        var rayInteractable = currentSphere.GetComponent<Oculus.Interaction.RayInteractable>();
        if (rayInteractable != null)
        {
            rayInteractable.WhenStateChanged += (args) =>
            {
                if (args.NewState == Oculus.Interaction.InteractableState.Select){
                    target.RaySelected();
                }
                else if (args.NewState == Oculus.Interaction.InteractableState.Hover){
                    target.OnHoverEnter();
                }
                else if (args.PreviousState == Oculus.Interaction.InteractableState.Hover && args.NewState != Oculus.Interaction.InteractableState.Select){
                    target.OnHoverExit();
                }
            };
        }

        trialNumberText.text = "<color=white><b>Trial: " + (currentTrialIndex + 1) + " / " + trials.Count + "</b></color>";

        if (trial.interaction == "Controller Ray")
            interactionMethodText.text = "<color=#00FFFF><b>Use: Controller Ray</b></color>";
        else
            interactionMethodText.text = "<color=#FF6600><b>Use: Pinch Ray</b></color>";

        trialResultText.text = "";
        completionMessageText.text = "";

        trialStartTime = Time.time;
    }

    public void OnSphereSelected(bool isHit)
    {
        if (processingTrial) return;
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
            hit = isHit
        });

        if (isHit)
            trialResultText.text = "<color=#00FF00><b>HIT!</b></color>";//green in html
        else
            trialResultText.text = "<color=#FF0000><b>MISS!</b></color>";//red in html

        currentTrialIndex++;
        StartCoroutine(NextTrialDelay());
    }

    //IEnumerator for unity coroutine so that method can pause without freezing the rest of the program
    IEnumerator NextTrialDelay()
    {
        yield return new WaitForSeconds(0.5f);
        processingTrial = false;
        SpawnNextSphere();
    }

    void EndExperiment()
    {
        if (currentSphere != null)
            Destroy(currentSphere);

        SaveCSV();
        completionMessageText.text = "Experiment Complete!\nCSV saved to: " + GetCSVPath();
        trialNumberText.text = "";
        interactionMethodText.text = "";
        trialResultText.text = "";
    }

    void SaveCSV()
    {
        string path = GetCSVPath();
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("Trial,Interaction,Distance,Diameter,Direction,Repetition,MT,Hit");
            foreach (TrialData d in results)
            {
                //F4 is 4 decimal places
                writer.WriteLine(
                    d.trialNumber + "," +
                    d.interaction + "," +
                    d.distance + "," +
                    d.diameter + "," +
                    d.direction + "," +
                    d.repetition + "," +
                    d.movementTime.ToString("F4") + "," +
                    (d.hit ? "1" : "0")
                );
            }

            //stats summary
            int totalTrials = results.Count;
            int misses = results.FindAll(r => !r.hit).Count;
            float errorRate = (float)misses / totalTrials * 100f;
            writer.WriteLine("");
            writer.WriteLine("Total Trials," + totalTrials);
            writer.WriteLine("Misses," + misses);
            writer.WriteLine("Error Rate %," + errorRate.ToString("F2"));
        }
        Debug.Log("CSV saved to: " + path);
    }

    string GetCSVPath()
    {
        string path = Application.dataPath + "/Dungeoneers_Outputfile.csv";
        Debug.Log("CSV path: " + path);
        return path;
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