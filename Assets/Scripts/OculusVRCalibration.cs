using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OculusVRCalibration : MonoBehaviour
{
    [SerializeField] Transform resetTransform1;
    [SerializeField] Transform resetTransform2;
    [SerializeField] GameObject player;
    [SerializeField] Camera playerHead;

    [SerializeField] private ActionBasedController rightController;

    private bool primaryButtonPressed = false;
    private bool secondaryButtonPressed = false;

    private bool isResettingPosition = false;
    private float resetPositionTimer = 0f;
    private const float resetPositionDelay = 3f;

    private void Start()
    {
        rightController = player.GetComponent<ActionBasedController>();
    }

    private void Update()
    {
        // Проверяем состояния кнопок
        primaryButtonPressed = rightController.activateAction.action.ReadValue<float>() > 0.5f;
        secondaryButtonPressed = rightController.uiPressAction.action.ReadValue<float>() > 0.5f;

        // Проверяем, если кнопки A и B зажаты
        if (primaryButtonPressed && secondaryButtonPressed)
        {
            if (!isResettingPosition)
            {
                isResettingPosition = true;
                resetPositionTimer = resetPositionDelay;
            }
        }
        else
        {
            isResettingPosition = false;
        }

        // Обновляем таймер сброса позиции
        if (isResettingPosition)
        {
            resetPositionTimer -= Time.deltaTime;
            if (resetPositionTimer <= 0f)
            {
                ResetPositionToTransform1();
                ResetPositionToTransform2();
            }
        }
    }

    private void ResetPositionToTransform1()
    {
        // Поворачиваем игрока относительно resetTransform2
        var rotationAngleY = resetTransform2.rotation.eulerAngles.y -
                             playerHead.transform.rotation.eulerAngles.y;
        player.transform.Rotate(xAngle: 0, rotationAngleY, zAngle: 0);

        // Перемещаем игрока в позицию resetTransform1
        var distanceDiff = resetTransform1.position -
                           playerHead.transform.position;
        player.transform.position += distanceDiff;
    }

    private void ResetPositionToTransform2()
    {
        // Поворачиваем игрока относительно resetTransform1
        var rotationAngleY = resetTransform1.rotation.eulerAngles.y -
                             playerHead.transform.rotation.eulerAngles.y;
        player.transform.Rotate(xAngle: 0, rotationAngleY, zAngle: 0);

        // Перемещаем игрока в позицию resetTransform2
        var distanceDiff = resetTransform2.position -
                           playerHead.transform.position;
        player.transform.position += distanceDiff;
    }
}