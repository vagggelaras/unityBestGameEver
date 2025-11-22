using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class WeaponManager : MonoBehaviour
    {
        [Header("Weapon Container")]
        [Tooltip("The transform where weapons will be positioned (usually the main camera)")]
        public Transform weaponContainer;

        [Header("Weapon Position/Rotation")]
        [Tooltip("Position offset for weapons")]
        public Vector3 weaponPositionOffset = new Vector3(0.5f, -0.5f, 1f);

        [Tooltip("Rotation offset for weapons")]
        public Vector3 weaponRotationOffset = new Vector3(1.25f, 177.77f, 1.20f);

        [Tooltip("Scale for weapons")]
        public float weaponScale = 1.02f;

        private List<GameObject> weapons = new List<GameObject>();
        private List<string> weaponNames = new List<string>();
        private int currentWeaponIndex = -1;

        void Start()
        {
            Debug.Log($"[WeaponManager] Starting on GameObject: {gameObject.name}");

            // If weaponContainer is not set, use this object's transform (camera)
            if (weaponContainer == null)
            {
                weaponContainer = transform;
                Debug.Log($"[WeaponManager] Weapon container set to: {weaponContainer.name}");
            }

            // Hide all weapons at start
            UpdateWeaponVisibility();
        }

        void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null) return;

            // Handle weapon switching with Q
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                SwitchToPreviousWeapon();
            }
            // Handle weapon switching with Tab
            else if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                SwitchToNextWeapon();
            }

            // Number keys for direct weapon selection
            if (Keyboard.current.digit1Key.wasPressedThisFrame && weapons.Count > 0)
            {
                SwitchToWeapon(0);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame && weapons.Count > 1)
            {
                SwitchToWeapon(1);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame && weapons.Count > 2)
            {
                SwitchToWeapon(2);
            }
#endif
        }

        public void PickupWeapon(GameObject weaponPrefab, string weaponName)
        {
            Debug.Log($"[WeaponManager] PickupWeapon called for: {weaponName}");

            // Check if we already have this weapon
            if (weaponNames.Contains(weaponName))
            {
                Debug.Log($"[WeaponManager] You already have {weaponName}!");
                return;
            }

            Debug.Log($"[WeaponManager] Creating weapon in container: {weaponContainer.name}");

            // Create the weapon as a child of the weapon container
            GameObject newWeapon = Instantiate(weaponPrefab, weaponContainer);
            Debug.Log($"[WeaponManager] Weapon instantiated: {newWeapon.name}");

            // Position and rotate the weapon
            newWeapon.transform.localPosition = weaponPositionOffset;
            newWeapon.transform.localRotation = Quaternion.Euler(weaponRotationOffset);
            newWeapon.transform.localScale = Vector3.one * weaponScale;

            Debug.Log($"[WeaponManager] Weapon positioned at local: {weaponPositionOffset}, rotation: {weaponRotationOffset}");
            Debug.Log($"[WeaponManager] After setting - Local Pos: {newWeapon.transform.localPosition}, World Pos: {newWeapon.transform.position}");

            // Remove any physics components
            Rigidbody rb = newWeapon.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Destroy(rb);
                Debug.Log("[WeaponManager] Removed Rigidbody from weapon");
            }

            Collider col = newWeapon.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
                Debug.Log("[WeaponManager] Removed Collider from weapon");
            }

            // Remove the pickup script if it exists
            WeaponPickup pickup = newWeapon.GetComponent<WeaponPickup>();
            if (pickup != null)
            {
                Destroy(pickup);
                Debug.Log("[WeaponManager] Removed WeaponPickup script from weapon");
            }

            // Add to our weapons list
            weapons.Add(newWeapon);
            weaponNames.Add(weaponName);
            Debug.Log($"[WeaponManager] Added {weaponName} to inventory. Total weapons: {weapons.Count}");

            // If this is our first weapon, equip it
            if (weapons.Count == 1)
            {
                currentWeaponIndex = 0;
                Debug.Log($"[WeaponManager] First weapon equipped: {weaponName}");
            }

            UpdateWeaponVisibility();

            Debug.Log($"[WeaponManager] Pickup complete! Press Tab/Q to switch weapons or 1-3 for direct selection.");
        }

        public void SwitchToWeapon(int index)
        {
            if (index < 0 || index >= weapons.Count) return;

            currentWeaponIndex = index;
            UpdateWeaponVisibility();

            Debug.Log($"Switched to {weaponNames[currentWeaponIndex]}");
        }

        public void SwitchToNextWeapon()
        {
            if (weapons.Count == 0) return;

            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
            UpdateWeaponVisibility();

            Debug.Log($"Switched to {weaponNames[currentWeaponIndex]}");
        }

        public void SwitchToPreviousWeapon()
        {
            if (weapons.Count == 0) return;

            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = weapons.Count - 1;
            }

            UpdateWeaponVisibility();

            Debug.Log($"Switched to {weaponNames[currentWeaponIndex]}");
        }

        private void UpdateWeaponVisibility()
        {
            // Hide all weapons
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i] != null)
                {
                    bool shouldBeActive = i == currentWeaponIndex;
                    weapons[i].SetActive(shouldBeActive);

                    if (shouldBeActive)
                    {
                        Debug.Log($"[WeaponManager] Active weapon: {weaponNames[i]}");
                        Debug.Log($"[WeaponManager] - Local Position: {weapons[i].transform.localPosition}");
                        Debug.Log($"[WeaponManager] - World Position: {weapons[i].transform.position}");
                        Debug.Log($"[WeaponManager] - Local Rotation: {weapons[i].transform.localRotation.eulerAngles}");
                        Debug.Log($"[WeaponManager] - Local Scale: {weapons[i].transform.localScale}");
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            // Draw a sphere at weapon container position
            if (weaponContainer != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(weaponContainer.position, 0.1f);
            }

            // Draw weapons
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i] != null && weapons[i].activeSelf)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(weapons[i].transform.position, Vector3.one * 0.2f);
                }
            }
        }

        void OnGUI()
        {
            // Display current weapon
            if (currentWeaponIndex >= 0 && currentWeaponIndex < weaponNames.Count)
            {
                GUI.Label(new Rect(10, 10, 200, 30), $"Weapon: {weaponNames[currentWeaponIndex]}");
            }

            // Display weapon list
            GUI.Label(new Rect(10, 40, 200, 30), $"Weapons: {weapons.Count}");
            for (int i = 0; i < weaponNames.Count; i++)
            {
                string prefix = (i == currentWeaponIndex) ? ">" : " ";
                GUI.Label(new Rect(10, 70 + (i * 20), 200, 20), $"{prefix} [{i + 1}] {weaponNames[i]}");
            }
        }
    }
}
