using System;
using UnityEngine;

namespace SMT3.InputSystem
{
    public struct TouchInfo
    {
        public int FingerId;
        public int Lane;
        public Vector2 ScreenPosition;
        public double DpsTime;
    }
    
    public class InputReader : MonoBehaviour
    {
        public event Action<TouchInfo> OnTabBegan;
        public event Action<TouchInfo> OnTabEnded;
        public event Action<TouchInfo> OnHeldTab;
        
        private bool _isHeldTab = false;
        private int _landCount = 4;

        private void Update()
        {
            MouseInputReader();
        }

        private void MouseInputReader()
        {
            TouchInfo touchInfo = BuildTouchInfo(0, Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && !_isHeldTab)
            {
                OnTab(touchInfo);
            }

            if (Input.GetMouseButton(0) && _isHeldTab)
            {
                OnHeld(touchInfo);
            }
            
            if (Input.GetMouseButtonUp(0) && _isHeldTab)
            {
                OnTabEnd(touchInfo);
            }
        }

        private void OnTab(TouchInfo touchInfo)
        {
            _isHeldTab = true;
            OnTabBegan?.Invoke(touchInfo);
        }

        private void OnHeld(TouchInfo touchInfo)
        {
            OnHeldTab?.Invoke(touchInfo);
        }

        private void OnTabEnd(TouchInfo touchInfo)
        {
            _isHeldTab = false;
            OnTabEnded?.Invoke(touchInfo);
        }

        private TouchInfo BuildTouchInfo(int fingerID ,Vector2 position)
        {
            return new TouchInfo()
            {
                FingerId = fingerID,
                Lane = ScreenToLane(position),
                ScreenPosition = position,
                DpsTime = AudioSettings.dspTime
            };
        }
        
        private int ScreenToLane(Vector2 position)
        {
            float nomalized = Mathf.Clamp01(position.x / Screen.width);
            int lane = (int)(nomalized * _landCount) + 1;
            return Mathf.Clamp(lane, 1, _landCount);
        }
        
    }
}
