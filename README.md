# OwChatAssistant

A lightweight real-time toxicity prevention overlay for competitive games like Overwatch.

OwChatAssistant detects when the in-game chat is open, analyzes the message being written, and warns or blocks potentially toxic messages before they are sent.

The project is inspired by HUD-style overlays and tools like Grammarly, but focused on encouraging healthier communication during gameplay.

---

## Features

- 🎮 Real-time in-game overlay
- 🧠 Toxicity detection before sending messages
- 💬 Chat-open detection using visual and cursor heuristics
- ✨ Custom sci-fi HUD inspired UI
- 🌈 Animated overlay with gradients and glow effects
- 🔷 Chamfered futuristic panel design
- 🖱️ Click-through overlay
- ⚡ Lightweight and low CPU usage
- 🔧 Built with C# and WinForms

---

## How It Works

The application uses a combination of:

- Global keyboard hooks
- Cursor visibility detection
- Screen region analysis
- Lightweight heuristics

to determine when the game chat is active.

When a potentially toxic message is detected:

- A HUD warning is displayed
- The message can be automatically cancelled before sending

---

## Screenshots

> Add screenshots here

---

## Technologies

- C#
- .NET
- WinForms
- WinAPI
- Screen Capture
- Pixel Detection

---

## Current Status

This project is currently experimental and intended for educational and personal use.

The implementation avoids memory manipulation, DLL injection or game modification techniques and focuses only on external overlays and input analysis.

---

## Future Improvements

- AI-based toxicity scoring
- Smart message rewriting
- OCR-based chat detection
- Multi-game support
- Overwolf integration
- Better overlay animations
- Configurable toxicity profiles

---

## Disclaimer

This project is not affiliated with or endorsed by Blizzard Entertainment.