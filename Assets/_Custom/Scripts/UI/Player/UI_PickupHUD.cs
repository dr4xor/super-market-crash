using UnityEngine;
using UnityEngine.UI;

public class UI_PickupHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Image backgroundImage;

    [Header("World Follow")]
    [SerializeField] private Camera worldCamera;

    private Transform _worldFollowTarget;
    private RectTransform _rectTransform;
    private RectTransform _parentRectTransform;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
        _parentRectTransform = transform.parent as RectTransform;
    }

    /// <summary>
    /// Initializes the PickupHUD to follow a world-space transform and optionally apply a player's color.
    /// </summary>
    /// <param name="worldTransform">The transform in world space to follow (e.g. item or interaction point).</param>
    /// <param name="player">Optional. If set, applies the player's color to the background image.</param>
    public void Initialize(Transform worldTransform, Player player = null)
    {
        _worldFollowTarget = worldTransform;

        if (backgroundImage != null && player != null)
            backgroundImage.color = player.Color;

        SetProgress(0f);
    }

    /// <summary>
    /// Sets the pickup progress (0 = empty, 1 = complete).
    /// </summary>
    public void SetProgress(float progress)
    {
        if (progressSlider != null)
            progressSlider.value = Mathf.Clamp01(progress);
    }

    private void LateUpdate()
    {
        if (_worldFollowTarget == null || _rectTransform == null || _parentRectTransform == null)
            return;

        Camera cam = worldCamera != null ? worldCamera : Camera.main;
        if (cam == null)
            return;

        Vector3 worldPos = _worldFollowTarget.position;
        Vector2 screenPoint = cam.WorldToScreenPoint(worldPos);

        // For Screen Space Overlay canvas, pass null as camera
        Canvas canvas = GetComponentInParent<Canvas>();
        Camera eventCam = canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentRectTransform, screenPoint, eventCam, out Vector2 localPoint))
        {
            _rectTransform.anchoredPosition = localPoint;
        }
    }

    /// <summary>
    /// Stops following the world transform. Call before destroying or when hiding.
    /// </summary>
    public void ClearTarget()
    {
        _worldFollowTarget = null;
    }
}
