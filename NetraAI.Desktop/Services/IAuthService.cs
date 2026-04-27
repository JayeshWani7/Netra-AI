using NetraAI.Desktop.Models;

namespace NetraAI.Desktop.Services
{
    /// <summary>
    /// Interface for authentication service
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Login user with email and password
        /// </summary>
        Task<User?> LoginAsync(string email, string password);

        /// <summary>
        /// Sign up a new user
        /// </summary>
        Task<User?> SignupAsync(string email, string password, string displayName);

        /// <summary>
        /// Login with Google
        /// </summary>
        Task<User?> LoginWithGoogleAsync();

        /// <summary>
        /// Logout current user
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Get current authenticated user
        /// </summary>
        User? GetCurrentUser();

        /// <summary>
        /// Restore an existing session without re-authentication
        /// </summary>
        void RestoreSession(User user);

        /// <summary>
        /// Check if user is authenticated
        /// </summary>
        bool IsAuthenticated();

        /// <summary>
        /// Refresh authentication token
        /// </summary>
        Task<bool> RefreshTokenAsync();

        /// <summary>
        /// Password reset request
        /// </summary>
        Task<bool> RequestPasswordResetAsync(string email);
    }
}
