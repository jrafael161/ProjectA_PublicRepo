using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode
{
    ThirdPersonCamera,
    FirstPersonCamera,
    LockOnCamera
}
public class CameraHandler : MonoBehaviour
{
    InputHandler inputHandler;
    PlayerManager playerManager;

    public Cinemachine.CinemachineFreeLook thirdPersonCamera;
    public Cinemachine.CinemachineVirtualCamera firstPersonCamera;
    public Cinemachine.CinemachineVirtualCamera lockOnCamera;
    //public Cinemachine.CinemachineVirtualCamera AimCamera;

    public CameraMode previousCamera;
    public CameraMode currentCamera;

    public Transform playerTransform;
    public Transform cameraTransform;

    public LayerMask ignoreLayers;
    public LayerMask environmentLayer;
    public static CameraHandler _instance;

    List<CharacterManager> availableTargets = new List<CharacterManager>();
    public float maximumLockOnDistance = 50;

    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;

    public CharacterManager nearestLockOnTarget;
    public CharacterManager currentLockOnTarget;

    bool allowCameraRotation = true;
    Vector3 currentCameraRotation;

    public Light undergroundLight;

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
        {
            _instance = this;
        }
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        if (playerTransform != null)
        {
            inputHandler = playerTransform.GetComponentInChildren<InputHandler>();
            playerManager = playerTransform.GetComponentInChildren<PlayerManager>();
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player.transform;
            inputHandler = player.GetComponentInChildren<InputHandler>();
            playerManager = player.GetComponentInChildren<PlayerManager>();
        }
        currentCameraRotation = new Vector3();
        DetermineCamera();
    }

    private void Start()
    {
        environmentLayer = LayerMask.NameToLayer("Environment");
    }

    private void FixedUpdate()
    {
        if (!allowCameraRotation)//maybe put this in a coroutine?
        {
            cameraTransform.rotation = Quaternion.Euler(currentCameraRotation);
        }
        if (currentCamera == CameraMode.FirstPersonCamera)
        {
            HandleCameraRotationWhileInFirstPerson();
        }
        if (currentCamera == CameraMode.LockOnCamera)
        {
            EvaluateLockOn(Time.deltaTime);
        }
    }

    public void HandleLockOn()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = -Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(playerTransform.position, 30);
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager character = colliders[i].GetComponentInParent<CharacterManager>();
            if (character != null && colliders[i].tag!="Player")
            {
                Vector3 lockTargetDirection = character.transform.position - playerTransform.position;
                float distanceFromTarget = Vector3.Distance(playerTransform.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);

                if (character.transform.root != playerTransform.transform.root && viewableAngle > -80 && viewableAngle < 80 && distanceFromTarget <= maximumLockOnDistance)//Why their root couldnt be the same? just to avoid mistaking the player for the enemy?
                {
                    RaycastHit hit;
                    if (Physics.Linecast(playerManager.lockOnTransform.position,character.lockOnTransform.position, out hit))
                    {
                        Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position,Color.red);
                        if (hit.transform.gameObject.layer == environmentLayer)
                        {
                            //Debug.Log("Something is in the way");
                        }
                        else
                        {
                            availableTargets.Add(character);
                        }
                    }                                                    
                }
            }
        }

        for (int j = 0; j < availableTargets.Count; j++)
        {
            float distanceFromTarget = Vector3.Distance(playerTransform.position, availableTargets[j].transform.position);
            if (distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                nearestLockOnTarget = availableTargets[j];
            }

            if (inputHandler.lockOnFlag) //&&  currentLockOnTarget
            {
                //Vector3 relativeEnemyPosition = currentLockOnTarget.transform.InverseTransformPoint(availableTargets[j].transform.position);
                //var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[j].transform.position.x;
                //var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[j].transform.position.x;

                Vector3 relativeEnemyPosition = inputHandler.transform.InverseTransformPoint(availableTargets[j].transform.position);
                var distanceFromLeftTarget = relativeEnemyPosition.x;
                var distanceFromRightTarget = relativeEnemyPosition.x;

                if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget && availableTargets[j] != currentLockOnTarget)
                {
                    shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                    leftLockOnTarget = availableTargets[j];
                }

                else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget && availableTargets[j] != currentLockOnTarget)
                {
                    shortestDistanceOfRightTarget = distanceFromRightTarget;
                    rightLockOnTarget = availableTargets[j];
                }
            }
        }
    }

    private void EvaluateLockOn(float delta)
    {
        if (Vector3.Distance(playerManager.transform.position,currentLockOnTarget.transform.position) > maximumLockOnDistance)
        {
            CancelLockOn();
            return;
        }
        if (inputHandler.horizontal > 0)
        {
            var thirdPersonFollow = lockOnCamera.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>();
            thirdPersonFollow.m_ScreenX = Mathf.Lerp(thirdPersonFollow.m_ScreenX, .9f,delta);
        }
        else if(inputHandler.horizontal < 0)
        {
            var thirdPersonFollow = lockOnCamera.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>();
            thirdPersonFollow.m_ScreenX = Mathf.Lerp(thirdPersonFollow.m_ScreenX, 0, delta);
        }
    }

    public void CancelLockOn()
    {
        inputHandler.lockOnFlag = false;
        ClearLockOnTargets();
        ChangeCameraMode(previousCamera);
    }

    public void ClearLockOnTargets()
    {
        availableTargets.Clear();
        if (currentLockOnTarget!=null)
        {
            LockOnReticle lockOnReticle = currentLockOnTarget.GetComponentInChildren<LockOnReticle>();
            if (lockOnReticle != null)
            {
                lockOnReticle.gameObject.SetActive(false);
            }
        }        
        nearestLockOnTarget = null;
        currentLockOnTarget = null;
        lockOnCamera.LookAt = null;
    }

    public void ChangeLockOnReticle(bool isLeft)
    {
        if (currentLockOnTarget != null)
        {
            LockOnReticle lockOnReticle = currentLockOnTarget.GetComponentInChildren<LockOnReticle>(true);
            if (lockOnReticle != null)
            {
                lockOnReticle.gameObject.SetActive(false);
            }
            if (isLeft)
            {
                lockOnReticle = leftLockOnTarget.GetComponentInChildren<LockOnReticle>(true);
                if (lockOnReticle != null)
                {
                    lockOnReticle.gameObject.SetActive(true);
                }
            }
            else if (!isLeft)
            {
                lockOnReticle = rightLockOnTarget.GetComponentInChildren<LockOnReticle>(true);
                if (lockOnReticle != null)
                {
                    lockOnReticle.gameObject.SetActive(true);
                }
            }
        }
    }

    private void HandleCameraRotationWhileInFirstPerson()
    {
        //cameraTransform.Rotate(0,playerTransform.localRotation.y,0);
    }

    public void ChangeCameraMode(CameraMode cameraMode)
    {
        int currentCameraPriority = 0;
        UndergroundLightning undergroundLightning = null;

        switch (currentCamera)
        {
            case CameraMode.ThirdPersonCamera:
                currentCameraPriority = thirdPersonCamera.Priority;
                previousCamera = CameraMode.ThirdPersonCamera;
                break;
            case CameraMode.FirstPersonCamera:
                currentCameraPriority = firstPersonCamera.Priority;
                previousCamera = CameraMode.FirstPersonCamera;
                break;
            case CameraMode.LockOnCamera:
                currentCameraPriority = lockOnCamera.Priority;
                previousCamera = CameraMode.LockOnCamera;
                break;
            default:
                break;
        }

        switch (cameraMode)
        {
            case CameraMode.ThirdPersonCamera:
                thirdPersonCamera.Priority = currentCameraPriority + 1;
                firstPersonCamera.Priority = 9;
                lockOnCamera.Priority = 9;
                thirdPersonCamera.Priority = 10;
                undergroundLightning = playerManager.gameObject.GetComponentInChildren<UndergroundLightning>();
                undergroundLightning.CheckIfCurrentlyUnderground();
                undergroundLight.gameObject.SetActive(false);
                currentCamera = CameraMode.ThirdPersonCamera;
                break;
            case CameraMode.FirstPersonCamera:
                firstPersonCamera.Priority = currentCameraPriority + 1;
                thirdPersonCamera.Priority = 9;
                lockOnCamera.Priority = 9;
                firstPersonCamera.Priority = 10;
                undergroundLightning = playerManager.gameObject.GetComponentInChildren<UndergroundLightning>();
                undergroundLightning.CheckIfCurrentlyUnderground();
                if (playerManager.isUnderground)
                    undergroundLight.gameObject.SetActive(true);
                else
                    undergroundLight.gameObject.SetActive(false);
                currentCamera = CameraMode.FirstPersonCamera;
                break;
            case CameraMode.LockOnCamera:
                lockOnCamera.Priority = currentCameraPriority + 1;
                thirdPersonCamera.Priority = 9;
                firstPersonCamera.Priority = 9;
                lockOnCamera.Priority = 10;
                if (playerManager.isUnderground)
                    undergroundLight.gameObject.SetActive(true);
                else
                    undergroundLight.gameObject.SetActive(false);
                currentCamera = CameraMode.LockOnCamera;
                break;
            default:
                break;
        }
        currentCameraRotation = cameraTransform.rotation.eulerAngles;//Find a way for the camera to start looking in the direction the player is facing
        StartCoroutine("DisableCameraInputsTemporarily");
    }

    private void DetermineCamera()
    {
        //TODO: dedtermine the initial camera given the initial state of the game
    }

    public CameraMode GetCurrentCameraInUse()
    {
        return currentCamera;
    }

    public void DisableCameraInputs()
    {
        Cinemachine.CinemachineInputProvider cinemachineInputProvider = null;
        switch (GetCurrentCameraInUse())
        {
            case CameraMode.ThirdPersonCamera:
                cinemachineInputProvider = thirdPersonCamera.GetComponent<Cinemachine.CinemachineInputProvider>();
                cinemachineInputProvider.enabled = false;
                break;
            case CameraMode.FirstPersonCamera:
                cinemachineInputProvider = firstPersonCamera.GetComponent<Cinemachine.CinemachineInputProvider>();
                cinemachineInputProvider.enabled = false;
                break;
            case CameraMode.LockOnCamera:
            default:
                break;
        }
    }

    public void EnableCameraInputs()
    {
        Cinemachine.CinemachineInputProvider cinemachineInputProvider = null;
        switch (GetCurrentCameraInUse())
        {
            case CameraMode.ThirdPersonCamera:
                cinemachineInputProvider = thirdPersonCamera.GetComponent<Cinemachine.CinemachineInputProvider>();
                cinemachineInputProvider.enabled = true;
                break;
            case CameraMode.FirstPersonCamera:
                cinemachineInputProvider = firstPersonCamera.GetComponent<Cinemachine.CinemachineInputProvider>();
                cinemachineInputProvider.enabled = true;
                break;
            case CameraMode.LockOnCamera:
                break;
            default:
                break;
        }
    }

    IEnumerator DisableCameraInputsTemporarily()
    {
        Cinemachine.CinemachineInputProvider cinemachineInputProvider = null;
        allowCameraRotation = false;
        switch (GetCurrentCameraInUse())
        {
            case CameraMode.ThirdPersonCamera:
                cinemachineInputProvider = thirdPersonCamera.GetComponent<Cinemachine.CinemachineInputProvider>();
                cinemachineInputProvider.enabled = false;
                break;
            case CameraMode.FirstPersonCamera:
                cinemachineInputProvider = firstPersonCamera.GetComponent<Cinemachine.CinemachineInputProvider>();
                cinemachineInputProvider.enabled = false;
                break;
            case CameraMode.LockOnCamera:
                if (previousCamera == CameraMode.ThirdPersonCamera)
                {
                    cinemachineInputProvider = thirdPersonCamera.GetComponent<Cinemachine.CinemachineInputProvider>();
                    cinemachineInputProvider.enabled = false;
                }
                else if (previousCamera == CameraMode.FirstPersonCamera)
                {
                    cinemachineInputProvider = firstPersonCamera.GetComponent<Cinemachine.CinemachineInputProvider>();
                    cinemachineInputProvider.enabled = false;
                }
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(1f);
        cinemachineInputProvider.enabled = true;
        allowCameraRotation = true;
    }
}
