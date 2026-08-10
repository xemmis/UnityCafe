using UnityEngine;
using System;


namespace Core
{
    public static class GameCondition
    {
        public static event Action<bool> OnBuilding;
        public static event Action<bool> OnCameraConditionChanged;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static bool IsBuildingModeEnabled { get; private set; }
        public static bool IsDragging { get; private set; }
        public static bool CameraControllEnabled { get; private set; }
        public static bool IsPaused { get; private set; }
        public static event Action<bool> OnShovelModeChanged;
        public static bool IsShovelModeEnabled { get; private set; }

        public static void ChangeShovelModeCondition()
        {
            IsShovelModeEnabled = !IsShovelModeEnabled;
            OnShovelModeChanged?.Invoke(IsShovelModeEnabled);
        }

        public static void ChangeShovelModeCondition(bool condition)
        {
            IsShovelModeEnabled = condition;
            OnShovelModeChanged?.Invoke(IsShovelModeEnabled);
        }

        public static void Pause()
        {
            if (IsPaused) return;
            IsPaused = true;
            Time.timeScale = 0f;
            OnGamePaused?.Invoke();
        }

        public static void Resume()
        {
            if (!IsPaused) return;
            IsPaused = false;
            Time.timeScale = 1f;
            OnGameResumed?.Invoke();
        }

        public static void ChangeBuildingModeCondition()
        {
            IsBuildingModeEnabled = !IsBuildingModeEnabled;

            OnBuilding?.Invoke(IsBuildingModeEnabled);
        }

        public static void ChangeBuildingModeCondition(bool condition)
        {
            IsBuildingModeEnabled = condition;

            OnBuilding?.Invoke(IsBuildingModeEnabled);
        }

        public static void ChangeCameraControllCondition(bool condition = false)
        {
            CameraControllEnabled = condition;
            OnCameraConditionChanged?.Invoke(CameraControllEnabled);
        }
    }
}
 