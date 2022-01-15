using VContainer;
using YG;

namespace Game {
    public class Yg2UserService : IUserService {
        public string UserId => YG2.player.id;

        
        [Preserve]
        public Yg2UserService() {}
    }
}