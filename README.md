# Poker AI Full-Stack Application

A full-stack Texas Hold’em Poker AI game built with an ASP.NET Core Web API backend (C#) and an Angular frontend (TypeScript). Bots and human players can play through a REST API or via a web UI with automated bot “thinking” delays.

---

## Table of Contents

* [Features](#features)
* [Tech Stack](#tech-stack)
* [Prerequisites](#prerequisites)
* [Project Structure](#project-structure)
* [Getting Started](#getting-started)

  * [Backend Setup](#backend-setup)
  * [Frontend Setup](#frontend-setup)
* [Usage](#usage)
* [API Endpoints](#api-endpoints)
* [Contributing](#contributing)
* [License](#license)

---

## Features

* Deal, betting, and showdown logic for Texas Hold’em
* Automated bot players with adjustable “thinking” delays (1–3 seconds)
* Stage progression (preflop → flop → turn → river → showdown)
* Full game state exposed over REST endpoints
* Interactive Angular UI to visualize pot, community cards, and player actions
* Swagger documentation for backend API

---

## Tech Stack

* **Backend:** ASP.NET Core Web API (C#), .NET 6+
* **Frontend:** Angular 12+ (TypeScript)
* **HTTP:** RESTful API
* **Tools:** Swagger (Swashbuckle), Angular CLI, .NET CLI

---

## Prerequisites

* [.NET 6 SDK](https://dotnet.microsoft.com/download)
* [Node.js (v14+)](https://nodejs.org/) and npm
* [Angular CLI](https://angular.io/cli) (install via `npm install -g @angular/cli`)
* Git (to clone the repo)

---

## Project Structure

```
poker-ai/
├── backend/           # ASP.NET Core Web API (PokerApi)
│   ├── Controllers/   # REST controllers (GameController)
│   ├── Models/        # Domain models (Player, Table, Deck, Card)
│   ├── Services/      # GameService (singleton holding Table instance)
│   └── Program.cs     # Startup and DI configuration
└── frontend/          # Angular web app (poker-web)
    ├── src/app/
    │   ├── services/   # GameService (HTTP client)
    │   ├── components/ # GameComponent + templates
    │   └── app.module.ts
    ├── angular.json    # Angular project config
    └── package.json    # npm dependencies
```

---

## Getting Started

Follow these steps to run the application locally.

### Backend Setup

1. Navigate to the backend folder:

   ```bash
   cd poker-ai/backend/PokerApi
   ```
2. Restore and build:

   ```bash
   dotnet restore
   dotnet build
   ```
3. Run the API:

   ```bash
   dotnet run
   ```

   The API will start on `http://localhost:5000`.
4. (Optional) Visit Swagger UI at `http://localhost:5000/swagger` to explore endpoints.

### Frontend Setup

1. Navigate to the frontend folder:

   ```bash
   cd poker-ai/frontend/poker-web
   ```
2. Install npm dependencies:

   ```bash
   npm install
   ```
3. Run the Angular development server:

   ```bash
   ng serve --open
   ```

   The app will open in your browser at `http://localhost:4200`.

---

## Usage

1. On the web UI, you can:

   * Start a new hand
   * Fold, Check, Call, Bet/Raise (enter an amount)
   * Watch bots make decisions with a 1–3 second delay
2. The game automatically progresses through all stages and deals new hands at showdown.

---

## API Endpoints

| Endpoint            | Method | Description                                    |
| ------------------- | ------ | ---------------------------------------------- |
| `/api/game/newhand` | POST   | Reset the hand, post blinds, deal cards        |
| `/api/game/action`  | POST   | Submit a player action (fold, call, bet, etc.) |
| `/api/game/botmove` | POST   | Execute a single bot move (for delay logic)    |

**Action Request Body**:

```json
{ "action": "call", "amount": 50 }
```

**GameState Response** includes:

* `pot`, `currentBet`, `stage`, `turnIndex`, `roundComplete`
* Array of `players` (name, chips, hand, folded, isTurn, isDealer)
* `shownCards` (community cards)

---

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

1. Fork the repository
2. Create a new branch (`git checkout -b feature/my-feature`)
3. Commit your changes (`git commit -m "Add my feature"`)
4. Push to your branch (`git push origin feature/my-feature`)
5. Open a Pull Request

---

## License

This project is released under the MIT License. See [LICENSE](LICENSE) for details.
