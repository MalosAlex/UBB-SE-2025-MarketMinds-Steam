using SteamProfile.Models;
using SteamProfile.Repositories;
using System;
using System.Threading.Tasks;

namespace SteamProfile.Services
{
    public class SessionService
    {
        private readonly SessionRepository _sessionRepository;
        private readonly UsersRepository _usersRepository;

        public SessionService(SessionRepository sessionRepository, UsersRepository usersRepository)
        {
            _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public Guid CreateNewSession(User user)
        {
            // Business logic: Delete any existing sessions for this user
            _sessionRepository.DeleteUserSessions(user.UserId);

            // Create a new session
            var sessionDetails = _sessionRepository.CreateSession(user.UserId);

            // Update the singleton session instance
            UserSession.Instance.UpdateSession(
                sessionDetails.SessionId,
                user.UserId,
                sessionDetails.CreatedAt,
                sessionDetails.ExpiresAt
            );

            return sessionDetails.SessionId;
        }

        public void EndSession()
        {
            var sessionId = UserSession.Instance.CurrentSessionId;
            if (sessionId.HasValue)
            {
                _sessionRepository.DeleteSession(sessionId.Value);
                UserSession.Instance.ClearSession();
            }
        }

        public User GetCurrentUser()
        {
            // Check if we have a valid session
            if (!UserSession.Instance.IsSessionValid())
            {
                if (UserSession.Instance.CurrentSessionId.HasValue)
                {
                    // If session exists but is invalid (likely expired), delete it
                    _sessionRepository.DeleteSession(UserSession.Instance.CurrentSessionId.Value);
                    UserSession.Instance.ClearSession();
                }
                return null;
            }

            // Get user details from the database
            return _usersRepository.GetUserById(UserSession.Instance.UserId);
        }

        public bool IsUserLoggedIn()
        {
            return UserSession.Instance.IsSessionValid();
        }

        public void RestoreSessionFromDatabase(Guid sessionId)
        {
            var sessionDetails = _sessionRepository.GetSessionById(sessionId);

            if (sessionDetails != null)
            {
                // Check if session is expired
                if (DateTime.Now > sessionDetails.ExpiresAt)
                {
                    // Business logic: Delete expired session
                    _sessionRepository.DeleteSession(sessionId);
                    return;
                }

                // Session is valid, update the singleton
                UserSession.Instance.UpdateSession(
                    sessionDetails.SessionId,
                    sessionDetails.UserId,
                    sessionDetails.CreatedAt,
                    sessionDetails.ExpiresAt
                );
            }
        }

        // Method to cleanup expired sessions (could be called by a background job)
        public void CleanupExpiredSessions()
        {
            var expiredSessions = _sessionRepository.GetExpiredSessions();
            foreach (var expiredSessionId in expiredSessions)
            {
                _sessionRepository.DeleteSession(expiredSessionId);
            }

            // Also check the current session
            if (UserSession.Instance.CurrentSessionId.HasValue && !UserSession.Instance.IsSessionValid())
            {
                UserSession.Instance.ClearSession();
            }
        }
    }
}