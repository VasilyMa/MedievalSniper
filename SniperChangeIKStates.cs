using UnityEngine;
using UnityEngine.Animations.Rigging;
using Client;

public class SniperChangeIKStates : MonoBehaviour
{
    public TwoBoneIKConstraint HandFirstPositionConstraint;
    public TwoBoneIKConstraint HandSecondPositionConstraint;

    public MultiParentConstraint BowstringMultiParentConstraint;

    public GameState GameState { get; set; }

    private enum BowstringPulling
    {
        Idle,
        Pulling,
        NotPulling
    }

    private BowstringPulling _bowstringPulling;

    public MultiParentConstraint ArrowMultiParentConstraint;

    private enum ArrowMode
    {
        Idle,
        Quive,
        Hand,
        First,
        Second,
        Out
    }

    private ArrowMode _arrowMode;

    void Start()
    {
        _bowstringPulling = BowstringPulling.NotPulling;
        _arrowMode = ArrowMode.Quive;
    }

    void Update()
    {
        if(_bowstringPulling != BowstringPulling.Idle)
        {
            HandFirstPositionConstraint.weight = _bowstringPulling == BowstringPulling.Pulling ? 1f : 0f;

            BowstringMultiParentConstraint.weight = _bowstringPulling == BowstringPulling.Pulling ? 1f : 0f;

            _bowstringPulling = BowstringPulling.Idle;
        }

        if(_arrowMode != ArrowMode.Idle)
        {
            var sourceObjects = ArrowMultiParentConstraint.data.sourceObjects;

            sourceObjects.SetWeight(0, _arrowMode == ArrowMode.Quive ? 1f : 0f);
            sourceObjects.SetWeight(1, _arrowMode == ArrowMode.Hand ? 1f : 0f);
            sourceObjects.SetWeight(2, _arrowMode == ArrowMode.First ? 1f : 0f);
            sourceObjects.SetWeight(3, _arrowMode == ArrowMode.Second ? 1f : 0f);

            ArrowMultiParentConstraint.data.sourceObjects = sourceObjects;

            _arrowMode = ArrowMode.Idle;
        }
    }

    public void Pulling()
    {
        _bowstringPulling = BowstringPulling.Pulling;
        _arrowMode = ArrowMode.First;
    }

    public void Quive()
    {
        _arrowMode = ArrowMode.Quive;
    }

    public void Hand()
    {
        _arrowMode = ArrowMode.Hand;
    }

    public void Out()
    {
        _arrowMode = ArrowMode.Out;
    }

    public void StartPullingBowstring()
    {
        GameState.PullingBowstringSystems = true;
    }
}
