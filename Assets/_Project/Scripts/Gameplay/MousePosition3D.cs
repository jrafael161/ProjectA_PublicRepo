using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MousePosition3D : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private LayerMask nonPointabbleColliders;
    public float radius = 5f;
    public float speed = 5f;
    public float minAngle = -45;
    public float maxAngle = 45;

    private void OnEnable()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, nonPointabbleColliders))
        {
            transform.position = raycastHit.point;
        }
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance != radius)
        {
            Vector3 direction = (transform.position - player.position).normalized;
            transform.position = player.position + direction * radius;
        }
        transform.rotation = mainCamera.transform.rotation;
    }
}
