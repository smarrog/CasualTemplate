using System.Collections.Generic;
using System.Linq;
using VContainer;

namespace Game {
    public class DiFacade {
        private static IReadOnlyList<IInitializable> _instancesWithInitialization;

        public static void InitializeInstances() {
            foreach (var instance in _instancesWithInitialization) {
                instance.Initialize();
            }
        }
        
        [Preserve]
        public DiFacade(IEnumerable<IInitializable> instancesWithInitialization) {
            _instancesWithInitialization = instancesWithInitialization.ToList();
        }
    }
}