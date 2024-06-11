using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class PlayerCalibration : MonoBehaviour
{
    public GameObject calibrationMarker; // GameObject, представляющий точку калибровки
    private Vector3 calibrationPosition; // Позиция точки калибровки
    private Quaternion calibrationRotation; // Ориентация точки калибровки

    private void Start()
    {
        calibrationPosition = calibrationMarker.transform.position;
        calibrationRotation = calibrationMarker.transform.rotation;
        
        XRInputSubsystem xrInputSubsystem = null;
        if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
        {
            xrInputSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();
            xrInputSubsystem.trackingOriginUpdated += OnTrackingOriginUpdated;
        }
    }

    private void OnTrackingOriginUpdated(XRInputSubsystem xrInputSubsystem)
    {
        // Устанавливаем точку калибровки в качестве новой точки отсчета трекинга
        xrInputSubsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);
        xrInputSubsystem.TryRecenter();

        // Устанавливаем позицию и ориентацию игрока в соответствии с точкой калибровки
        transform.position = calibrationPosition;
        transform.rotation = calibrationRotation;
    }
}