namespace Smr.Animations {
    public class AbstractBehaviorWithExposedLerpValue<TExposedValue, TValue> : AbstractBehaviorWithLerpValue<TValue> {
        public TExposedValue ExposedStartValue;
        public TExposedValue ExposedEndValue;
    }
}