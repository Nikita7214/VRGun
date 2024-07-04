using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class OculusVRCalibration : MonoBehaviour
{
    public GameObject xrOrigin;
    public GameObject calibrationPoint1;
    public GameObject calibrationPoint2;
    public ActionBasedController rightController;

    private bool isCalibrated = false;
    private bool isButtonHeld = false;
    private float buttonHoldTime = 3.0f;
    private float buttonHoldTimer = 0.0f;
    private float lastCalibrationTime = 0.0f;

    private XRNode oculusNode = XRNode.Head;
    private Vector3 initialXROriginPosition = new(74.05929f, 5.05f, -29);
    private Quaternion initialXROriginRotation  = new(0, -214.661f, 0, 0);


    private void Start()
    {
        // Сохраняем начальные положение и ориентацию XROrigin
        initialXROriginPosition = xrOrigin.transform.position;
        initialXROriginRotation = xrOrigin.transform.rotation;
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
            // Вычисляем положение и ориентацию XROrigin, основываясь на calibrationPoint1 и calibrationPoint2
            Vector3 centerPosition = (calibrationPoint1.transform.position + calibrationPoint2.transform.position) / 2f;
            Quaternion centerRotation = Quaternion.LookRotation(calibrationPoint2.transform.position - calibrationPoint1.transform.position, Vector3.up);

            xrOrigin.transform.position = centerPosition + (oculusPosition - centerPosition);
            xrOrigin.transform.rotation = centerRotation * Quaternion.Euler(oculusRotation.eulerAngles.x, oculusRotation.eulerAngles.y, oculusRotation.eulerAngles.z);

            Debug.Log("Calibration completed. XROrigin position and rotation set.");
        }
        else
        {
            // Возвращаем XROrigin в начальное положение и ориентацию
            xrOrigin.transform.SetPositionAndRotation(initialXROriginPosition, initialXROriginRotation);
            Debug.Log("Calibration reset. XROrigin returned to initial position and rotation.");
        }

        isCalibrated = !isCalibrated;
        lastCalibrationTime = Time.time;
    }
}