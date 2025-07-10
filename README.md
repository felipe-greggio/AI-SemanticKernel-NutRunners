This is a POC project using .NET and Microsoft Semantic Kernel for and AI Powered application that is able to read logs and manage nutrunners state (but could be any device connected to a network).

## ⚙️ Environment Configuration

This project requires certain API keys and settings to run locally. These are managed using the .NET Secret Manager and are not stored in the repository.

To configure your local environment, follow these steps:

1.  **Obtain API Keys:** You will need an API key from your AI Service provider (Open AI, Azure AI Foundry, etc.).
2.  
1.  **Obtain Endpoint:** You will also need the endpoint to call the API, you can get it from the same place you got the API Key.

3.  **Set the Secret Keys:** Open a terminal in the project's root directory and run the following command, replacing `"PASTE_YOUR_KEY_HERE"` with your actual API key:

    ```sh
    dotnet user-secrets set "Settings:AIApiKey" "PASTE_YOUR_KEY_HERE"
    ```

    ```sh
    dotnet user-secrets set "Settings:AIEndpoint" "PASTE_YOUR_ENDPOINT_HERE"
    ```

4.  Once the secrets are set, you can run the project.
