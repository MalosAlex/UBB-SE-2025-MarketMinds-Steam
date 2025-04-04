using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.fakes
{
    public class FakeSessionService : ISessionService
    {
        public void CleanupExpiredSessions()
        {
            // throw error
        }

        public Guid CreateNewSession(User user)
        {
            Guid g = Guid.NewGuid();
            return g;
        }

        public void EndSession()
        {
            // throw error
        }

        public User GetCurrentUser()
        {
            User u = new User();
            u.UserId = 1;
            return u;
        }

        public bool IsUserLoggedIn()
        {
            return true;
        }

        public void RestoreSessionFromDatabase(Guid sessionId)
        {
            // throw error
        }
    }
}
