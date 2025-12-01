using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

public class GameOverManager : MonoBehaviour
{
    [Header("Refs")]
    public PlayerHealth playerHealth;
    public Canvas restartCanvas;
    public Button restartButton;
    public Transform cameraTransform;

    [Header("Laser control")]
    public XRRayInteractor[] rayInteractors;
    public XRInteractorLineVisual[] rayLineVisuals;

    void Awake()
    {
        if (restartCanvas) restartCanvas.gameObject.SetActive(false);
        SetLineVisuals(false);
    }

    void Start()
    {
        if (playerHealth) playerHealth.onDeath.AddListener(OnPlayerDeath);
        if (restartButton) restartButton.onClick.AddListener(RestartGame);
    }

    void OnDestroy()
    {
        if (playerHealth) playerHealth.onDeath.RemoveListener(OnPlayerDeath);
        if (restartButton) restartButton.onClick.RemoveListener(RestartGame);
    }

    void OnPlayerDeath()
    {
        if (!restartCanvas || !cameraTransform) return;

        // Show the UI (leave world space)
        restartCanvas.gameObject.SetActive(true);

        // Make sure the canvas can receive tracked-device UI input
        var cam = cameraTransform.GetComponent<Camera>();
        if (cam) restartCanvas.worldCamera = cam;

        // SHOW the laser so player can click the button
        SetLineVisuals(true);

        // Pause gameplay last (UI + InputSystem still work)
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        if (restartCanvas) restartCanvas.gameObject.SetActive(false);
        SetLineVisuals(false);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SetLineVisuals(bool enabled)
    {
        if (rayLineVisuals != null)
        {
            for (int i = 0; i < rayLineVisuals.Length; i++)
            {
                var lv = rayLineVisuals[i];
                if (!lv) continue;
                lv.enabled = enabled;
            }
        }
    }
}
