using UnityEngine;
using UnityEngine.SceneManagement;

public class FPSCameraController : MonoBehaviour
{
#region FPS Variables
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform headCube;
    public Transform capsule;

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
        } else if (Cursor.visible == false) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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

        float newXRotation = xRotation;

        newXRotation = Mathf.Clamp(newXRotation, -50, 26);
        headCube.rotation = Quaternion.Euler(newXRotation, yRotation, 0);
        
        capsule.rotation = Quaternion.Euler(0, yRotation + 90, 0);
    }
}
