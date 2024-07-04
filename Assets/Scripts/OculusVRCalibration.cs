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
    private float lastCalibrationTime = 0.0f;

    private XRNode oculusNode = XRNode.Head;
    private Vector3 initialPlayerPosition = new(74.05929f, 5.05f, -29);
    private Quaternion initialPlayerRotation = new(0, -214.661f, 0, 0);

    private void Start()
    {
        // Сохраняем начальные положение и ориентацию игрока
        initialPlayerPosition = player.transform.position;
        initialPlayerRotation = player.transform.rotation;
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
                CalibrateScene();
                isButtonHeld = false;
            }
        }
        else
        {
            isButtonHeld = false;
        }
    }

    private void CalibrateScene()
    {
        InputDevices.GetDeviceAtXRNode(oculusNode).TryGetFeatureValue(CommonUsages.devicePosition, out var oculusPosition);
        InputDevices.GetDeviceAtXRNode(oculusNode).TryGetFeatureValue(CommonUsages.deviceRotation, out var oculusRotation);

        if (!isCalibrated)
        {
            // Вычисляем положение игрока, основываясь на положении Oculus
            player.transform.position = oculusPosition;
            player.transform.rotation = Quaternion.Euler(oculusRotation.eulerAngles.x, oculusRotation.eulerAngles.y, oculusRotation.eulerAngles.z);

            Debug.Log("Calibration completed. Player position and rotation set.");
        }
        else
        {
            // Возвращаем игрока в начальное положение и ориентацию
            player.transform.SetPositionAndRotation(initialPlayerPosition, initialPlayerRotation);

            Debug.Log("Calibration reset. Player returned to initial position and rotation.");
        }

        isCalibrated = !isCalibrated;
        lastCalibrationTime = Time.time;
    }
}