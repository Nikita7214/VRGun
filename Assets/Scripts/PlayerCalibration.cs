using UnityEngine;
using System.Collections;

public class PlayerCalibration : MonoBehaviour
{
    public Transform rightController;
    public float calibrationDistance = 1f; // Расстояние для второй точки калибрации
    private bool isCalibrating = false;
    private Vector3 calibrationOrigin;
    private Quaternion calibrationRotation;
    
    private void Update()
    {
        if (Input.GetButtonDown("RightControllerABButton") && !isCalibrating)
        {
            StartCalibration();
        }
        else if (Input.GetButtonDown("RightControllerABButton") && isCalibrating)
        {
            FinishCalibration();
        }
    }

    private void StartCalibration()
    {
        isCalibrating = true;
        calibrationOrigin = rightController.position;
        calibrationRotation = rightController.rotation;
        Debug.Log("Начата калибровка. Держите правый контроллер в первой точке.");
    }

    private void FinishCalibration()
    {
        isCalibrating = false;

        // Перемещаем игрока в нулевую точку
        transform.position = calibrationOrigin;
        transform.rotation = calibrationRotation;

        // Устанавливаем направление взгляда игрока
        Vector3 secondCalibrationPoint = calibrationOrigin + (rightController.forward * calibrationDistance);
        transform.LookAt(secondCalibrationPoint);

        Debug.Log("Калибровка завершена. Игрок перемещён в нулевую точку и ориентирован на вторую точку калибрации.");
    }
}