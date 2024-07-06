using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;

public class OculusVRCalibration : MonoBehaviour
{
    [SerializeField] Transform calibrationPointA;
    [SerializeField] Transform calibrationPointB;
    [SerializeField] Transform calibrationPointC;
    [SerializeField] InputActionReference rightControllerActivate;
    [SerializeField] InputActionReference rightControllerUiPress;
    [SerializeField] GameObject player;
    [SerializeField] Camera playerHead;

    private bool isCalibrating = false;
    private bool isCalibrated = false;
    private float calibrationTimer = 0f;
    private const float calibrationDuration = 3f;
    private float buttonPressedTime = 0f;
    private bool isButtonPressed = false;

    void Update()
    {
        // Проверяем, нажаты ли кнопки A и B на правом контроллере
        bool primaryButtonPressed = rightControllerActivate.action.ReadValue<float>() > 0.5f;
        bool secondaryButtonPressed = rightControllerUiPress.action.ReadValue<float>() > 0.5f;

        if (primaryButtonPressed && secondaryButtonPressed)
        {
            if (!isCalibrating && !isButtonPressed)
            {
                StartCalibration();
                isButtonPressed = true;
                buttonPressedTime = Time.time;
            }
            else if (isCalibrated)
            {
                PerformCalibrationAction();
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

        // Проверяем, сколько времени кнопка была нажата
        if (isButtonPressed)
        {
            if (Time.time - buttonPressedTime >= 3f)
            {
                // Кнопка была нажата в течение 3 секунд
                MoveToTarget();
                isButtonPressed = false;
            }
        }
    }

    private void StartCalibration()
    {
        isCalibrating = true;
        calibrationTimer = 0f;

        // Перемещаем игрока на первую точку калибровки
        ResetPosition(calibrationPointA);
    }

    private void CompleteCalibration()
    {
        isCalibrating = false;
        isCalibrated = true;
        calibrationTimer = 0f;

        // Разворачиваем взгляд игрока с учетом второй точки калибровки относительно первой
        SetCameraRotation(calibrationPointA, calibrationPointB);
        
        // Устанавливаем высоту пола по положению правого контроллера
        SetFloorHeight();
    }

    private void PerformCalibrationAction()
    {
        // Перемещаем игрока к третьей точке калибровки
        ResetPosition(calibrationPointC);

        // Разворачиваем взгляд игрока в направлении от контроллера к третьей точке
        SetCameraRotation(calibrationPointB, calibrationPointC);
    }

    private void MoveToTarget()
    {
        // Код для перемещения на целевую точку
        transform.position = calibrationPointC.position;
    }

    private void ResetPosition(Transform targetPosition)
    {
        var rotationAngleY = targetPosition.rotation.eulerAngles.y -
                             playerHead.transform.rotation.eulerAngles.y;
        player.transform.Rotate(xAngle: 0, rotationAngleY, zAngle: 0);

        var distanceDiff = targetPosition.position -
                           playerHead.transform.position;
        player.transform.position += distanceDiff;
    }

    private void SetCameraRotation(Transform pointA, Transform pointB)
    {
        Vector3 calibrationPointA = pointA.position;
        Vector3 calibrationPointB = pointB.position;
        Vector3 calibrationDirection = calibrationPointB - calibrationPointA;
        Quaternion targetRotation = Quaternion.LookRotation(calibrationDirection, Vector3.up);
        playerHead.transform.rotation = Quaternion.Slerp(playerHead.transform.rotation, targetRotation, 1f);
    }

    private void SetFloorHeight()
    {
        UnityEngine.XR.InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 controllerPosition))
        {
            player.transform.position = new Vector3(player.transform.position.x, controllerPosition.y, player.transform.position.z);
        }
    }
}