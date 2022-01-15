using Smr.UI;

namespace Game {
    public class WindowHiddenSignal {
        public AbstractUiElement Window { get; }

        public WindowHiddenSignal(AbstractUiElement window) {
            Window = window;
        }
    }
}