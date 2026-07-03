using System;
using System.Collections.Generic;
using SMT3.Game;
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
        private Dictionary<int, TouchInfo> _activeTouches = new();
        private bool _activeInput = false;

        private void OnEnable()
        {
            GameEvents.OnGameStarted += ActiveInput;
            GameEvents.OnGameOver += InactiveInput;
            GameEvents.OnGameReset += ActiveInput;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= ActiveInput;
            GameEvents.OnGameOver -= InactiveInput;
            GameEvents.OnGameReset -= ActiveInput;
        }

        private void ActiveInput()
        {
            _activeInput = true;
            _isHeldTab = false;
            _activeTouches.Clear();
        }

        private void InactiveInput()
        {
            _activeInput = false;
            _isHeldTab = false;
            _activeTouches.Clear();
        }

        private void Update()
        {
            if(!_activeInput) return;
#if !UNITY_EDITOR && UNITY_ANDROID
            HandleTouchInput();
#else
            MouseInputReader();
#endif
            
        }
        
        private void HandleTouchInput()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t  = Input.GetTouch(i);
                var   info = BuildTouchInfo(t.fingerId, t.position);

                switch (t.phase)
                {
                    case TouchPhase.Began:
                        _activeTouches[t.fingerId] = info;
                        OnTabBegan?.Invoke(info);
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (_activeTouches.TryGetValue(t.fingerId, out var prev))
                        {
                            var updated = BuildTouchInfo(t.fingerId, t.position);
                            if (updated.Lane != prev.Lane)
                            {
                                _activeTouches[t.fingerId] = updated;
                                OnHeldTab?.Invoke(updated);
                            }
                            else OnHeldTab?.Invoke(prev);
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (_activeTouches.TryGetValue(t.fingerId, out var end))
                        {
                            OnTabEnded?.Invoke(end);
                            _activeTouches.Remove(t.fingerId);
                        }
                        break;
                }
            }
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
