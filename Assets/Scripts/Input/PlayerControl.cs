using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Settings/PlayerControlSetting")]
public class PlayerControlSetting : ScriptableObject
{
    public bool followCameraForwardDirection;
}

public class PlayerControl : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform projectileLauncherTransform;
    public Transform headTransform;

    private CameraControl _cameraControl;
    private InputManager _inputManager;
    private SettingManager _settingManager;

    private Vector3 _movement;
    private Quaternion _targetBodyRotation;
    private Quaternion _targetHeadRotation;

    private float _moveSpeed = 4.0f;
    private float _rotationSpeed = 8.0f;
    private float _idleRotationSpeed = 3.0f;

    private float _aimingAngleTolerance = 45.0f;
    private Coroutine _aimingCoroutine = null;
    private RaycastHit _aimingHit;
    private bool _aimingHasHit = false;

    public enum PlayerState { NotAiming, Aiming };
    private PlayerState _playerState;

    public PlayerState State { get => _playerState;}

    private void Start()
    {
        _inputManager = InputManager.Instance;
        _settingManager = SettingManager.Instance;

        _inputManager.OnRightTriggerPressed += OnRightTriggerPressed;
        _inputManager.OnLeftTriggerPressed += OnLeftTriggerPressed;
        _inputManager.OnLeftTriggerReleased += OnLeftTriggerReleased;

        _cameraControl = Camera.main.GetComponent<CameraControl>();

        _playerState = PlayerState.NotAiming;
    }

    private void FixedUpdate()
    {
        Vector2 movementAxes = _inputManager.LeftStick;

        if (movementAxes.x != 0 || movementAxes.y != 0)
        {
            _movement.x = movementAxes.x;
            _movement.z = -movementAxes.y;

            _movement = _cameraControl.ProjectedRotation * _movement;

            transform.position += _movement.normalized * _moveSpeed * Time.fixedDeltaTime;

            if (_playerState == PlayerState.NotAiming)
            {
                _targetBodyRotation = Quaternion.LookRotation(_movement, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, _targetBodyRotation, _rotationSpeed * Time.fixedDeltaTime);
            }
        }

        if (_playerState == PlayerState.Aiming)
        {
            if (_aimingHasHit)
            {
                _targetHeadRotation = Quaternion.LookRotation(_aimingHit.point - headTransform.position);
                headTransform.rotation = Quaternion.Slerp(headTransform.rotation, _targetHeadRotation, _rotationSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            headTransform.localRotation = Quaternion.Slerp(headTransform.localRotation, Quaternion.identity, _rotationSpeed * Time.fixedDeltaTime);
        }


        if (_settingManager.playerControlSetting.followCameraForwardDirection)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _cameraControl.ProjectedRotation, _idleRotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnLeftTriggerPressed()
    {
        _playerState = PlayerState.Aiming;
        _aimingCoroutine = StartCoroutine(Aiming());
        _cameraControl.ToAiming();
    }

    private void OnLeftTriggerReleased()
    {
        _playerState = PlayerState.NotAiming;
        StopCoroutine(Aiming());
        _aimingCoroutine = null;
        _cameraControl.ExitAiming();
        _aimingHasHit = false;
    }

    private void OnRightTriggerPressed()
    {
        if(_playerState == PlayerState.Aiming)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject newProjectileGO = Instantiate(projectilePrefab, projectileLauncherTransform.position, projectileLauncherTransform.rotation);
        if (_aimingHasHit)
        {
            newProjectileGO.transform.LookAt(_aimingHit.point);
        }
        newProjectileGO.GetComponent<Rigidbody>().velocity = newProjectileGO.transform.forward * 20.0f;
    }

    private IEnumerator Aiming()
    {
        while(_playerState == PlayerState.Aiming)
        {
            Ray ray = Camera.main.ScreenPointToRay(_inputManager.CurrentGazePoint.Screen);
            if (Physics.Raycast(ray, out _aimingHit, float.MaxValue, ~(1 << 9)))
            {
                _aimingHasHit = true;

                //if (Vector3.Angle(_aimingHit.point - transform.position, transform.forward) < _aimingAngleTolerance)
                //{
                    
                //}
                //else
                //{
                //    _aimingHasHit = false;
                //}
            }
            else
            {
                _aimingHasHit = false;
            }

            yield return new WaitForFixedUpdate();
        }

        _aimingCoroutine = null;
    }

}
