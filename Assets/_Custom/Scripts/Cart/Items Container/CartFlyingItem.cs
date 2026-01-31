using UnityEngine;

public class CartFlyingItem : MonoBehaviour
{
    public Transform TargetPose
    { get; set; } = null;

    private float _time = 0f;
    private AnimationCurve _yHeightFlyCurve;
    private float _duration;
    private Vector3 _startPos;

    public delegate void FlyingFinishedEvent(CartFlyingItem cartFlyingItem);
    public FlyingFinishedEvent OnFlyingFinished;

    public void StartFlying(float duration, AnimationCurve yHeightFlyCurve, Transform targetPose)
    {
        _time = 0f;
        TargetPose = targetPose;
        _yHeightFlyCurve = yHeightFlyCurve;
        _duration = duration;
        _startPos = transform.position;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        float t = _time / _duration;
        float yHeight = _yHeightFlyCurve.Evaluate(t);

        Vector3 flatLerpedPos = Vector3.Lerp(_startPos, TargetPose.position, t);

        transform.position = new Vector3(flatLerpedPos.x, flatLerpedPos.y + yHeight, flatLerpedPos.z);

        if (t >= 1f)
        {
            OnFlyingFinished?.Invoke(this);
        }
    }
}
