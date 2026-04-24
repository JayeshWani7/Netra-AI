# 🚀 AI Desktop Copilot - Project Phases & Requirements

**Project Name:** Netra AI - Desktop Copilot Assistant  
**Target OS:** Windows  
**Tech Stack:** C# + WPF, Firebase Auth, Gemini API, Tesseract OCR  
**Status:** Planning Phase  
**Last Updated:** April 24, 2026

---

## 📋 Table of Contents
1. [Project Overview](#project-overview)
2. [System Architecture](#system-architecture)
3. [Phase Breakdown](#phase-breakdown)
4. [Detailed Requirements](#detailed-requirements)
5. [Dependency Map](#dependency-map)
6. [Success Criteria](#success-criteria)

---

## 🎯 Project Overview

### Vision
Build a professional, installable Windows desktop application that acts as an AI copilot. Users can capture screen content, extract text via OCR, and get intelligent answers from Google Gemini API—all through a beautiful, always-on-top floating interface.

### Key Differentiators
- ✅ Explicit permission system (transparent, not invasive)
- ✅ Cloud-backed authentication (Firebase)
- ✅ Real-time AI assistance from screen context
- ✅ Professional-grade UI/UX
- ✅ Background execution capability
- ✅ Installable & auto-updatable

### Target Users
- Developers needing quick answers from code
- Designers looking for design insights
- Students studying complex content
- Knowledge workers researching

---

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────┐
│         Windows Desktop App (WPF)               │
├─────────────────────────────────────────────────┤
│  Floating Overlay UI                            │
│  ├─ Hotkey Listener                            │
│  ├─ Always-on-top Widget                       │
│  └─ Chat Interface                             │
├─────────────────────────────────────────────────┤
│  Core Services Layer                            │
│  ├─ Auth Service (Firebase)                    │
│  ├─ Screen Capture Service                     │
│  ├─ OCR Service (Tesseract)                    │
│  ├─ Permission Manager                        │
│  └─ Hotkey Manager                             │
├─────────────────────────────────────────────────┤
│  AI & Data Layer                                │
│  ├─ Gemini API Client                          │
│  ├─ Local Cache (Recent captures)              │
│  └─ Chat History Storage                       │
├─────────────────────────────────────────────────┤
│  System Integration                             │
│  ├─ Windows Registry (Settings)                │
│  ├─ File System (Logs, Cache)                  │
│  └─ Startup Manager                            │
└─────────────────────────────────────────────────┘
```

---

## 📅 Phase Breakdown

### 🔵 Phase 1: Foundation & Setup (Week 1)
**Goal:** Basic app structure, authentication, and clean UI foundation  
**Duration:** 5 days  

### 🟢 Phase 2: Screen Capture & OCR (Week 2)
**Goal:** Core functionality - capture screen and extract text  
**Duration:** 5 days  

### 🟡 Phase 3: AI Integration (Week 3)
**Goal:** Connect Gemini API and process queries  
**Duration:** 5 days  

### 🟣 Phase 4: UI Polish & Overlay (Week 4)
**Goal:** Floating widget, hotkeys, final UI  
**Duration:** 5 days  

### 🔴 Phase 5: Refinement & Release (Week 5)
**Goal:** Testing, installer, documentation, deployment  
**Duration:** 5 days  

---

## 📝 Detailed Requirements

---

## PHASE 1️⃣: Foundation & Setup (Week 1)

### 1.1 Project Structure
**Deliverable:** Complete folder structure and project setup

```
NetraAI/
├── NetraAI.sln (Main Solution)
├── NetraAI.Desktop/
│   ├── NetraAI.Desktop.csproj (WPF App)
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── Properties/
│   │   ├── AssemblyInfo.cs
│   │   └── Settings.settings
│   ├── Views/
│   │   ├── LoginWindow.xaml
│   │   ├── LoginWindow.xaml.cs
│   │   ├── MainWindow.xaml
│   │   ├── MainWindow.xaml.cs
│   │   ├── PermissionsWindow.xaml
│   │   ├── PermissionsWindow.xaml.cs
│   │   └── OverlayWindow.xaml
│   │   └── OverlayWindow.xaml.cs
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── PermissionService.cs
│   │   ├── NavigationService.cs
│   │   └── IAuthService.cs (Interface)
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Permission.cs
│   │   ├── AppConfig.cs
│   │   └── ChatMessage.cs
│   ├── Utils/
│   │   ├── Constants.cs
│   │   ├── Logger.cs
│   │   └── JsonHelper.cs
│   └── Resources/
│       ├── Styles.xaml
│       └── Assets/ (logos, icons)
├── NetraAI.Tests/
│   ├── NetraAI.Tests.csproj
│   └── AuthServiceTests.cs
├── docs/
│   └── API_KEYS.md (Firebase, Gemini)
└── README.md
```

### 1.2 Firebase Setup
**Deliverable:** Firebase project configured and integrated

- [ ] Create Firebase project at console.firebase.google.com
- [ ] Enable Email/Password authentication
- [ ] Enable Google Sign-In
- [ ] Download Firebase config file
- [ ] Store API keys in secure location (environment variables)
- [ ] Install Firebase NuGet package: `FirebaseAuthentication.CSharp`
- [ ] Test Firebase connection with dummy user

**Files to Create:**
- `config/firebase-config.json`
- `Services/AuthService.cs` (Firebase integration)

**Key Code Structure:**
```csharp
public class AuthService : IAuthService
{
    private FirebaseAuth _firebaseAuth;
    
    public async Task<User> LoginAsync(string email, string password)
    {
        // Firebase login logic
    }
    
    public async Task<User> SignupAsync(string email, string password)
    {
        // Firebase signup logic
    }
    
    public async Task LogoutAsync()
    {
        // Logout logic
    }
}
```

### 1.3 Login UI
**Deliverable:** Professional login window

**Requirements:**
- [ ] Email input field
- [ ] Password input field
- [ ] "Sign Up" option
- [ ] "Forgot Password" link
- [ ] Google Sign-In button
- [ ] Loading indicator
- [ ] Error message display
- [ ] Remember me checkbox (optional)
- [ ] Clean, modern design (minimal, dark theme friendly)

**XAML Structure:**
```xaml
<Window x:Class="NetraAI.Desktop.Views.LoginWindow">
    <!-- Login form with email, password, buttons -->
</Window>
```

### 1.4 Permission Model
**Deliverable:** Permission tracking system

**Data Model:**
```csharp
public class Permission
{
    public string PermissionId { get; set; }
    public string UserId { get; set; }
    public bool ScreenAccess { get; set; }
    public bool MicrophoneAccess { get; set; }
    public bool BackgroundRunning { get; set; }
    public bool ClipboardAccess { get; set; }
    public DateTime GrantedAt { get; set; }
    public bool IsExplicitlyRequested { get; set; }
}
```

**Storage:**
- [ ] Store in local JSON: `%AppData%/NetraAI/permissions.json`
- [ ] Sync with cloud (optional)

### 1.5 Basic Settings Management
**Deliverable:** Settings system

**Requirements:**
- [ ] Store app settings locally
- [ ] Store user preferences (theme, hotkey, etc.)
- [ ] Settings file format: JSON
- [ ] Default values defined
- [ ] Settings accessible via `AppConfig` class

**Settings Structure:**
```json
{
  "user_id": "",
  "auth_token": "",
  "theme": "dark",
  "hotkey": "Ctrl+Alt+A",
  "auto_start": false,
  "permissions": {
    "screen_access": false,
    "mic_access": false,
    "background_running": false
  }
}
```

### 1.6 Logging System
**Deliverable:** Application logging

- [ ] Create `Logger.cs` utility
- [ ] Log to file: `%AppData%/NetraAI/logs/`
- [ ] Include timestamp, log level, message
- [ ] Rotate logs (keep last 7 days)

### 1.7 NuGet Dependencies
**Deliverable:** All required packages installed

```
FirebaseAuthentication.CSharp (Latest)
Newtonsoft.Json (13.0.1+)
log4net (2.0.14+)
```

### 1.8 Code Architecture
**Deliverable:** Clean, testable structure

- [ ] MVVM pattern ready (no code-behind, or minimal)
- [ ] Dependency injection container (SimpleInjector or similar)
- [ ] Unit test project created
- [ ] Interface-based services for testability

### 1.9 CI/CD Setup (Optional but recommended)
**Deliverable:** Automated build pipeline

- [ ] GitHub Actions workflow for build
- [ ] Auto-run tests
- [ ] Build artifacts ready

---

### ✅ Phase 1 Success Criteria
- [x] User can launch app and see login screen
- [x] User can sign up with email/password
- [x] User can login with existing credentials
- [x] User can login with Google
- [x] Sessions persist (token saved)
- [x] Logout clears session
- [x] Settings file created and saved
- [x] No hardcoded API keys
- [x] App structure is clean and testable

---

## PHASE 2️⃣: Screen Capture & OCR (Week 2)

### 2.1 Permissions Window
**Deliverable:** Beautiful permissions prompt

**UI Requirements:**
- [ ] List of requested permissions with descriptions
- [ ] Toggle switches for each permission
- [ ] "Grant All" and "Deny All" buttons
- [ ] "Learn More" links for each permission
- [ ] Clear explanation why each is needed
- [ ] Professional, non-threatening design

**Permissions to Request:**
1. Screen Capture
2. Microphone (optional)
3. Background Execution
4. Clipboard Access (optional)

**XAML Layout:**
```xaml
<StackPanel>
    <TextBlock Text="App Permissions" FontSize="24" FontWeight="Bold"/>
    <TextBlock Text="To use Netra AI, we need:"/>
    
    <!-- Permission Items -->
    <PermissionItem Icon="..." Name="Screen Capture" 
        Description="To see what's on your screen"/>
    <PermissionItem Icon="..." Name="Microphone" 
        Description="For voice input (optional)"/>
</StackPanel>
```

### 2.2 Screen Capture Service
**Deliverable:** Core screen capture functionality

**Features:**
- [ ] Capture full screen
- [ ] Capture specific region/window
- [ ] Save to memory (not disk)
- [ ] High performance (minimal lag)
- [ ] Support multiple monitors

**Implementation Approaches:**

**Option A: Simple (Graphics.CopyFromScreen)**
```csharp
public Bitmap CaptureScreen()
{
    var screenSize = Screen.PrimaryScreen.Bounds;
    var bitmap = new Bitmap(screenSize.Width, screenSize.Height);
    var graphics = Graphics.FromImage(bitmap);
    graphics.CopyFromScreen(0, 0, 0, 0, screenSize.Size);
    return bitmap;
}
```

**Option B: Advanced (Desktop Duplication API - DXGI)**
- [ ] Use Direct3D for better performance
- [ ] Lower CPU usage
- [ ] Recommended for production

**Code Structure:**
```csharp
public interface IScreenCaptureService
{
    Task<Bitmap> CaptureFullScreenAsync();
    Task<Bitmap> CaptureRegionAsync(Rectangle region);
    Task<Bitmap> CaptureWindowAsync(IntPtr hwnd);
}

public class ScreenCaptureService : IScreenCaptureService
{
    // Implementation
}
```

### 2.3 Region Selection Tool
**Deliverable:** Visual region picker

**Features:**
- [ ] Click & drag to select region
- [ ] Visual crosshair cursor
- [ ] Highlight selected area
- [ ] Cancel with ESC
- [ ] Show coordinates
- [ ] Snap to windows (optional)

### 2.4 Tesseract OCR Integration
**Deliverable:** Text extraction engine

**Setup:**
- [ ] Install Tesseract NuGet: `Tesseract` (5.2.0+)
- [ ] Download Tesseract data files (eng)
- [ ] Bundle with app installer
- [ ] Set language packs

**Code Structure:**
```csharp
public interface IOCRService
{
    Task<string> ExtractTextAsync(Bitmap screenshot);
    Task<string> ExtractTextAsync(string imagePath);
    Task<OCRResult> ExtractTextWithConfidenceAsync(Bitmap screenshot);
}

public class OCRService : IOCRService
{
    private TesseractEngine _engine;
    
    public async Task<string> ExtractTextAsync(Bitmap screenshot)
    {
        // OCR logic
    }
}
```

**Advanced Features (Phase 2+):**
- [ ] Language detection
- [ ] Confidence scoring
- [ ] Layout preservation
- [ ] Table detection

### 2.5 Text Processing Pipeline
**Deliverable:** Clean extracted text

**Processing Steps:**
1. Extract raw text from OCR
2. Remove extra whitespace
3. Fix common OCR errors
4. Remove non-printable characters
5. Format structured data
6. Cache result

**Code:**
```csharp
public class TextProcessor
{
    public static string CleanOCRText(string rawText)
    {
        // Remove extra spaces
        // Fix common errors (rn → m, etc.)
        // Trim
        return cleanedText;
    }
}
```

### 2.6 Caching System
**Deliverable:** Store recent captures

**Requirements:**
- [ ] Store last 50 screenshots in memory
- [ ] Store extracted text
- [ ] Include timestamp
- [ ] Implement LRU cache
- [ ] Optional: Save cache to disk

**Data Model:**
```csharp
public class CacheEntry
{
    public Guid Id { get; set; }
    public Bitmap Screenshot { get; set; }
    public string ExtractedText { get; set; }
    public DateTime CapturedAt { get; set; }
    public double OCRConfidence { get; set; }
}
```

### 2.7 Performance Optimization
**Deliverable:** Fast capture & OCR

- [ ] Measure capture time (target: < 500ms)
- [ ] Measure OCR time (target: < 2s)
- [ ] Profile memory usage
- [ ] Optimize image compression
- [ ] Consider async/await throughout

### 2.8 Error Handling
**Deliverable:** Graceful failure

- [ ] Handle permission denied scenarios
- [ ] Handle OCR failures
- [ ] Handle large images
- [ ] Log all errors
- [ ] Show user-friendly error messages

---

### ✅ Phase 2 Success Criteria
- [x] User sees permission prompt after login
- [x] Can grant individual permissions
- [x] Permissions persist
- [x] Can capture full screen with hotkey
- [x] Can manually select region to capture
- [x] OCR extracts text from screenshots
- [x] Text is cleaned and formatted
- [x] Recent captures are cached
- [x] No permission errors crash app
- [x] Performance is acceptable (< 3s total)

---

## PHASE 3️⃣: AI Integration (Week 3)

### 3.1 Gemini API Setup
**Deliverable:** Gemini integration ready

**Setup Steps:**
- [ ] Create Google Cloud project
- [ ] Enable Generative Language API
- [ ] Create API key (restrict to desktop app)
- [ ] Store API key securely (encrypted in config)
- [ ] Install NuGet: `Google.Generativeai` (latest)

**Security:**
- [ ] Never hardcode API key
- [ ] Use environment variables
- [ ] Consider backend proxy for production

### 3.2 AI Service Layer
**Deliverable:** Query handler

**Code Structure:**
```csharp
public interface IAIService
{
    Task<AIResponse> QueryAsync(string query, string screenshotText = null);
    Task<AIResponse> QueryWithImageAsync(string query, Bitmap screenshot);
    Task<string> SummarizeScreenAsync(string screenText);
    Task<string> AnswerQuestionAsync(string question, string context);
}

public class GeminiService : IAIService
{
    private readonly string _apiKey;
    private readonly GenerativeModel _model;
    
    public async Task<AIResponse> QueryAsync(string query, string screenshotText)
    {
        // Call Gemini API
    }
}
```

### 3.3 Chat History Management
**Deliverable:** Store conversations

**Data Model:**
```csharp
public class ChatMessage
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string Role { get; set; } // "user" or "assistant"
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid? ScreenshotId { get; set; }
    public double? Confidence { get; set; }
}

public class ChatSession
{
    public Guid SessionId { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastMessageAt { get; set; }
    public List<ChatMessage> Messages { get; set; }
}
```

**Storage:**
- [ ] Store locally: `%AppData%/NetraAI/chat_history.db` (SQLite)
- [ ] Optional: Sync to cloud
- [ ] Keep last 100 sessions locally

### 3.4 Context Management
**Deliverable:** Multi-turn conversation support

**Features:**
- [ ] Maintain conversation context
- [ ] Send previous messages to Gemini
- [ ] Limit context window (last 10 messages)
- [ ] User can clear history
- [ ] Sessions persist across restarts

**Prompt Engineering:**
```csharp
private string BuildSystemPrompt()
{
    return @"You are Netra AI, a helpful desktop assistant. 
    You help users understand what's on their screen and answer questions.
    Be concise, accurate, and helpful.
    If you see code, explain it simply.
    If you see images/UI, describe what you see.";
}
```

### 3.5 Response Formatting
**Deliverable:** Parse and display AI responses

**Features:**
- [ ] Handle markdown in responses
- [ ] Syntax highlighting for code blocks
- [ ] Format lists properly
- [ ] Handle long responses (pagination)
- [ ] Copy-to-clipboard functionality

### 3.6 Error Handling & Fallbacks
**Deliverable:** Robust API integration

- [ ] Handle API rate limiting
- [ ] Retry logic with exponential backoff
- [ ] Timeout handling (default 30s)
- [ ] Offline fallback (show cached responses)
- [ ] User-friendly error messages

### 3.7 Cost Management
**Deliverable:** Track API usage

**Monitoring:**
- [ ] Log all API calls
- [ ] Track tokens used
- [ ] Warn if usage is high
- [ ] Optional: Daily/monthly limit

**Code:**
```csharp
public class UsageTracker
{
    public int TokensUsed { get; set; }
    public int RequestsToday { get; set; }
    public DateTime ResetAt { get; set; }
}
```

### 3.8 Advanced Features (Optional)
- [ ] Vision mode: Send actual screenshot image to Gemini
- [ ] Multi-modal prompts
- [ ] Custom system prompts per use case
- [ ] Response streaming (for long answers)

---

### ✅ Phase 3 Success Criteria
- [x] Gemini API key configured securely
- [x] User can type query in chat window
- [x] Gemini returns answers
- [x] Chat history saved locally
- [x] Previous messages provide context
- [x] API errors handled gracefully
- [x] Responses formatted nicely
- [x] No API key exposed in logs
- [x] Usage tracked

---

## PHASE 4️⃣: UI Polish & Floating Overlay (Week 4)

### 4.1 Main Dashboard Window
**Deliverable:** Central hub UI

**Features:**
- [ ] Chat history sidebar
- [ ] Current session display
- [ ] Settings access
- [ ] User profile dropdown
- [ ] Logout button
- [ ] Help & documentation

**Design Inspiration:**
- Modern, clean (similar to Arc browser, Notion)
- Dark theme by default
- Smooth animations
- Responsive layout

### 4.2 Floating Overlay Window
**Deliverable:** Always-on-top widget

**Features:**
- [ ] Appears as draggable widget
- [ ] Show/hide with hotkey
- [ ] Collapse/expand
- [ ] Quick chat input
- [ ] Show loading state
- [ ] Minimize to system tray
- [ ] Configurable size & position
- [ ] Stays on top of other windows

**XAML:**
```xaml
<Window x:Class="NetraAI.Desktop.Views.OverlayWindow"
    WindowStyle="None"
    AllowsTransparency="True"
    Background="Transparent"
    Topmost="True"
    ShowInTaskbar="False"
    ResizeMode="CanResizeWithGrip">
    <!-- Overlay content -->
</Window>
```

**Styling:**
- [ ] Dark semi-transparent background
- [ ] Rounded corners
- [ ] Shadow effect
- [ ] Glassmorphism (optional)

### 4.3 Hotkey System
**Deliverable:** Global hotkey management

**Features:**
- [ ] Register global hotkey (Ctrl+Alt+A default)
- [ ] Customizable hotkey
- [ ] Works even when app is minimized
- [ ] Multiple hotkey actions:
  - `Ctrl+Alt+A`: Open/close overlay
  - `Ctrl+Alt+S`: Capture screen
  - `Ctrl+Alt+R`: Select region

**Implementation:**
```csharp
public class HotkeyManager
{
    public void RegisterHotkey(int id, ModifierKeys modifiers, 
        Keys keys, Action callback)
    {
        // Register with Windows API
        // Call callback on trigger
    }
}
```

### 4.4 System Tray Integration
**Deliverable:** App lives in system tray

**Features:**
- [ ] Tray icon
- [ ] Right-click context menu
- [ ] "Open Dashboard" option
- [ ] "Settings" option
- [ ] "Exit" option
- [ ] Show unread messages badge
- [ ] Double-click to toggle overlay

### 4.5 Chat Interface Refinement
**Deliverable:** Beautiful message display

**Features:**
- [ ] User messages aligned right (light)
- [ ] AI responses aligned left (darker)
- [ ] Timestamps on hover
- [ ] Copy button on AI responses
- [ ] Regenerate response button
- [ ] Reaction buttons (👍 👎)
- [ ] Rich text formatting

### 4.6 Settings Window
**Deliverable:** User configuration

**Settings Available:**
- [ ] Theme (dark/light)
- [ ] Hotkey customization
- [ ] Auto-start on login
- [ ] OCR language
- [ ] Default capture mode
- [ ] Privacy settings
- [ ] About & version info

### 4.7 Theme System
**Deliverable:** Consistent styling

**Requirements:**
- [ ] Define color palette
- [ ] Dark theme (default)
- [ ] Light theme
- [ ] Accent colors
- [ ] Apply globally via resources

**Colors (Dark Theme):**
- Background: `#1a1a1a`
- Surface: `#2d2d2d`
- Primary: `#7c3aed` (violet)
- Text: `#f0f0f0`
- Error: `#ef4444`

### 4.8 Animations & Transitions
**Deliverable:** Smooth, polished feel

- [ ] Window fade in/out
- [ ] Smooth overlay slide
- [ ] Message fade in
- [ ] Button hover effects
- [ ] Loading spinner animation
- [ ] Transition between screens

### 4.9 Accessibility
**Deliverable:** Inclusive design

- [ ] High contrast mode support
- [ ] Keyboard navigation
- [ ] Tab order correct
- [ ] Screen reader support
- [ ] Font size scaling

### 4.10 Responsive Layout
**Deliverable:** Works on different screen sizes

- [ ] Works on 1920x1080+
- [ ] Mobile-friendly chat (if applicable)
- [ ] Scales properly
- [ ] Overlay resizable

---

### ✅ Phase 4 Success Criteria
- [x] Professional-looking dashboard
- [x] Floating overlay appears on hotkey press
- [x] Overlay draggable and resizable
- [x] Hotkeys configurable in settings
- [x] System tray integration working
- [x] Theme toggle working
- [x] Chat messages beautifully formatted
- [x] All UI consistent and polished
- [x] Smooth animations throughout
- [x] No lag in overlay rendering

---

## PHASE 5️⃣: Refinement, Testing & Release (Week 5)

### 5.1 Comprehensive Testing
**Deliverable:** Quality assurance complete

**Unit Tests:**
- [ ] AuthService tests (login, signup, logout)
- [ ] PermissionService tests
- [ ] ScreenCaptureService tests
- [ ] OCRService tests
- [ ] TextProcessor tests
- [ ] HotkeyManager tests

**Target:** > 80% code coverage

**Integration Tests:**
- [ ] Full auth flow
- [ ] Screen capture → OCR → AI pipeline
- [ ] Chat history persistence
- [ ] Settings save/load

**Manual Testing Checklist:**
- [ ] [x] Fresh install → Login → Permissions → Chat
- [ ] [x] All hotkeys work
- [ ] [x] Permissions enforce correctly
- [ ] [x] OCR extracts text accurately
- [ ] [x] AI responses are relevant
- [ ] [x] Chat history persists across restarts
- [ ] [x] No memory leaks
- [ ] [x] No crashes
- [ ] [x] Settings persist
- [ ] [x] Logout works cleanly

**Performance Testing:**
- [ ] Capture time: < 500ms
- [ ] OCR time: < 2s
- [ ] AI response time: < 10s
- [ ] Memory usage: < 300MB
- [ ] CPU usage: < 20% idle
- [ ] Battery drain (if applicable)

### 5.2 Bug Fixes & Stability
**Deliverable:** Production-ready stability

- [ ] Review and fix all known bugs
- [ ] Handle edge cases
- [ ] Test on multiple Windows versions (10, 11)
- [ ] Test with different hardware
- [ ] Stress test (rapid captures, etc.)

### 5.3 Documentation
**Deliverable:** User & developer docs

**User Documentation:**
- [ ] README with features overview
- [ ] Installation guide
- [ ] Quick start guide
- [ ] FAQ
- [ ] Privacy policy
- [ ] Terms of service

**Developer Documentation:**
- [ ] Architecture overview
- [ ] How to build from source
- [ ] API documentation
- [ ] Contributing guidelines

### 5.4 Installer Creation
**Deliverable:** Professional installer

**Tools:**
- [ ] Use WiX Toolset or NSIS
- [ ] Create .msi or .exe installer
- [ ] Configure install options:
  - [ ] Desktop shortcut
  - [ ] Auto-start checkbox
  - [ ] Run after install

**Installer Features:**
- [ ] Uninstaller
- [ ] Version upgrade support
- [ ] Check system requirements
- [ ] Install dependencies (if any)

### 5.5 Code Signing
**Deliverable:** Secure executable

- [ ] Obtain code signing certificate
- [ ] Sign .exe and .msi
- [ ] Sign auto-updater
- [ ] Prevents SmartScreen warnings

### 5.6 Auto-Update System
**Deliverable:** Easy updates for users

**Implementation:**
- [ ] Check for updates on startup
- [ ] Download & apply silently
- [ ] Notify user of update
- [ ] Graceful restart

**Options:**
- [ ] Squirrel.Windows (for Electron-style updates)
- [ ] Custom update service
- [ ] Windows Update integration

### 5.7 Analytics & Monitoring
**Deliverable:** Usage insights

**Tracking (Privacy-Respecting):**
- [ ] Anonymous usage metrics
- [ ] Error reporting
- [ ] Feature usage
- [ ] Crash logs (opt-in)
- [ ] Never track actual screenshot content

**Tools:**
- [ ] Application Insights (Azure)
- [ ] Sentry.io
- [ ] Custom backend

### 5.8 Security Audit
**Deliverable:** Security hardening

**Checklist:**
- [ ] No hardcoded secrets
- [ ] API keys encrypted
- [ ] Secure local storage
- [ ] No sensitive data in logs
- [ ] Authentication token handling
- [ ] Rate limiting on API
- [ ] Input validation
- [ ] HTTPS for all network calls

### 5.9 Performance Optimization
**Deliverable:** Lightning-fast app

- [ ] Profile startup time (target: < 3s)
- [ ] Optimize OCR performance
- [ ] Cache optimization
- [ ] Memory leak fixes
- [ ] UI responsiveness

### 5.10 Release Checklist
**Deliverable:** Ready for public release

- [ ] All tests passing
- [ ] Code review complete
- [ ] Documentation complete
- [ ] Installer tested
- [ ] Security audit passed
- [ ] Performance targets met
- [ ] Hotfixes for critical bugs
- [ ] Version number updated
- [ ] Changelog written
- [ ] Marketing materials ready

### 5.11 Deployment
**Deliverable:** App available to users

**Channels:**
- [ ] GitHub Releases
- [ ] Direct download from website
- [ ] Optional: Microsoft Store
- [ ] Optional: Installers marketplace

**Launch Activities:**
- [ ] Write release notes
- [ ] Create demo video
- [ ] Share on forums/communities
- [ ] Reddit/HackerNews posts
- [ ] GitHub stars campaign

---

### ✅ Phase 5 Success Criteria
- [x] 80%+ unit test coverage
- [x] All critical bugs fixed
- [x] User documentation complete
- [x] Professional installer created
- [x] Executable code-signed
- [x] Auto-update working
- [x] Security audit passed
- [x] Performance meets targets
- [x] Version 1.0.0 released
- [x] Users can install and use app

---

## 🔗 Dependency Map

```
Phase 1 (Foundation)
├── Firebase Setup ✓
├── Login UI ✓
└── Settings System ✓
    │
    ├─→ Phase 2 (Capture & OCR)
    │   ├── Permissions Window
    │   ├── Screen Capture Service
    │   ├── Tesseract OCR
    │   └── Text Processing
    │       │
    │       ├─→ Phase 3 (AI Integration)
    │       │   ├── Gemini API
    │       │   ├── Chat History
    │       │   └── AI Service
    │       │       │
    │       │       └─→ Phase 4 (UI & Overlay)
    │       │           ├── Floating Overlay
    │       │           ├── Hotkey System
    │       │           ├── Dashboard UI
    │       │           └── Settings UI
    │       │               │
    │       │               └─→ Phase 5 (Release)
    │       │                   ├── Testing
    │       │                   ├── Installer
    │       │                   ├── Documentation
    │       │                   └── Release
```

**Critical Path:** Phase 1 → Phase 2 → Phase 3 → Phase 4 → Phase 5

**Can Parallelize:**
- Phase 4 UI work can start before Phase 3 AI is complete
- Documentation can be drafted during Phase 4

---

## 📊 Success Criteria by Phase

| Phase | Criteria | Status |
|-------|----------|--------|
| 1 | Login & settings working | ❌ Pending |
| 2 | Screen capture & OCR functional | ❌ Pending |
| 3 | Gemini integration complete | ❌ Pending |
| 4 | Beautiful UI & overlay working | ❌ Pending |
| 5 | Released & tested | ❌ Pending |

---

## 🎯 KPIs & Metrics

### Product Metrics
- **User Growth:** Target 100+ users by end of Q2
- **DAU:** Daily active users
- **Feature Adoption:** % users using OCR vs direct chat
- **Error Rate:** < 1% of sessions
- **Crash Rate:** < 0.1%

### Performance Metrics
- **Startup Time:** < 3 seconds
- **Capture + OCR:** < 2.5 seconds
- **AI Response:** < 10 seconds (including network)
- **Memory Footprint:** < 300MB
- **CPU (idle):** < 5%

### Quality Metrics
- **Test Coverage:** > 80%
- **Code Review Completions:** 100% of PRs
- **Bug Fix Time:** < 24 hours for critical
- **User Satisfaction:** > 4.5/5.0 stars

---

## 📚 Resources & References

### Documentation
- [Firebase Auth Docs](https://firebase.google.com/docs/auth)
- [Gemini API Guide](https://ai.google.dev/tutorials/python_quickstart)
- [Tesseract OCR](https://github.com/UB-Mannheim/tesseract/wiki)
- [WPF Best Practices](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)

### Tools & Libraries
- **WPF:** XAML, Code-behind, Binding
- **Testing:** NUnit, xUnit, Moq
- **Logging:** Serilog, NLog
- **DI:** Autofac, SimpleInjector
- **API Client:** HttpClient, Refit
- **Database:** SQLite, Dapper

### Third-Party APIs
- Google Gemini API (AI)
- Firebase (Auth)
- Tesseract (OCR)

---

## 🚨 Risk Assessment

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|-----------|
| OCR accuracy poor | High | Medium | Use multiple engines, train on data |
| Gemini API rate limits | High | Medium | Implement queuing, local cache |
| Permission denial | Medium | Medium | Graceful degradation |
| Screen capture slow | High | Low | Optimize DXGI, profile early |
| User adoption low | High | High | Marketing, community engagement |
| Security breach | Critical | Low | Audit regularly, use secrets manager |

---

## 📞 Contact & Support

**Project Lead:** [Your Name]  
**GitHub:** [Repository Link]  
**Discord:** [Community Link]  
**Email:** support@netraai.com  

---

## 📋 Revision History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | Apr 24, 2026 | Initial project plan |
| - | - | - |

---

## 🎓 Resume Impact

> Developed Netra AI, a production-grade Windows desktop application featuring user authentication via Firebase, explicit permission management system, OCR-based text extraction using Tesseract, and real-time AI assistance powered by Google Gemini API. Implemented floating overlay UI with global hotkey integration, multi-turn chat history, and comprehensive caching system. Built with C# + WPF following MVVM architecture, 80%+ unit test coverage, and professional installer with code signing. Successfully shipped v1.0 with 100+ users and 4.5+ star ratings.

---

**Status:** 🔵 Planning Phase  
**Next:** Phase 1 Kickoff  
**Updated:** April 24, 2026
