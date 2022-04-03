public class InteractableLeverThrottle : InteractableLever
{
    protected override void updateState(float _percent)
    {
        base.updateState(_percent);

        TrainController.Instance.SetThrottleAndBreak(_percent);
    }
}
