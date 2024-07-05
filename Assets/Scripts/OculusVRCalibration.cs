using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OculusVRCalibration : MonoBehaviour
{
    [SerializeField] private Transform resetTransform1;
    [SerializeField] private Transform resetTransform2;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera playerHead;

    private ActionBasedController rightController;

    private bool primaryButtonPressed = false;
    private bool secondaryButtonPressed = false;

    private bool isResettingPosition = false;
    private float resetPositionTimer = 0f;
    private const float resetPositionDelay = 3f;

    private bool isUsingResetTransform1 = true;

    private void Start()
    {
        if (player != null)
        {
            rightController = player.GetComponent<ActionBasedController>();
            if (rightController == null)
            {
                Debug.LogError("OculusVRCalibration: 'player' GameObject does not have an ActionBasedController component.");
            }
        }
        else
        {
            Debug.LogError("OculusVRCalibration: 'player' GameObject is not assigned.");
        }
    }

    private void Update()
    {
        if (rightController != null && player != null && playerHead != null)
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
                    isUsingResetTransform1 = !isUsingResetTransform1;
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
                    ResetPosition();
                }
            }
        }
    }

    private void ResetPosition()
    {
        if (resetTransform1 != null && resetTransform2 != null && playerHead != null && player != null)
        {
            // Поворачиваем игрока относительно текущего resetTransform
            var currentResetTransform = isUsingResetTransform1 ? resetTransform1 : resetTransform2;
            var rotationAngleY = currentResetTransform.rotation.eulerAngles.y -
                                 playerHead.transform.rotation.eulerAngles.y;
            player.transform.Rotate(xAngle: 0, rotationAngleY, zAngle: 0);

            // Перемещаем игрока в позицию текущего resetTransform
            var distanceDiff = currentResetTransform.position -
                               playerHead.transform.position;
            player.transform.position += distanceDiff;
        }
        else
        {
            Debug.LogError("OculusVRCalibration: One or more of the required transforms are not assigned.");
        }
    }
}