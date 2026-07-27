using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private NavMeshAgent agent;

    private PlayerInputActions inputPlayer;

    void Awake()
    {
        inputPlayer = new PlayerInputActions();
    }

    void Update()
    {
        if (inputPlayer.Player.Mouse.WasPressedThisFrame())
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    private void OnEnable() => inputPlayer.Player.Enable();
    private void OnDisable() => inputPlayer.Player.Disable();
}
