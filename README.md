This is a POC project using .NET and Microsoft Semantic Kernel for and AI Powered application that is able to read logs and manage nutrunners state (but could be any device connected to a network).

## ⚙️ Environment Configuration

This project requires certain API keys and settings to run locally. These are managed using the .NET Secret Manager and are not stored in the repository.

To configure your local environment, follow these steps:

1.  **Obtain API Keys:** You will need an API key from [Name of the Service, e.g., Google Maps, Stripe, etc.]. You can get one by signing up on their website.

2.  **Set the Secret Key:** Open a terminal in the project's root directory and run the following command, replacing `"PASTE_YOUR_KEY_HERE"` with your actual API key:

    ```sh
    dotnet user-secrets set "Settings:ApiKey" "PASTE_YOUR_KEY_HERE"
    ```

3.  Once the secret is set, you can run the project.
