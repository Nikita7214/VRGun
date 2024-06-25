using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class OculusVRCalibration : MonoBehaviour
{
    public GameObject calibrationPoint1;
    public GameObject calibrationPoint2;
    public ActionBasedController rightController;
    public XROrigin xrOrigin;

    private bool isCalibrated = false;
    private bool isButtonHeld = false;
    private float buttonHoldTime = 3.0f;
    private float buttonHoldTimer = 0.0f;
    private float calibrationCooldown = 2.0f;
    private float lastCalibrationTime = 0.0f;

    private XRNode oculusNode = XRNode.Head;

    void Start()
    {
        ResetSceneTransform();
    }

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
        InputDevices.GetDeviceAtXRNode(oculusNode).TryGetFeatureValue(CommonUsages.devicePosition, out var oculusPosition);

        // Рассчитываем среднее смещение между двумя точками калибровки и камерой
        Vector3 cameraOffset1 = Camera.main.transform.position - calibrationPoint1.transform.position;
        Vector3 cameraOffset2 = Camera.main.transform.position - calibrationPoint2.transform.position;
        Vector3 cameraOffset = (cameraOffset1 + cameraOffset2) / 2f;

        xrOrigin.transform.position = oculusPosition - cameraOffset;

        isCalibrated = true;
        lastCalibrationTime = Time.time;
        Debug.Log("Scene calibrated to position between Calibration Points.");
    }

    private void CalibrateOrientation()
    {
        Vector3 directionToController = rightController.transform.position - Camera.main.transform.position;
        directionToController.y = 0;

        Quaternion currentSceneRotation = xrOrigin.transform.rotation;
        Quaternion targetSceneRotation = Quaternion.LookRotation(directionToController);
        xrOrigin.transform.rotation = targetSceneRotation * Quaternion.Inverse(currentSceneRotation);

        Debug.Log("Scene orientation calibrated to face the Right Controller.");
    }

    private void ResetSceneTransform()
    {
        InputDevices.GetDeviceAtXRNode(oculusNode).TryGetFeatureValue(CommonUsages.devicePosition, out var oculusPosition);
        InputDevices.GetDeviceAtXRNode(oculusNode).TryGetFeatureValue(CommonUsages.deviceRotation, out var oculusRotation);
        xrOrigin.transform.SetPositionAndRotation(oculusPosition, oculusRotation);
    }
}