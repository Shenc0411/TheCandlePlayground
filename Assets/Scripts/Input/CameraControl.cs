using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Settings/CameraControlSetting")]
public class CameraControlSetting : ScriptableObject
{
    public float defaultFollowDistance;
    public float maxFollowDistance;
    public float minFollowDistance;
    public float followDisatnceAdjustmentRate;
    public bool shouldLerpFollowDistanceAdjustment;

    public float followPitchRecoveryRate;
    public bool shouldRecoverPitchWhenIdle;
    public float defaultFollowPitch;
    public float minFollowPitch;
    public float maxFollowPitch;
    public float followPitchAdjustmentSpeed;

    public float lookAheadDistance;

    public float followAngleAdjustmentSpeed;
    public float defaultFollowAngle;

    public Vector3 aimingOffset;
    public float aimingFollowDistance;
}

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
    public Transform transformToFollow;

    private Camera _camera;
    private InputManager _inputManager;
    private SettingManager _settingManager;

    private float _currentFollowDistance = 3.0f;
    private float _targetFollowDistance = 3.0f;

    private float _currentfollowPitch = 40.0f * Mathf.Deg2Rad;
    private float _currentfollowAngle = 90.0f * Mathf.Deg2Rad;

    private Vector3 _followOffset = Vector3.zero;

    private bool _isAiming = false;
    private Coroutine _toAimingCoroutine = null;
    private Coroutine _exitAimingCoroutine = null;
    private float _aimingRate = 5.0f;
    private float _aimingValue = 0.0f;

    private Vector3 _projectedForward;
    private Quaternion _projectedRotation;

    public Vector3 ProjectedForward { get => _projectedForward; }
    public Quaternion ProjectedRotation { get => _projectedRotation; }

    private void Start()
    {
        _inputManager = InputManager.Instance;
        _settingManager = SettingManager.Instance;
        _camera = GetComponent<Camera>();

        _projectedForward = new Vector3(Mathf.Cos(_currentfollowAngle), 0, Mathf.Sin(_currentfollowAngle));
        _projectedRotation = Quaternion.LookRotation(ProjectedForward, Vector3.up);

        transform.position = transformToFollow.position - _projectedForward * _currentFollowDistance
            + new Vector3(0, Mathf.Tan(_currentfollowPitch) * _currentFollowDistance, 0);
        transform.LookAt(transformToFollow.position + _projectedForward * _settingManager.cameraControlSetting.lookAheadDistance);
    }

    private void FixedUpdate()
    {
        #region Control Handler


        _currentfollowAngle -= _inputManager.RightStick.x * _settingManager.cameraControlSetting.followAngleAdjustmentSpeed * Time.fixedDeltaTime;

        if (_currentfollowAngle < 0.0f)
        {
            _currentfollowAngle += 2.0f * Mathf.PI;
        }
        else if (_currentfollowAngle > 2.0f * Mathf.PI)
        {
            _currentfollowAngle -= 2.0f * Mathf.PI;
        }

        if (!_isAiming)
        {

            if (_inputManager.RightStick.y != 0)
            {
                _currentfollowPitch += _inputManager.RightStick.y * _settingManager.cameraControlSetting.followPitchAdjustmentSpeed * Time.fixedDeltaTime;
                _currentfollowPitch = Mathf.Clamp(_currentfollowPitch, _settingManager.cameraControlSetting.minFollowPitch, _settingManager.cameraControlSetting.maxFollowPitch);
            }
            else if (_settingManager.cameraControlSetting.shouldRecoverPitchWhenIdle)
            {
                _currentfollowPitch = Mathf.Lerp(_currentfollowPitch, _settingManager.cameraControlSetting.defaultFollowPitch, _settingManager.cameraControlSetting.followPitchRecoveryRate * Time.fixedDeltaTime);
            }

        }

        _projectedForward = new Vector3(Mathf.Cos(_currentfollowAngle), 0, Mathf.Sin(_currentfollowAngle));
        _projectedRotation = Quaternion.LookRotation(ProjectedForward, Vector3.up);

        #endregion

        #region Collision Handler
        Vector3 targetRay = transformToFollow.position - transform.position;
        RaycastHit hit;

        if (Physics.Linecast(transformToFollow.position - _projectedForward * _targetFollowDistance + Vector3.up * Mathf.Tan(_currentfollowPitch) * _targetFollowDistance, transformToFollow.position, out hit, ~(1 << 9)))
        {
            _targetFollowDistance = (hit.distance * Mathf.Cos(_currentfollowPitch)) * 0.8f;
        }
        else
        {
            if (Physics.Linecast(transformToFollow.position - _projectedForward * _settingManager.cameraControlSetting.maxFollowDistance + Vector3.up * Mathf.Tan(_currentfollowPitch) * _settingManager.cameraControlSetting.maxFollowDistance, transformToFollow.position, out hit, ~(1 << 9)))
            {
                _targetFollowDistance = (hit.distance * Mathf.Cos(_currentfollowPitch)) * 0.8f;
            }
            else
            {
                _targetFollowDistance = _settingManager.cameraControlSetting.maxFollowDistance;
            }
        }

        _targetFollowDistance = Mathf.Clamp(_targetFollowDistance, _settingManager.cameraControlSetting.minFollowDistance, _settingManager.cameraControlSetting.maxFollowDistance);

        if (_isAiming)
        {
            _targetFollowDistance = Mathf.Min(_targetFollowDistance, _settingManager.cameraControlSetting.aimingFollowDistance);
        }

        if (_settingManager.cameraControlSetting.shouldLerpFollowDistanceAdjustment)
        {
            _currentFollowDistance = Mathf.Lerp(_currentFollowDistance, _targetFollowDistance, _settingManager.cameraControlSetting.followDisatnceAdjustmentRate * Time.fixedDeltaTime);
        }
        else
        {
            _currentFollowDistance = _targetFollowDistance;
        }

        #endregion

        transform.position = transformToFollow.position - _projectedForward * _currentFollowDistance
            + new Vector3(0, Mathf.Tan(_currentfollowPitch) * _currentFollowDistance, 0) + _projectedRotation * _followOffset;
        transform.LookAt(transformToFollow.position + _projectedForward * _settingManager.cameraControlSetting.lookAheadDistance);
    }

    private void Initialze()
    {
        _currentFollowDistance = _settingManager.cameraControlSetting.defaultFollowDistance;
        _currentfollowAngle = _settingManager.cameraControlSetting.defaultFollowAngle;
        _currentfollowPitch = _settingManager.cameraControlSetting.defaultFollowPitch;

        _targetFollowDistance = _currentFollowDistance;
    }
    
    public void ToAiming()
    {
        if (_exitAimingCoroutine != null)
        {
            StopCoroutine(_exitAimingCoroutine);
            _exitAimingCoroutine = null;
        }

        _toAimingCoroutine = StartCoroutine(ToAimingCoroutine());
        _isAiming = true;
    }

    public void ExitAiming()
    {
        if(_toAimingCoroutine != null)
        {
            StopCoroutine(_toAimingCoroutine);
            _toAimingCoroutine = null;
        }

        _exitAimingCoroutine = StartCoroutine(ExitAimingCoroutine());
        _isAiming = false;
    }

    private IEnumerator ToAimingCoroutine()
    {

        while (_aimingValue < 1.0f)
        {
            _aimingValue += _aimingRate * Time.deltaTime;

            if (_aimingValue > 1.0f)
            {
                _aimingValue = 1.0f;
            }

            _followOffset = Vector3.Lerp(Vector3.zero, _settingManager.cameraControlSetting.aimingOffset, _aimingValue);

            yield return new WaitForEndOfFrame();
        }

        _toAimingCoroutine = null;
    }

    private IEnumerator ExitAimingCoroutine()
    {

        while (_aimingValue > 0.0f)
        {
            _aimingValue -= _aimingRate * Time.deltaTime;

            if (_aimingValue < 0.0f)
            {
                _aimingValue = 0.0f;
            }

            _followOffset = Vector3.Lerp(Vector3.zero, _settingManager.cameraControlSetting.aimingOffset, _aimingValue);

            yield return new WaitForEndOfFrame();
        }

        _exitAimingCoroutine = null;
    }

}
