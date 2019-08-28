using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tobii.Gaming;

public class InputManager : Singleton<InputManager>
{

    public enum InputMode { Controller, Keyboard };

    #region Axis Values
    public Vector2 _leftStick;
    public Vector2 _rightStick;
    public Vector2 _DPad;
    public float _leftTrigger = 0.0f;
    public float _rightTrigger = 0.0f;
    #endregion

    #region Input Events
    public Action OnButtonXPressed;
    public Action OnButtonXReleased;
    public bool _buttonXHold = false;

    public Action OnButtonYPressed;
    public Action OnButtonYReleased;
    public bool _buttonYHold = false;

    public Action OnButtonAPressed;
    public Action OnButtonAReleased;
    public bool _buttonAHold = false;

    public Action OnButtonBPressed;
    public Action OnButtonBReleased;
    public bool _buttonBHold = false;

    public Action OnLeftBumperPressed;
    public Action OnLeftBumperReleased;
    public bool _leftBumperHold = false;

    public Action OnRightBumperPressed;
    public Action OnRightBumperReleased;
    public bool _rightBumperHold = false;

    public Action OnDPadUpPressed;
    public Action OnDPadUpReleased;
    public bool _DPadUpHold = false;

    public Action OnDPadDownPressed;
    public Action OnDPadDownReleased;
    public bool _DPadDownHold = false;

    public Action OnDPadLeftPressed;
    public Action OnDPadLeftReleased;
    public bool _DPadLeftHold = false;

    public Action OnDPadRightPressed;
    public Action OnDPadRightReleased;
    public bool _DPadRightHold = false;

    public Action OnLeftTriggerPressed;
    public Action OnLeftTriggerReleased;
    public bool _leftTriggerHold = false;

    public Action OnRightTriggerPressed;
    public Action OnRightTriggerReleased;
    public bool _rightTriggerHold = false;

    #endregion

    #region EyeTracking
    private bool _eyetrackingEnabled = false;
    private GazePoint _currentGazePoint;
    #endregion

    public bool EyetrackingEnabled { get => _eyetrackingEnabled; }
    public Vector2 LeftStick { get => _leftStick; }
    public Vector2 RightStick { get => _rightStick; }
    public float LeftTrigger { get => _leftTrigger; }
    public float RightTrigger { get => _rightTrigger; }
    public Vector2 DPad { get => _DPad;}
    public bool ButtonXHold { get => _buttonXHold; }
    public bool ButtonYHold { get => _buttonYHold; }
    public bool ButtonAHold { get => _buttonAHold; }
    public bool LeftBumperHold { get => _leftBumperHold; }
    public bool RightBumperHold { get => _rightBumperHold; }
    public bool DPadUpHold { get => _DPadUpHold; }
    public bool DPadDownHold { get => _DPadDownHold; }
    public bool DPadLeftHold { get => _DPadLeftHold; }
    public bool DPadRightHold { get => _DPadRightHold; }
    public bool LeftTriggerHold { get => _leftTriggerHold; }
    public bool RightTriggerHold { get => _rightTriggerHold; }
    public GazePoint CurrentGazePoint { get => _currentGazePoint;}
    

    protected override void Awake()
    {
        base.Awake();

        #region Register Hold Lambda Functions

        OnButtonAPressed += () => { _buttonAHold = true; };
        OnButtonAReleased += () => { _buttonAHold = false; };

        OnButtonBPressed += () => { _buttonBHold = true; };
        OnButtonBReleased += () => { _buttonBHold = false; };

        OnButtonXPressed += () => { _buttonXHold = true; };
        OnButtonXReleased += () => { _buttonXHold = false; };

        OnButtonYPressed += () => { _buttonYHold = true; };
        OnButtonYReleased += () => { _buttonYHold = false; };

        OnLeftBumperPressed += () => { _leftBumperHold = true; };
        OnLeftBumperReleased += () => { _leftBumperHold = false; };

        OnRightBumperPressed += () => { _rightBumperHold = true; };
        OnRightBumperReleased += () => { _rightBumperHold = false; };

        OnDPadUpPressed += () => { _DPadUpHold = true; };
        OnDPadUpReleased += () => { _DPadUpHold = false; };

        OnDPadDownPressed += () => { _DPadDownHold = true; };
        OnDPadDownReleased += () => { _DPadDownHold = false; };

        OnDPadLeftPressed += () => { _DPadLeftHold = true; };
        OnDPadLeftReleased += () => { _DPadLeftHold = false; };

        OnDPadRightPressed += () => { _DPadRightHold = true; };
        OnDPadRightReleased += () => { _DPadRightHold = false; };

        #endregion

    }

    private void Update()
    {
        #region Controller Axes Value Update
        _leftStick.x = Input.GetAxis("LeftStickX");
        _leftStick.y = Input.GetAxis("LeftStickY");
        _rightStick.x = Input.GetAxis("RightStickX");
        _rightStick.y = Input.GetAxis("RightStickY");
        _leftTrigger = Input.GetAxis("LeftTrigger");
        _rightTrigger = Input.GetAxis("RightTrigger");
        _DPad.x = Input.GetAxis("DPadX");
        _DPad.y = Input.GetAxis("DPadY");
        #endregion

        #region EyeTracking

        if (!TobiiAPI.IsConnected)
        {
            Debug.LogError("Eyetracking Device Not Found!");
            _eyetrackingEnabled = false;
        }
        else
        {
            _currentGazePoint = TobiiAPI.GetGazePoint();
        }

        #endregion

        #region Event Broadcasting
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            OnButtonAPressed?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.Joystick1Button0))
        {
            OnButtonAReleased?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            OnButtonBPressed?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.Joystick1Button1))
        {
            OnButtonBReleased?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            OnButtonXPressed?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.Joystick1Button2))
        {
            OnButtonXReleased?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            OnButtonYPressed?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.Joystick1Button3))
        {
            OnButtonYReleased?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            OnLeftBumperPressed?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.Joystick1Button4))
        {
            OnLeftBumperReleased?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            OnRightBumperPressed?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.Joystick1Button5))
        {
            OnRightBumperReleased?.Invoke();
        }

        if (_leftTriggerHold && _leftTrigger == 0.0f)
        {
            OnLeftTriggerReleased?.Invoke();
            _leftTriggerHold = false;
        }
        else if (!_leftTriggerHold && _leftTrigger > 0.0f)
        {
            OnLeftTriggerPressed?.Invoke();
            _leftTriggerHold = true;
        }

        if (_rightTriggerHold && _rightTrigger == 0.0f)
        {
            OnRightTriggerReleased?.Invoke();
            _rightTriggerHold = false;
        }
        else if (!_rightTriggerHold && _rightTrigger > 0.0f)
        {
            OnRightTriggerPressed?.Invoke();
            _rightTriggerHold = true;
        }

        #endregion



    }

}
