using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRCalibration : MonoBehaviour
{
    public GameObject xrOrigin;
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
        bool primaryButtonPressed = rightController.activateAction.action.ReadValue<float>() > 0.5f;
        bool secondaryButtonPressed = rightController.uiPressAction.action.ReadValue<float>() > 0.5f;

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
                    CalibratePositionAndOrientation();
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

    private void CalibratePositionAndOrientation()
    {
        // Калибровка позиции
        xrOrigin.transform.position = calibrationPoint1.transform.position;

        // Калибровка ориентации
        Vector3 directionToCalibrationPoint3 = calibrationPoint3.transform.position - calibrationPoint1.transform.position;
        Quaternion targetSceneRotation = Quaternion.LookRotation(directionToCalibrationPoint3);
        xrOrigin.transform.rotation = targetSceneRotation;

        isCalibrated = true;
        lastCalibrationTime = Time.time;
        Debug.Log("XR Origin calibrated to position and orientation at Calibration Point 1 and 3.");
    }

    private void CalibrateOrientation()
    {
        Vector3 directionToCalibrationPoint3 = calibrationPoint3.transform.position - calibrationPoint1.transform.position;
        Quaternion targetSceneRotation = Quaternion.LookRotation(directionToCalibrationPoint3);
        xrOrigin.transform.rotation = targetSceneRotation;

        Debug.Log("XR Origin orientation calibrated to face the Calibration Point 3.");
    }
}