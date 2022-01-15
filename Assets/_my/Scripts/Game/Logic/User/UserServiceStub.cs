using VContainer;

namespace Game {
    public class UserServiceStub : IUserService {
        public string UserId => "local_user";
        
        [Preserve]
        public UserServiceStub() {}
    }
}