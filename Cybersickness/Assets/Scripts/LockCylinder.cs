using UnityEngine;
using Oculus.Voice;
using Meta.WitAi;
using Meta.WitAi.Json;

public class LockCylinder : MonoBehaviour
{
    public AppVoiceExperience appVoiceExperience;
    private bool isUnlocked = false;
    private bool hasReached180 = false;
    private float totalRotation = 0f;
    private float lastControllerRotation;
    private float lastObjectRotation;
    private TrialManager trialManager;
    private bool isActiveTrial = false;

    public void SetTrialManager(TrialManager tm)
    {
        trialManager = tm;
        isActiveTrial = true;
    }

    void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;

        appVoiceExperience.VoiceEvents.OnResponse.AddListener(OnResponse);
        appVoiceExperience.VoiceEvents.OnStoppedListening.AddListener(RestartListening);
        appVoiceExperience.Activate();
    }

    private void RestartListening()
    {
        appVoiceExperience.Activate();
    }

    private void OnResponse(WitResponseNode response)
    {
        if (!isActiveTrial) return;

        string text = response["text"].Value.ToLower().Trim();
        Debug.Log("Heard: " + text);

        if (text.Contains("unlock"))
        {
            isUnlocked = true;
            totalRotation = 0f;
            hasReached180 = false;
            lastObjectRotation = transform.eulerAngles.z;
            lastControllerRotation = OVRInput.GetLocalControllerRotation(
                OVRInput.Controller.RTouch).eulerAngles.z;
            Debug.Log("trialManager is: " + (trialManager == null ? "NULL" : "SET"));
            trialManager?.OnUnlock();
            Debug.Log("Unlocked!");
        }

        if (text.Contains("lock") && !text.Contains("unlock"))
        {
            isUnlocked = false;
            Debug.Log("Locked!");
            trialManager?.OnLockAttempt(hasReached180);

            if (hasReached180)
            {
                isActiveTrial = false;
                Debug.Log("180 reached and locked — disappearing!");
                transform.parent.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (!isUnlocked) return;

        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
        {
            Quaternion controllerRot = OVRInput.GetLocalControllerRotation(
                OVRInput.Controller.RTouch);
            float controllerZ = controllerRot.eulerAngles.z;
            float controllerDelta = Mathf.DeltaAngle(lastControllerRotation, controllerZ) * 0.5f;
            transform.Rotate(0f, 0f, controllerDelta);
            lastControllerRotation = controllerZ;

            float currentObjectRotation = transform.eulerAngles.z;
            float objectDelta = Mathf.DeltaAngle(lastObjectRotation, currentObjectRotation);
            totalRotation += objectDelta;
            lastObjectRotation = currentObjectRotation;

            Debug.Log("Degrees turned: " + Mathf.Abs(totalRotation));
        }
        else
        {
            lastControllerRotation = OVRInput.GetLocalControllerRotation(
                OVRInput.Controller.RTouch).eulerAngles.z;
            lastObjectRotation = transform.eulerAngles.z;
        }

        if (Mathf.Abs(totalRotation) >= 180f && !hasReached180)
        {
            hasReached180 = true;
            trialManager?.On180Reached();
        }
    }

    void OnDestroy()
    {
        appVoiceExperience.VoiceEvents.OnResponse.RemoveListener(OnResponse);
        appVoiceExperience.VoiceEvents.OnStoppedListening.RemoveListener(RestartListening);
    }
}