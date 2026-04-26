using NetraAI.Desktop.Models;
using NetraAI.Desktop.Utils;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
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
        private readonly string _googleClientId;
        private readonly string _googleClientSecret;
        private readonly string _googleRedirectUri;
        private const string FirebaseAuthBaseUrl = "https://identitytoolkit.googleapis.com/v1";
        private const string FirebaseTokenRefreshUrl = "https://securetoken.googleapis.com/v1/token";
        private const string GoogleAuthUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string GoogleTokenUrl = "https://oauth2.googleapis.com/token";

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
            _googleClientId = firebaseConfig.GoogleClientId ?? string.Empty;
            _googleClientSecret = firebaseConfig.GoogleClientSecret ?? string.Empty;
            _googleRedirectUri = firebaseConfig.GoogleRedirectUri ?? string.Empty;
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

                if (string.IsNullOrWhiteSpace(_googleClientId))
                {
                    throw new InvalidOperationException("Google Sign-In is not configured. Add Firebase:GoogleClientId in appsettings.json.");
                }

                var redirectUri = BuildRedirectUri();
                var listenerPrefix = EnsureListenerPrefix(redirectUri);

                var codeVerifier = GenerateCodeVerifier();
                var codeChallenge = GenerateCodeChallenge(codeVerifier);
                var state = Guid.NewGuid().ToString("N");

                using var listener = new HttpListener();
                listener.Prefixes.Add(listenerPrefix);
                listener.Start();

                var authUrl = BuildGoogleAuthorizationUrl(_googleClientId, redirectUri, codeChallenge, state);
                Process.Start(new ProcessStartInfo
                {
                    FileName = authUrl,
                    UseShellExecute = true
                });

                _logger.Info("Opened browser for Google OAuth consent");

                var code = await WaitForAuthorizationCodeAsync(listener, state);
                var idToken = await ExchangeAuthorizationCodeForIdTokenAsync(code, codeVerifier, redirectUri);

                if (string.IsNullOrWhiteSpace(idToken))
                {
                    throw new InvalidOperationException("Google authentication did not return an ID token.");
                }

                var firebaseUser = await SignInWithFirebaseGoogleAsync(idToken, redirectUri);
                if (firebaseUser == null)
                {
                    _logger.Warning("Google login failed during Firebase token exchange");
                    return null;
                }

                _currentUser = firebaseUser;
                _logger.Info($"Google login successful for user: {_currentUser.Email} (ID: {_currentUser.UserId})");
                return _currentUser;
            }
            catch (InvalidOperationException)
            {
                throw;
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

        private async Task<User?> SignInWithFirebaseGoogleAsync(string googleIdToken, string redirectUri)
        {
            var url = $"{FirebaseAuthBaseUrl}/accounts:signInWithIdp?key={_firebaseApiKey}";
            var postBody = $"id_token={Uri.EscapeDataString(googleIdToken)}&providerId=google.com";

            var request = new
            {
                postBody,
                requestUri = redirectUri,
                returnSecureToken = true,
                returnIdpCredential = true
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
                _logger.Error($"Firebase Google sign-in failed: {response.StatusCode} - {errorContent}");
                throw new InvalidOperationException("Google Sign-In failed in Firebase. Ensure Google provider is enabled in Firebase Auth.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
            if (authResponse == null)
            {
                return null;
            }

            return new User
            {
                UserId = authResponse["localId"]?.ToString() ?? string.Empty,
                Email = authResponse["email"]?.ToString() ?? string.Empty,
                DisplayName = authResponse["displayName"]?.ToString() ?? "Google User",
                ProfilePictureUrl = authResponse["photoUrl"]?.ToString(),
                AuthToken = authResponse["idToken"]?.ToString() ?? string.Empty,
                RefreshToken = authResponse["refreshToken"]?.ToString() ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };
        }

        private async Task<string> WaitForAuthorizationCodeAsync(HttpListener listener, string expectedState)
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
            var contextTask = listener.GetContextAsync();
            var timeoutTask = Task.Delay(Timeout.Infinite, timeoutCts.Token);
            var completed = await Task.WhenAny(contextTask, timeoutTask);

            if (completed != contextTask)
            {
                throw new InvalidOperationException("Google Sign-In timed out. Please try again.");
            }

            var context = await contextTask;
            var queryParams = ParseQueryParams(context.Request.Url?.Query);

            var responseHtml = "<html><body><h3>Authentication complete</h3><p>You can close this window and return to Netra AI.</p></body></html>";
            var responseBytes = Encoding.UTF8.GetBytes(responseHtml);
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = responseBytes.Length;
            await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            context.Response.OutputStream.Close();

            if (queryParams.TryGetValue("error", out var oauthError) && !string.IsNullOrWhiteSpace(oauthError))
            {
                throw new InvalidOperationException($"Google Sign-In canceled or failed: {oauthError}");
            }

            if (!queryParams.TryGetValue("state", out var returnedState) || returnedState != expectedState)
            {
                throw new InvalidOperationException("Invalid OAuth state. Please try Google Sign-In again.");
            }

            if (!queryParams.TryGetValue("code", out var code) || string.IsNullOrWhiteSpace(code))
            {
                throw new InvalidOperationException("Authorization code was not returned by Google.");
            }

            return code;
        }

        private async Task<string?> ExchangeAuthorizationCodeForIdTokenAsync(string code, string codeVerifier, string redirectUri)
        {
            var tokenBody = new Dictionary<string, string>
            {
                ["client_id"] = _googleClientId,
                ["code"] = code,
                ["code_verifier"] = codeVerifier,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = redirectUri
            };

            if (!string.IsNullOrWhiteSpace(_googleClientSecret))
            {
                tokenBody["client_secret"] = _googleClientSecret;
            }

            using var tokenContent = new FormUrlEncodedContent(tokenBody);
            var tokenResponse = await _httpClient.PostAsync(GoogleTokenUrl, tokenContent);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                var tokenErrorContent = await tokenResponse.Content.ReadAsStringAsync();
                _logger.Error($"Google token exchange failed: {tokenResponse.StatusCode} - {tokenErrorContent}");
                var tokenErrorMessage = GetGoogleTokenErrorMessage(tokenErrorContent);
                throw new InvalidOperationException($"Google Sign-In token exchange failed: {tokenErrorMessage}");
            }

            var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();
            var tokenPayload = JsonConvert.DeserializeObject<dynamic>(tokenResponseContent);
            return tokenPayload?["id_token"]?.ToString();
        }

        private static string BuildGoogleAuthorizationUrl(string clientId, string redirectUri, string codeChallenge, string state)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["redirect_uri"] = redirectUri,
                ["response_type"] = "code",
                ["scope"] = "openid email profile",
                ["code_challenge"] = codeChallenge,
                ["code_challenge_method"] = "S256",
                ["state"] = state,
                ["access_type"] = "offline",
                ["prompt"] = "select_account"
            };

            var encoded = string.Join("&", queryParams.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            return $"{GoogleAuthUrl}?{encoded}";
        }

        private static string GenerateCodeVerifier()
        {
            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Base64UrlEncode(bytes);
        }

        private static string GenerateCodeChallenge(string codeVerifier)
        {
            var bytes = Encoding.ASCII.GetBytes(codeVerifier);
            var hash = SHA256.HashData(bytes);
            return Base64UrlEncode(hash);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", string.Empty);
        }

        private static Dictionary<string, string> ParseQueryParams(string? query)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(query))
            {
                return result;
            }

            var trimmed = query.StartsWith("?") ? query[1..] : query;
            var pairs = trimmed.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var parts = pair.Split('=', 2);
                var key = Uri.UnescapeDataString(parts[0]);
                var value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;
                result[key] = value;
            }

            return result;
        }

        private string BuildRedirectUri()
        {
            if (!string.IsNullOrWhiteSpace(_googleRedirectUri))
            {
                return EnsureRedirectUriFormat(_googleRedirectUri);
            }

            var redirectPort = GetAvailableTcpPort();
            return $"http://127.0.0.1:{redirectPort}/";
        }

        private static string EnsureRedirectUriFormat(string uri)
        {
            return uri.EndsWith("/", StringComparison.Ordinal) ? uri : $"{uri}/";
        }

        private static string EnsureListenerPrefix(string redirectUri)
        {
            return redirectUri.EndsWith("/", StringComparison.Ordinal) ? redirectUri : $"{redirectUri}/";
        }

        private static string GetGoogleTokenErrorMessage(string errorContent)
        {
            try
            {
                var payload = JsonConvert.DeserializeObject<dynamic>(errorContent);
                var error = payload?["error"]?.ToString();
                var description = payload?["error_description"]?.ToString();

                if (!string.IsNullOrWhiteSpace(description))
                {
                    return description;
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    return error;
                }
            }
            catch
            {
                // Fall through to generic message.
            }

            return "Check OAuth client configuration.";
        }

        private static int GetAvailableTcpPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
