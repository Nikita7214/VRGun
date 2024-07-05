using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;

public class OculusVRCalibration : MonoBehaviour
{
    [SerializeField] Transform calibrationPointA;
    [SerializeField] Transform calibrationPointB;
    [SerializeField] InputActionReference rightControllerActivate;
    [SerializeField] InputActionReference rightControllerUiPress;
    [SerializeField] GameObject player;
    [SerializeField] Camera playerHead;

    private bool isCalibrating = false;
    private float calibrationTimer = 0f;
    private const float calibrationDuration = 3f;

    void Update()
    {
        // Проверяем, нажаты ли кнопки A и B на правом контроллере
        bool primaryButtonPressed = rightControllerActivate.action.ReadValue<float>() > 0.5f;
        bool secondaryButtonPressed = rightControllerUiPress.action.ReadValue<float>() > 0.5f;

        if (primaryButtonPressed && secondaryButtonPressed)
        {
            if (!isCalibrating)
            {
                StartCalibration();
            }
            else
            {
                CompleteCalibration();
            }
        }

        if (isCalibrating)
        {
            calibrationTimer += Time.deltaTime;
            if (calibrationTimer >= calibrationDuration)
            {
                CompleteCalibration();
            }
        }
    }

    private void StartCalibration()
    {
        isCalibrating = true;
        calibrationTimer = 0f;

        // Перемещаем игрока на первую точку калибровки
        ResetPosition();
    }

    private void CompleteCalibration()
    {
        isCalibrating = false;
        calibrationTimer = 0f;

        // Разворачиваем взгляд игрока с учетом второй точки калибровки относительно первой
        Vector3 calibrationPointA = playerHead.transform.position;

        UnityEngine.XR.InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 controllerPosition))
        {
            Vector3 calibrationPointB = controllerPosition;
            Vector3 calibrationDirection = calibrationPointB - calibrationPointA;
            Quaternion targetRotation = Quaternion.LookRotation(calibrationDirection, Vector3.up);
            playerHead.transform.rotation = Quaternion.Slerp(playerHead.transform.rotation, targetRotation, 1f);
        }
    }
    private void ResetPosition()
    {
        var rotationAngleY = calibrationPointA.rotation.eulerAngles.y -
                             playerHead.transform.rotation.eulerAngles.y;
        player.transform.Rotate(xAngle: 0, rotationAngleY, zAngle: 0);

        var distanceDiff = calibrationPointA.position -
                           playerHead.transform.position;
        player.transform.position += distanceDiff;
    }
}