namespace Smr.Ui {
    public enum VisibilityState {
        Unknown = 0,
        Appearing = 1,
        Hiding = 2,
        Visible = 3,
        Invisible = 4
    }

    public static class VisibilityStateExtensions {
        public static bool IsAnimated(this VisibilityState state) {
            return state is VisibilityState.Appearing or VisibilityState.Hiding;
        }
    }
}