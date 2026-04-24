# 🚀 Netra AI - Desktop Copilot Assistant

A professional Windows desktop application that acts as your AI copilot. Capture screen content, extract text via OCR, and get intelligent answers from Google Gemini API—all through a beautiful, always-on-top floating interface.

## ✨ Features

- 🔐 **Secure Authentication** - Email/password and Google login via Firebase
- 🎯 **Explicit Permissions** - Transparent permission system for screen, microphone, and background access
- 📸 **Smart Screen Capture** - Full screen or region-based capture with OCR text extraction
- 🧠 **AI-Powered Insights** - Real-time assistance using Google Gemini API
- 🎨 **Beautiful UI** - Modern dark-theme interface with floating widget
- ⌨️ **Global Hotkeys** - Quick access with customizable keyboard shortcuts
- 💾 **Chat History** - Persistent conversation storage and context management

## 🏗️ Tech Stack

- **Frontend:** C# + WPF (.NET 8.0)
- **Authentication:** Firebase Authentication
- **AI:** Google Gemini API
- **OCR:** Tesseract
- **Database:** SQLite
- **Logging:** Serilog
- **DI:** Microsoft.Extensions.DependencyInjection

## 📋 System Requirements

- Windows 10 or later
- .NET 8.0 Runtime
- 300MB RAM (minimum)
- 100MB Disk Space

## 🚀 Getting Started

### Prerequisites

- Visual Studio 2022 or Visual Studio Code
- .NET 8.0 SDK or later
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/netra-ai.git
   cd netra-ai
   ```

2. **Open the solution**
   ```bash
   dotnet open NetraAI.sln
   ```

3. **Configure API Keys**
   - Copy `docs/API_KEYS.md.example` to `docs/API_KEYS.md`
   - Fill in your Firebase and Gemini API credentials

4. **Restore dependencies**
   ```bash
   dotnet restore
   ```

5. **Build the project**
   ```bash
   dotnet build
   ```

6. **Run the application**
   ```bash
   dotnet run --project NetraAI.Desktop
   ```

## 📖 Project Structure

```
NetraAI/
├── NetraAI.Desktop/          # Main WPF Application
│   ├── Views/                # XAML Windows
│   ├── Services/             # Business Logic
│   ├── Models/               # Data Models
│   ├── Utils/                # Utilities & Helpers
│   └── Resources/            # Styles & Assets
├── NetraAI.Tests/            # Unit Tests
├── docs/                     # Documentation
└── README.md
```

## 🔑 API Configuration

See [docs/API_KEYS.md](docs/API_KEYS.md) for detailed setup instructions:

### Required APIs
- Firebase Authentication
- Google Generative AI (Gemini)
- Tesseract OCR

## 📚 Development

### Project Phases

See [PROJECT_PHASES.md](../PROJECT_PHASES.md) for detailed implementation roadmap:

- **Phase 1:** Foundation & Setup ✅
- **Phase 2:** Screen Capture & OCR
- **Phase 3:** AI Integration
- **Phase 4:** UI Polish & Overlay
- **Phase 5:** Testing & Release

### Running Tests

```bash
dotnet test
```

### Building for Production

```bash
dotnet publish -c Release -r win-x64
```

## 🐛 Known Issues

- None yet (Just getting started!)

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

- **Issues:** [GitHub Issues](https://github.com/jayeshwani7/netra-ai/issues)
- **Email:** support@netraai.com
- **Documentation:** [Full Docs](docs/)

## 🙏 Acknowledgments

- Google Gemini API for AI capabilities
- Firebase for authentication
- Tesseract for OCR functionality
- WPF community for support

---

**Status:** 🔵 Active Development  
**Version:** 1.0.0-alpha  
