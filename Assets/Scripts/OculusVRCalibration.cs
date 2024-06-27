using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class OculusVRCalibration : MonoBehaviour
{
    public GameObject player;
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
    private int currentCalibrationIndex = 0;
    private Vector3[] calibrationPoints = new Vector3[2];
    private bool isControllerCalibration = false;

    private void Start()
    {
        ResetSceneTransform();
    }

    private void Update()
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
                CalibrateScene(!isControllerCalibration);
                isButtonHeld = false;
            }
        }
        else
        {
            isButtonHeld = false;
        }
    }

    private void CalibrateScene(bool toControllerCalibration)
    {
        InputDevices.GetDeviceAtXRNode(oculusNode).TryGetFeatureValue(CommonUsages.devicePosition, out var oculusPosition);
        InputDevices.GetDeviceAtXRNode(oculusNode).TryGetFeatureValue(CommonUsages.deviceRotation, out var oculusRotation);

        if (!isCalibrated)
        {
            calibrationPoints[currentCalibrationIndex] = calibrationPoint1.transform.position;
            currentCalibrationIndex = (currentCalibrationIndex + 1) % 2;
            calibrationPoints[currentCalibrationIndex] = calibrationPoint2.transform.position;
            isControllerCalibration = toControllerCalibration;
            Debug.Log($"Calibration Points set. Controller Calibration: {isControllerCalibration}");
        }
        else
        {
            Vector3 cameraOffset1 = calibrationPoint1.transform.position - calibrationPoints[0];
            Vector3 cameraOffset2 = calibrationPoint2.transform.position - calibrationPoints[1];
            Vector3 cameraOffset = isControllerCalibration ? (cameraOffset1 + cameraOffset2) / 2f : cameraOffset1;

            player.transform.position = oculusPosition - cameraOffset;

            if (isControllerCalibration)
            {
                Vector3 directionToController = rightController.transform.position - calibrationPoint1.transform.position;
                directionToController.y = 0;

                Quaternion currentSceneRotation = player.transform.rotation;
                Quaternion targetSceneRotation = Quaternion.LookRotation(directionToController);
                player.transform.rotation = targetSceneRotation * Quaternion.Inverse(currentSceneRotation);
            }

            Debug.Log(isControllerCalibration ? "Scene calibrated to face the Right Controller." : "Scene calibrated to face the Head.");
        }

        isCalibrated = !isCalibrated;
        lastCalibrationTime = Time.time;
    }

    private void ResetSceneTransform()
    {
        InputDevices.GetDeviceAtXRNode(oculusNode).TryGetFeatureValue(CommonUsages.devicePosition, out var oculusPosition);
        InputDevices.GetDeviceAtXRNode(oculusNode).TryGetFeatureValue(CommonUsages.deviceRotation, out var oculusRotation);

        // Устанавливаем правильную начальную позицию и ориентацию игрока
        player.transform.SetPositionAndRotation(oculusPosition, Quaternion.Euler(oculusRotation.eulerAngles.x, oculusRotation.eulerAngles.y, oculusRotation.eulerAngles.z));
    }
}