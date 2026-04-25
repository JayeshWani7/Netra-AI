using NetraAI.Desktop.Models;
using NetraAI.Desktop.Utils;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace NetraAI.Desktop.Services
{
    /// <summary>
    /// Firebase-based authentication service implementation
    /// Uses Firebase REST API for authentication
    /// </summary>
    public class AuthService : IAuthService
    {
        private User? _currentUser;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly string _firebaseApiKey;
        private const string FirebaseAuthBaseUrl = "https://identitytoolkit.googleapis.com/v1";
        private const string FirebaseTokenRefreshUrl = "https://securetoken.googleapis.com/v1/token";

        public AuthService(ILogger logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            
            var firebaseConfig = ConfigurationManager.GetFirebaseConfig();
            if (firebaseConfig == null || string.IsNullOrEmpty(firebaseConfig.ApiKey))
            {
                throw new InvalidOperationException("Firebase API Key not configured");
            }
            
            _firebaseApiKey = firebaseConfig.ApiKey;
            _logger.Info("AuthService initialized with Firebase configuration");
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            try
            {
                _logger.Info($"Attempting login for user: {email}");
                
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    _logger.Warning("Login attempt with empty email or password");
                    return null;
                }

                var url = $"{FirebaseAuthBaseUrl}/accounts:signInWithPassword?key={_firebaseApiKey}";
                
                var request = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(url, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.Error($"Firebase login failed: {response.StatusCode} - {errorContent}");
                    throw new InvalidOperationException(GetFriendlyFirebaseError(errorContent, isSignup: false));
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var authResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                if (authResponse == null)
                {
                    _logger.Error("Firebase returned invalid response");
                    return null;
                }

                _currentUser = new User
                {
                    UserId = authResponse["localId"]?.ToString() ?? string.Empty,
                    Email = authResponse["email"]?.ToString() ?? email,
                    DisplayName = authResponse["displayName"]?.ToString() ?? "User",
                    AuthToken = authResponse["idToken"]?.ToString() ?? string.Empty,
                    RefreshToken = authResponse["refreshToken"]?.ToString() ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.Info($"Login successful for user: {email} (ID: {_currentUser.UserId})");
                return _currentUser;
            }
            catch (HttpRequestException ex)
            {
                _logger.Error($"Network error during login: {ex.Message}", ex);
                return null;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"Login failed: {ex.Message}", ex);
                return null;
            }
        }

        public async Task<User?> SignupAsync(string email, string password, string displayName)
        {
            try
            {
                _logger.Info($"Attempting signup for user: {email}");
                
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    _logger.Warning("Signup attempt with empty email or password");
                    return null;
                }

                var url = $"{FirebaseAuthBaseUrl}/accounts:signUp?key={_firebaseApiKey}";
                
                var request = new
                {
                    email = email,
                    password = password,
                    displayName = displayName,
                    returnSecureToken = true
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(url, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.Error($"Firebase signup failed: {response.StatusCode} - {errorContent}");
                    throw new InvalidOperationException(GetFriendlyFirebaseError(errorContent, isSignup: true));
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var authResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                if (authResponse == null)
                {
                    _logger.Error("Firebase returned invalid response");
                    return null;
                }

                _currentUser = new User
                {
                    UserId = authResponse["localId"]?.ToString() ?? string.Empty,
                    Email = authResponse["email"]?.ToString() ?? email,
                    DisplayName = authResponse["displayName"]?.ToString() ?? displayName,
                    AuthToken = authResponse["idToken"]?.ToString() ?? string.Empty,
                    RefreshToken = authResponse["refreshToken"]?.ToString() ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.Info($"Signup successful for user: {email} (ID: {_currentUser.UserId})");
                return _currentUser;
            }
            catch (HttpRequestException ex)
            {
                _logger.Error($"Network error during signup: {ex.Message}", ex);
                return null;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"Signup failed: {ex.Message}", ex);
                return null;
            }
        }

        public async Task<User?> LoginWithGoogleAsync()
        {
            try
            {
                _logger.Info("Attempting Google login");
                // Note: Google Sign-In requires OAuth flow with browser/WebView integration
                // This would require additional setup with Google OAuth configuration
                _logger.Warning("Google Sign-In not yet implemented - requires OAuth flow setup");
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"Google login failed: {ex.Message}", ex);
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                _logger.Info("User logout initiated");
                
                if (_currentUser != null)
                {
                    _logger.Info($"Logged out user: {_currentUser.Email}");
                }
                
                _currentUser = null;
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.Error($"Logout failed: {ex.Message}", ex);
            }
        }

        public User? GetCurrentUser()
        {
            return _currentUser;
        }

        public bool IsAuthenticated()
        {
            return _currentUser != null && !string.IsNullOrEmpty(_currentUser.AuthToken);
        }

        public async Task<bool> RefreshTokenAsync()
        {
            try
            {
                _logger.Info("Refreshing authentication token");
                // TODO: Implement Firebase token refresh
                await Task.Delay(100); // Placeholder
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Token refresh failed: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            try
            {
                _logger.Info($"Password reset requested for: {email}");
                // TODO: Implement Firebase password reset
                await Task.Delay(100); // Placeholder
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Password reset request failed: {ex.Message}", ex);
                return false;
            }
        }

        private static string GetFriendlyFirebaseError(string errorContent, bool isSignup)
        {
            try
            {
                var payload = JsonConvert.DeserializeObject<dynamic>(errorContent);
                var code = payload?["error"]?["message"]?.ToString() ?? string.Empty;

                return code switch
                {
                    "EMAIL_EXISTS" => "This email is already registered. Please sign in instead.",
                    "INVALID_LOGIN_CREDENTIALS" => "Invalid email or password.",
                    "EMAIL_NOT_FOUND" => "No account found for this email.",
                    "INVALID_PASSWORD" => "Invalid email or password.",
                    "USER_DISABLED" => "This account has been disabled.",
                    "WEAK_PASSWORD" => "Password is too weak. Use at least 6 characters.",
                    "TOO_MANY_ATTEMPTS_TRY_LATER" => "Too many attempts. Please try again later.",
                    "OPERATION_NOT_ALLOWED" => "Email/password sign-in is not enabled in Firebase Auth.",
                    "API_KEY_INVALID" => "Firebase API key is invalid. Please check configuration.",
                    _ => isSignup
                        ? "Signup failed. Please check details and try again."
                        : "Login failed. Please check your credentials and try again."
                };
            }
            catch
            {
                return isSignup
                    ? "Signup failed. Please check details and try again."
                    : "Login failed. Please check your credentials and try again.";
            }
        }
    }
}
