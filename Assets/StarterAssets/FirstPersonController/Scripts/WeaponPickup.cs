using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class WeaponPickup : MonoBehaviour
    {
        [Header("Weapon Settings")]
        [Tooltip("The name of this weapon (e.g., 'Revolver', 'Rifle')")]
        public string weaponName = "Weapon";

        [Tooltip("The weapon prefab that will be equipped when picked up")]
        public GameObject weaponPrefab;

        [Header("Pickup Settings")]
        [Tooltip("Distance from which the player can pick up the weapon")]
        public float pickupDistance = 3f;

        private Transform playerTransform;
        private bool playerInRange = false;
        private WeaponManager weaponManager;

        void Start()
        {
            Debug.Log($"[WeaponPickup] Starting pickup for {weaponName} at position {transform.position}");

            // Find the player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Debug.Log($"[WeaponPickup] Player found: {player.name}");
                playerTransform = player.transform;
                weaponManager = player.GetComponentInChildren<WeaponManager>();

                if (weaponManager == null)
                {
                    Debug.LogError("[WeaponPickup] WeaponManager NOT found! Make sure it's attached to the PlayerCameraRoot.");
                }
                else
                {
                    Debug.Log($"[WeaponPickup] WeaponManager found on: {weaponManager.gameObject.name}");
                }
            }
            else
            {
                Debug.LogError("[WeaponPickup] Player NOT found! Make sure the player has the 'Player' tag.");
            }

            // If weaponPrefab is not set, use this object
            if (weaponPrefab == null)
            {
                weaponPrefab = gameObject;
                Debug.Log($"[WeaponPickup] Weapon prefab not set, using this object: {gameObject.name}");
            }
        }

        void Update()
        {
            if (playerTransform == null || weaponManager == null) return;

            // Check distance to player
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            bool wasInRange = playerInRange;
            playerInRange = distance <= pickupDistance;

            // Debug when player enters range
            if (playerInRange && !wasInRange)
            {
                Debug.Log($"[WeaponPickup] Player entered pickup range for {weaponName}. Distance: {distance:F2}m. Press E to pick up.");
            }

#if ENABLE_INPUT_SYSTEM
            // Check for pickup input (E key)
            if (Keyboard.current != null && playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log($"[WeaponPickup] Pickup key E pressed!");
                PickupWeapon();
            }
#endif
        }

        void PickupWeapon()
        {
            if (weaponManager != null)
            {
                weaponManager.PickupWeapon(weaponPrefab, weaponName);

                // Disable or destroy the pickup object
                gameObject.SetActive(false);

                Debug.Log($"Picked up {weaponName}!");
            }
        }

        void OnDrawGizmos()
        {
            // Draw pickup range in editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupDistance);
        }

        void OnGUI()
        {
            // Debug info at top of screen
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                GUI.Label(new Rect(10, 200, 400, 20), $"Distance to {weaponName}: {distance:F2}m (Pickup range: {pickupDistance}m)");
            }

            // Simple UI prompt when player is in range
            if (playerInRange && weaponManager != null)
            {
                // Center screen prompt
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 50, 200, 30),
                    $"Press E to pick up {weaponName}");

                // World space prompt
                if (Camera.main != null)
                {
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
                    if (screenPos.z > 0)
                    {
                        GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 50, 100, 20),
                            $"[{weaponName}]");
                    }
                }
            }
        }
    }
}
