using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRCalibration : MonoBehaviour
{
    public GameObject player;
    public GameObject calibrationPoint1;
    public GameObject calibrationPoint3;
    public ActionBasedController rightController;

    private bool isCalibrated = false;
    private bool isButtonHeld = false;
    private float buttonHoldTime = 3.0f;
    private float buttonHoldTimer = 0.0f;
    private float calibrationCooldown = 2.0f;
    private float lastCalibrationTime = 0.0f;

    void Update()
    {
        bool primaryButtonPressed = rightController.selectAction.action.ReadValue<float>() > 0.5f;
        bool secondaryButtonPressed = rightController.activateAction.action.ReadValue<float>() > 0.5f;

        if (primaryButtonPressed && secondaryButtonPressed)
        {
            if (!isButtonHeld)
            {
                isButtonHeld = true;
                buttonHoldTimer = 0.0f;
            }

            buttonHoldTimer += Time.deltaTime;

            if (buttonHoldTimer >= buttonHoldTime)
            {
                if (!isCalibrated)
                {
                    CalibratePosition();
                }
                else if (Time.time - lastCalibrationTime > calibrationCooldown)
                {
                    CalibrateOrientation();
                }

                isButtonHeld = false;
            }
        }
        else
        {
            isButtonHeld = false;
        }
    }

    private void CalibratePosition()
    {
        player.transform.position = calibrationPoint1.transform.position;
        isCalibrated = true;
        lastCalibrationTime = Time.time;
        Debug.Log("Player calibrated to position at Calibration Point 1.");
    }

    private void CalibrateOrientation()
    {
        Vector3 directionToController = rightController.transform.position - calibrationPoint1.transform.position;
        directionToController.y = 0;

        player.transform.rotation = Quaternion.LookRotation(directionToController);
        Debug.Log("Player orientation calibrated to face the Right Controller.");

        Vector3 directionToCalibrationPoint3 = calibrationPoint3.transform.position - rightController.transform.position;
        directionToCalibrationPoint3.y = 0;

        player.transform.rotation = Quaternion.LookRotation(directionToCalibrationPoint3);
        Debug.Log("Player orientation calibrated to align with Calibration Point 3.");
    }
}