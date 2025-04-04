using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Interfaces
{
    public interface ISessionService
    {
        Guid CreateNewSession(User user);
        void EndSession();
        User GetCurrentUser();
        bool IsUserLoggedIn();
        void RestoreSessionFromDatabase(Guid sessionId);
        void CleanupExpiredSessions();
    }
}
