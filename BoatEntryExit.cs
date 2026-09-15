using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class BoatEntryExit : MonoBehaviour
{
    [Header("References")]
    public GameObject playerCharacter;
    public FirstPersonController playerController;
    public CharacterController playerCharController; // drag PlayerCapsule's CharacterController here
    public Transform seatPoint;
    public Transform exitPoint;
    public BoatController boatController;

    [Header("Settings")]
    public float interactRange = 3f;

    private bool playerInRange = false;
    private bool playerInBoat = false;
    private Transform originalParent;
    private Vector3 originalScale;

    void Update()
    {
        CheckRange();

        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!playerInBoat)
                EnterBoat();
            else
                ExitBoat();
        }
    }

    void CheckRange()
    {
        if (playerCharacter == null || playerInBoat) return;
        float dist = Vector3.Distance(playerCharacter.transform.position, transform.position);
        playerInRange = dist <= interactRange;
    }

    void EnterBoat()
    {
        playerInBoat = true;
        originalParent = playerCharacter.transform.parent;
        originalScale = playerCharacter.transform.localScale;

        playerCharacter.transform.position = seatPoint.position;
        playerCharacter.transform.rotation = seatPoint.rotation;
        playerCharacter.transform.parent = transform;
        playerCharacter.transform.localScale = originalScale;

        if (playerController != null)
            playerController.disableMovement = true;

        if (playerCharController != null)
            playerCharController.enabled = false; // stop collision/depenetration while seated

        boatController.isBeingDriven = true;
    }

    void ExitBoat()
    {
        playerInBoat = false;

        playerCharacter.transform.parent = originalParent;
        playerCharacter.transform.position = exitPoint.position;
        playerCharacter.transform.rotation = exitPoint.rotation;
        playerCharacter.transform.localScale = originalScale;

        if (playerController != null)
            playerController.disableMovement = false;

        if (playerCharController != null)
            playerCharController.enabled = true; // re-enable once back on foot

        boatController.isBeingDriven = false;
    }  
}
