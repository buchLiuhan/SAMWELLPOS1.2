using SAMWELLPOS.MVVM.Models;

namespace SAMWELLPOS.Services
{
    public class SessionService
    {
        public UserModel? CurrentUser { get; private set; }

        public void SetCurrentUser(UserModel user)
        {
            CurrentUser = user;
        }

        public void Clear()
        {
            CurrentUser = null;
        }

        public bool IsCurrentUser(int userId)
        {
            return CurrentUser?.Id == userId;
        }
    }
}