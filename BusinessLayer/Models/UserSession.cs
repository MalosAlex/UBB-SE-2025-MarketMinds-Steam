using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models 
{
    //lalal
    public sealed class UserSession
    {
        private static UserSession? _instance;
        private static readonly object _lock = new object();

        private UserSession() { }

        public static UserSession Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserSession();
                    }
                    return _instance;
                }
            }
        }

        public Guid? CurrentSessionId { get; private set; }
        public int UserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public void UpdateSession(Guid sessionId, int userId, DateTime createdTime, DateTime expireTime)
        {
            lock (_lock)
            {
                CurrentSessionId = sessionId;
                UserId = userId;
                CreatedAt = createdTime;
                ExpiresAt = expireTime;
            }
        }

        public void ClearSession()
        {
            lock (_lock)
            {
                CurrentSessionId = null;
                UserId = 0;
                CreatedAt = DateTime.MinValue;
                ExpiresAt = DateTime.MinValue;
            }
        }

        public bool IsSessionValid()
        {
            lock (_lock)
            {
                return CurrentSessionId.HasValue && 
                       UserId > 0 && 
                       CreatedAt != DateTime.MinValue && 
                       ExpiresAt != DateTime.MinValue && 
                       DateTime.Now < ExpiresAt;
            }
        }
    }
}