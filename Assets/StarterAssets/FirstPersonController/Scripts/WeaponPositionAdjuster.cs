using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    /// <summary>
    /// Temporary script to help adjust weapon position in real-time
    /// Attach to PlayerCameraRoot with WeaponManager
    /// </summary>
    public class WeaponPositionAdjuster : MonoBehaviour
    {
        [Header("Adjustment Settings")]
        [Tooltip("How fast to move the weapon")]
        public float positionSpeed = 1.0f;

        [Tooltip("How fast to rotate the weapon")]
        public float rotationSpeed = 50f;

        [Tooltip("How fast to scale the weapon")]
        public float scaleSpeed = 0.5f;

        private WeaponManager weaponManager;
        private GameObject currentWeapon;

        void Start()
        {
            weaponManager = GetComponent<WeaponManager>();
            if (weaponManager == null)
            {
                Debug.LogError("[WeaponPositionAdjuster] WeaponManager not found!");
            }
        }

        void Update()
        {
            if (weaponManager == null) return;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null) return;

            // Get the current active weapon
            currentWeapon = GetActiveWeapon();
            if (currentWeapon == null) return;

            Vector3 currentPos = currentWeapon.transform.localPosition;
            Vector3 currentRot = currentWeapon.transform.localRotation.eulerAngles;
            Vector3 currentScale = currentWeapon.transform.localScale;

            bool changed = false;

            // Position adjustments (Arrow keys)
            if (Keyboard.current.upArrowKey.isPressed)
            {
                currentPos.y += positionSpeed * Time.deltaTime;
                changed = true;
            }
            if (Keyboard.current.downArrowKey.isPressed)
            {
                currentPos.y -= positionSpeed * Time.deltaTime;
                changed = true;
            }
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                currentPos.x -= positionSpeed * Time.deltaTime;
                changed = true;
            }
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                currentPos.x += positionSpeed * Time.deltaTime;
                changed = true;
            }

            // Z-axis (depth) - Page Up/Down
            if (Keyboard.current.pageUpKey.isPressed)
            {
                currentPos.z += positionSpeed * Time.deltaTime;
                changed = true;
            }
            if (Keyboard.current.pageDownKey.isPressed)
            {
                currentPos.z -= positionSpeed * Time.deltaTime;
                changed = true;
            }

            // Rotation adjustments (Numpad 4,6,8,2,7,9)
            if (Keyboard.current.numpad8Key.isPressed)
            {
                currentRot.x -= rotationSpeed * Time.deltaTime;
                changed = true;
            }
            if (Keyboard.current.numpad2Key.isPressed)
            {
                currentRot.x += rotationSpeed * Time.deltaTime;
                changed = true;
            }
            if (Keyboard.current.numpad4Key.isPressed)
            {
                currentRot.y -= rotationSpeed * Time.deltaTime;
                changed = true;
            }
            if (Keyboard.current.numpad6Key.isPressed)
            {
                currentRot.y += rotationSpeed * Time.deltaTime;
                changed = true;
            }
            if (Keyboard.current.numpad7Key.isPressed)
            {
                currentRot.z -= rotationSpeed * Time.deltaTime;
                changed = true;
            }
            if (Keyboard.current.numpad9Key.isPressed)
            {
                currentRot.z += rotationSpeed * Time.deltaTime;
                changed = true;
            }

            // Scale adjustments (+ and -)
            if (Keyboard.current.equalsKey.isPressed) // + key
            {
                currentScale += Vector3.one * scaleSpeed * Time.deltaTime;
                changed = true;
            }
            if (Keyboard.current.minusKey.isPressed)
            {
                currentScale -= Vector3.one * scaleSpeed * Time.deltaTime;
                currentScale = Vector3.Max(currentScale, Vector3.one * 0.1f); // Minimum scale
                changed = true;
            }

            // Apply changes
            if (changed)
            {
                currentWeapon.transform.localPosition = currentPos;
                currentWeapon.transform.localRotation = Quaternion.Euler(currentRot);
                currentWeapon.transform.localScale = currentScale;
            }

            // Print current values when pressing P
            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                PrintCurrentValues();
            }

            // Reset to defaults when pressing R
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                ResetToDefaults();
            }
#endif
        }

        GameObject GetActiveWeapon()
        {
            // Access the weapons list via reflection or make it public
            // For now, we'll search children
            for (int i = 0; i < weaponManager.weaponContainer.childCount; i++)
            {
                GameObject child = weaponManager.weaponContainer.GetChild(i).gameObject;
                if (child.activeSelf && (child.name.Contains("Revolver") || child.name.Contains("Rifle") || child.name.Contains("Weapon")))
                {
                    return child;
                }
            }
            return null;
        }

        void PrintCurrentValues()
        {
            if (currentWeapon == null) return;

            Debug.Log("=== CURRENT WEAPON VALUES ===");
            Debug.Log($"Position: ({currentWeapon.transform.localPosition.x:F2}, {currentWeapon.transform.localPosition.y:F2}, {currentWeapon.transform.localPosition.z:F2})");
            Debug.Log($"Rotation: ({currentWeapon.transform.localRotation.eulerAngles.x:F2}, {currentWeapon.transform.localRotation.eulerAngles.y:F2}, {currentWeapon.transform.localRotation.eulerAngles.z:F2})");
            Debug.Log($"Scale: ({currentWeapon.transform.localScale.x:F2}, {currentWeapon.transform.localScale.y:F2}, {currentWeapon.transform.localScale.z:F2})");
            Debug.Log("=============================");
        }

        void ResetToDefaults()
        {
            if (currentWeapon == null) return;

            currentWeapon.transform.localPosition = weaponManager.weaponPositionOffset;
            currentWeapon.transform.localRotation = Quaternion.Euler(weaponManager.weaponRotationOffset);
            currentWeapon.transform.localScale = Vector3.one * weaponManager.weaponScale;

            Debug.Log("[WeaponPositionAdjuster] Reset to default values");
        }

        void OnGUI()
        {
            if (currentWeapon == null) return;

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.normal.textColor = Color.yellow;

            GUI.Label(new Rect(10, Screen.height - 220, 500, 20), "=== WEAPON POSITION ADJUSTER ===", style);
            GUI.Label(new Rect(10, Screen.height - 200, 500, 20), "ARROWS: Move X/Y | PgUp/PgDn: Move Z (depth)");
            GUI.Label(new Rect(10, Screen.height - 180, 500, 20), "NUMPAD 8/2/4/6/7/9: Rotate X/Y/Z");
            GUI.Label(new Rect(10, Screen.height - 160, 500, 20), "+/-: Scale | P: Print values | R: Reset");
            GUI.Label(new Rect(10, Screen.height - 140, 500, 20), "---", style);
            GUI.Label(new Rect(10, Screen.height - 120, 500, 20), $"Pos: ({currentWeapon.transform.localPosition.x:F2}, {currentWeapon.transform.localPosition.y:F2}, {currentWeapon.transform.localPosition.z:F2})");
            GUI.Label(new Rect(10, Screen.height - 100, 500, 20), $"Rot: ({currentWeapon.transform.localRotation.eulerAngles.x:F0}, {currentWeapon.transform.localRotation.eulerAngles.y:F0}, {currentWeapon.transform.localRotation.eulerAngles.z:F0})");
            GUI.Label(new Rect(10, Screen.height - 80, 500, 20), $"Scale: {currentWeapon.transform.localScale.x:F2}");
        }
    }
}
