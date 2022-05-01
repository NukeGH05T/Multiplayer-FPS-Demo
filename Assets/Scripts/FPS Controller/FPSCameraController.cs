using UnityEngine;
using UnityEngine.SceneManagement;

public class FPSCameraController : MonoBehaviour
{
#region FPS Variables
    public float sensX;
    public float sensY;

    public Transform orientation;

    private float xRotation;
    private float yRotation;

    #endregion

    private void Update() {
        if (SceneManager.GetActiveScene().name == "Game") {
            GetMouseInput();

            if (Cursor.visible == true) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void GetMouseInput()
    {
        //Fetching mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70, 80);

        //Rotating Cam and Orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
