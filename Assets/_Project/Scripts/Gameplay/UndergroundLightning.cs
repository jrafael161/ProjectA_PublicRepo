using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundLightning : MonoBehaviour
{
    [SerializeField]
    PlayerManager playerManager;

    [SerializeField]
    GameObject UndergroundTrigger;

    [SerializeField]
    LayerMask GroundLayers;

    [SerializeField]
    float minimunDistanceToBeConsideredUnderground = 2.5f;

    bool _isWaiting = false;
    bool _isUndeground = false;

    public int numberOfUndergroundTriggersTriggered;
    private void Start()
    {
        if (playerManager == null)
        {
            playerManager = GetComponentInParent<PlayerManager>();
        }

        if (UndergroundTrigger == null)
        {
            UndergroundTrigger = GameObject.Find("UndergroundTrigger");
        }

        StartCoroutine("CheckIfUnderground");
    }

    private void Update()
    {
        if (!_isWaiting)
        {
            StartCoroutine("CheckIfUnderground");
        }
    }

    IEnumerator CheckIfUnderground()
    {
        _isWaiting = true;
        _isUndeground = false;

        bool underSomething = false;

        int undergroundCheckCounter = 0;//if its 3 or more the player is considered to be underground

        Debug.DrawRay(UndergroundTrigger.transform.position, UndergroundTrigger.transform.up * 100, Color.blue, 0.1f, false);
        if (Physics.Raycast(UndergroundTrigger.transform.position, UndergroundTrigger.transform.up, 100, GroundLayers))
        {
            underSomething = true;
        }

        Debug.DrawRay(UndergroundTrigger.transform.position, UndergroundTrigger.transform.right * minimunDistanceToBeConsideredUnderground, Color.blue, 0.1f, false);
        if (Physics.Raycast(UndergroundTrigger.transform.position, UndergroundTrigger.transform.right, minimunDistanceToBeConsideredUnderground, GroundLayers))
        {
            undergroundCheckCounter++;
        }

        Debug.DrawRay(UndergroundTrigger.transform.position, -UndergroundTrigger.transform.right * minimunDistanceToBeConsideredUnderground, Color.blue, 0.1f, false);
        if (Physics.Raycast(UndergroundTrigger.transform.position, -UndergroundTrigger.transform.right, minimunDistanceToBeConsideredUnderground, GroundLayers))
        {
            undergroundCheckCounter++;
        }

        Debug.DrawRay(UndergroundTrigger.transform.position, UndergroundTrigger.transform.forward * minimunDistanceToBeConsideredUnderground, Color.blue, 0.1f, false);
        if (Physics.Raycast(UndergroundTrigger.transform.position, UndergroundTrigger.transform.forward, minimunDistanceToBeConsideredUnderground, GroundLayers))
        {
            undergroundCheckCounter++;
        }

        Debug.DrawRay(UndergroundTrigger.transform.position, -UndergroundTrigger.transform.forward * minimunDistanceToBeConsideredUnderground, Color.blue, 0.1f, false);
        if (Physics.Raycast(UndergroundTrigger.transform.position, -UndergroundTrigger.transform.forward, minimunDistanceToBeConsideredUnderground, GroundLayers))
        {
            undergroundCheckCounter++;
        }

        numberOfUndergroundTriggersTriggered = undergroundCheckCounter;

        if (undergroundCheckCounter >= 3 && underSomething)
        {
            _isUndeground = true;
        }

        if (_isUndeground)
        {
            AmbientLightController._instance.ChangeLightning(0);
            if (CameraHandler._instance.currentCamera != CameraMode.LockOnCamera && CameraHandler._instance.currentCamera != CameraMode.FirstPersonCamera)
            {
                CameraHandler._instance.ChangeCameraMode(CameraMode.FirstPersonCamera);
                playerManager.isUnderground = true;
            }
        }
        else
        {
            AmbientLightController._instance.ChangeLightning(1);
            if (CameraHandler._instance.currentCamera != CameraMode.LockOnCamera && CameraHandler._instance.currentCamera != CameraMode.ThirdPersonCamera)
            {
                CameraHandler._instance.ChangeCameraMode(CameraMode.ThirdPersonCamera);
                playerManager.isUnderground = false;
            }
        }

        yield return new WaitForSeconds(1);
        _isWaiting = false;
    }

    public void CheckIfCurrentlyUnderground()
    {
        _isUndeground = false;

        int undergroundCheckCounter = 0;//if its 3 or more the player is considered to be underground

        Debug.DrawRay(UndergroundTrigger.transform.position, UndergroundTrigger.transform.up * minimunDistanceToBeConsideredUnderground, Color.blue, 0.1f, false);
        if (Physics.Raycast(UndergroundTrigger.transform.position, UndergroundTrigger.transform.up, minimunDistanceToBeConsideredUnderground, GroundLayers))
        {
            undergroundCheckCounter++;
        }

        Debug.DrawRay(UndergroundTrigger.transform.position, UndergroundTrigger.transform.right * minimunDistanceToBeConsideredUnderground, Color.blue, 0.1f, false);
        if (Physics.Raycast(UndergroundTrigger.transform.position, UndergroundTrigger.transform.right, minimunDistanceToBeConsideredUnderground, GroundLayers))
        {
            undergroundCheckCounter++;
        }

        Debug.DrawRay(UndergroundTrigger.transform.position, -UndergroundTrigger.transform.right * minimunDistanceToBeConsideredUnderground, Color.blue, 0.1f, false);
        if (Physics.Raycast(UndergroundTrigger.transform.position, -UndergroundTrigger.transform.right, minimunDistanceToBeConsideredUnderground, GroundLayers))
        {
            undergroundCheckCounter++;
        }

        Debug.DrawRay(UndergroundTrigger.transform.position, UndergroundTrigger.transform.forward * minimunDistanceToBeConsideredUnderground, Color.blue, 0.1f, false);
        if (Physics.Raycast(UndergroundTrigger.transform.position, UndergroundTrigger.transform.forward, minimunDistanceToBeConsideredUnderground, GroundLayers))
        {
            undergroundCheckCounter++;
        }

        Debug.DrawRay(UndergroundTrigger.transform.position, -UndergroundTrigger.transform.forward * minimunDistanceToBeConsideredUnderground, Color.blue, 0.1f, false);
        if (Physics.Raycast(UndergroundTrigger.transform.position, -UndergroundTrigger.transform.forward, minimunDistanceToBeConsideredUnderground, GroundLayers))
        {
            undergroundCheckCounter++;
        }

        if (undergroundCheckCounter >= 3)
        {
            _isUndeground = true;
        }

        if (_isUndeground)
        {
            playerManager.isUnderground = true;
        }
        else
        {
            playerManager.isUnderground = false;
        }
    }

}
