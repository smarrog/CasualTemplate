namespace Smr.Animations {
    public enum TimeMachineAction {
        Marker = 0,
        JumpToTime = 1,
        JumpToMarker = 2,
        Pause = 3,
        Event = 4,
        Skip = 5
    }

    public enum TimeMachineConditionType {
        Always = 0,
        Never = 1,
        Active = 2,
        NotActive = 3,
        Custom = 4,
        CustomInvert = 5,
    }
}