# DNN ChatBot MVC

A semantic search and Q&A application for DotNetNuke websites with multilingual support (Hebrew/English).

## Prerequisites

- .NET 8.0 SDK
- SQL Server
- Python 3.8+ with pip
- Visual Studio 2022 or VS Code

## Python Dependencies

Install the required Python packages:
```bash
pip install transformers torch sentencepiece
```

## Configuration

1. Update the connection strings in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DnnDatabase": "your_connection_string",
       "EmbeddingsDatabase": "your_connection_string"
     }
   }
   ```

2. Set up your OpenAI API key in `appsettings.json`:
   ```json
   {
     "OpenAI": {
       "ApiKey": "your_api_key"
     }
   }
   ```

3. Configure the Python path in `appsettings.json`:
   ```json
   {
     "PythonPath": "path_to_your_python_executable"
   }
   ```

## Database Setup

1. Open Package Manager Console
2. Run the following commands:
   ```
   Add-Migration InitialCreate
   Update-Database
   ```

## Running the Application

1. Build the solution
2. Run the application
3. Access the chat interface at `https://localhost:xxxx`

## Features

- Semantic search using OpenAI embeddings
- Multilingual support (Hebrew/English)
- Real-time chat interface
- DNN content integration
- Vector similarity search
- Automatic language detection and translation

## Project Structure

- `Controllers/`: MVC controllers
- `Models/`: Data models and view models
- `Services/`: Business logic and external service integration
- `Repositories/`: Data access layer
- `Views/`: UI templates
- `PythonServices/`: Python-based translation service
- `wwwroot/`: Static files (JS, CSS)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request
