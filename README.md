# MusicApiSolution

Welcome to **MusicApiSolution**! This repository contains a REST API designed to gather and serve music artist-related information from multiple data sources. The solution leverages the APIs of MusicBrainz, Wikidata, Wikipedia, and Cover Art Archive to provide detailed artist information based on an MBID (MusicBrainz Identifier).

## Table of Contents
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [Setup and Installation](#setup-and-installation)
- [API Endpoints](#api-endpoints)
- [How It Works](#how-it-works)
- [Contributing](#contributing)
- [License](#license)

## Features
- **Artist Info Aggregation**: Retrieve comprehensive information about an artist, including biography, images, and discography.
- **Data Sources**: Integrates data from [MusicBrainz](https://musicbrainz.org/), [Wikidata](https://www.wikidata.org/), [Wikipedia](https://en.wikipedia.org/), and [Cover Art Archive](https://coverartarchive.org/).
- **Single Endpoint**: Provides a unified response based on the MBID of the artist.

## Technologies Used
- **.NET 6** for building a robust API.
- **ASP.NET Core** for RESTful services.
- **C#** as the programming language.
- **HttpClient** for handling API requests.
- **MusicApi.Contracts** as a class library, making it easy to publish the solution as a NuGet package.

## Getting Started
These instructions will help you set up a copy of the project on your local machine for development and testing purposes.

### Prerequisites
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Setup and Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/jatindershub/MusicApiSolution.git
   cd MusicApiSolution
   ```
2. Navigate to the `MusicApi` folder and restore dependencies:
   ```bash
   dotnet restore
   ```
3. Build the solution:
   ```bash
   dotnet build
   ```
4. Run the API:
   ```bash
   dotnet run --project MusicApi
   ```
5. The API should now be available at `https://localhost:5001`.

## API Endpoints
- **GET** `/api/artist/{mbid}`: Retrieves detailed information about an artist.
  - **Parameters**: 
    - `{mbid}`: MusicBrainz Identifier for the artist.
  - **Response**:
    - Returns a JSON object containing artist data including name, biography, images, and albums.

### Example Request
```http
GET /api/artist/5b11f4ce-a62d-471e-81fc-a69a8278c7da
```

### Example Response
```json
{
  "name": "Nirvana",
  "biography": "Nirvana was an American rock band formed in Aberdeen, Washington, in 1987...",
  "albums": [
    {
      "title": "Nevermind",
      "releaseDate": "1991-09-24"
    }
  ],
  "images": [
    "https://coverartarchive.org/release/.../front.jpg"
  ]
}
```

## How It Works
- The API takes an MBID as input and queries the MusicBrainz API to gather initial information about the artist.
- Using the data from MusicBrainz, additional queries are made to Wikidata and Wikipedia for supplementary information (e.g., biography).
- Album cover images are fetched from the Cover Art Archive.
- The gathered data is then consolidated into a unified response.

## Contributing
Contributions are welcome! Please follow these steps:
1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Commit your changes (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature-branch`).
5. Open a Pull Request.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
