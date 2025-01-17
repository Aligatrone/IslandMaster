using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.InteractableSystem
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] float maxInteractingDistance = 10;
        [SerializeField] float interactingRadius = 1;
 
        LayerMask layerMask;
        Transform cameraTransform;
        InputAction interactAction;
        
        Vector3 origin;
        Vector3 direction;
        Vector3 hitPosition;
        float hitDistance;
 
        [HideInInspector] public Interactable interactableTarget;
        
        void Start()
        {
            cameraTransform = Camera.main.transform;
            layerMask = LayerMask.GetMask("Interactable","Enemy","NPC");
 
            interactAction = GetComponent<PlayerInput>().actions["Interact"];
            interactAction.performed += Interact;
        }
        
        void Update()
        {
            direction = cameraTransform.forward;
            origin = cameraTransform.position;
            RaycastHit hit;
 
            if (Physics.SphereCast(origin, interactingRadius, direction, out hit, maxInteractingDistance, layerMask))
            {
                hitPosition = hit.point;
                hitDistance = hit.distance;
                if(hit.transform.TryGetComponent(out interactableTarget))
                {
                    interactableTarget.TargetOn();
                }
            }
            else if (interactableTarget)
            {
                interactableTarget.TargetOff();
                interactableTarget = null;
            }
        }
        private void Interact(InputAction.CallbackContext obj)
        {
            if(interactableTarget == null) return;
            
            if(Vector3.Distance(transform.position,interactableTarget.transform.position) <= interactableTarget.interactionDistance)
            {
                interactableTarget.Interact();
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin,origin+direction * hitDistance);
            Gizmos.DrawWireSphere(hitPosition, interactingRadius);
        }
        
        private void OnDestroy()
        {
            interactAction.performed -= Interact;
        }
    }
}