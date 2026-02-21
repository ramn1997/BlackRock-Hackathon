# Retirement Saving System

This system provides a set of APIs to enable automated retirement savings through expense-based micro-investments.

## Features
- **Transaction Parser**: Enriches expenses with ceiling and remanent calculations.
- **Validator**: Validates transactions based on wage constraints and duplicates.
- **Temporal Filter**: Applies complex rules (Q for overrides, P for additions) based on date ranges.
- **Returns Calculator**: Calculates NPS and Index Fund returns with inflation adjustment and tax benefits.
- **Performance reporting**: Tracks system metrics like execution time and memory usage.

## Setup and Running

### Prerequisites
- .NET 10 SDK
- Docker (optional, for containerized execution)

### Running Locally
1. Navigate to the project root:
   ```bash
   cd RetirementSavingSystem
   ```
2. Build and run the API:
   ```bash
   dotnet run --project RetirementSystem.API/RetirementSystem.API.csproj
   ```
   The API will be available at `http://localhost:5477` (or as configured in `launchSettings.json`).

### Running with Docker (Local Build)
1. Build the image:
   ```bash
   docker build -t blk-hacking-ind-ram-n .
   ```
2. Run the container:
   ```bash
   docker run -d -p 5477:5477 blk-hacking-ind-ram-n
   ```
   Alternatively, use Docker Compose:
   ```bash
   docker compose up -d
   ```

### Running the Public Docker Image
A public Docker image is automatically built and published to GitHub Container Registry (GHCR) upon merges to the `main` branch. 
You can run it from anywhere without needing the source code:
```bash
docker run -d -p 5477:5477 ghcr.io/<your-github-username>/<your-repo-name>:latest
```
*Note: Ensure your GitHub repository's package is set to Public under its settings to allow anonymous pulls.*

## Testing
To run the unit tests:
```bash
dotnet test
```
The tests are located in the `test` folder.

## Technical Implementation Details
- **Compound Interest**: $A = P(1 + r/n)^{nt}$ where $n=1$ (compounded annually).
- **Inflation Adjustment**: $A_{real} = A / (1 + inflation)^t$
- **Tax Calculation**: Implements simplified Indian tax slabs (0%, 10%, 15%, 20%, 30%).
- **Rules Engine**: Handles overlapping temporal constraints for Q and P periods correctly.
