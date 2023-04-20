// See https://aka.ms/new-console-template for more information
using XboxWebApi.Authentication.Headless;
using XboxWebApi.Authentication;

namespace XboxAuth
{
    class Program
    {
        static async Task<int> ChooseAuthStrategy(IEnumerable<string> choices)
        {
            Console.WriteLine("Choose desired auth strategy");
            foreach (var choice in choices)
            {
                Console.WriteLine(choice);
            }
            string userChoice = await Console.In.ReadLineAsync();
            return Int32.Parse(userChoice);
        }

        static async Task<string> VerifyProof(string prompt)
        {
            Console.WriteLine(prompt);
            return await Console.In.ReadLineAsync();
        }

        static async Task<string> EnterOneTimeCode(string prompt)
        {
            Console.WriteLine(prompt);
            return await Console.In.ReadLineAsync();
        }
        static async Task<int> RunHeadlessAuth()
        {
            Console.WriteLine(":: Headless ::");
            var authUrl = AuthenticationService.GetWindowsLiveAuthenticationUrl();
            try
            {
                string Email = "z1@skyhk.net";
                string Password = "Sc147258";
                string TokenFilepath = "token";
                var headlessAuthService = new HeadlessAuthenticationService(authUrl);
                headlessAuthService.ChooseAuthStrategyCallback = ChooseAuthStrategy;
                headlessAuthService.VerifyPosessionCallback = VerifyProof;
                headlessAuthService.EnterOneTimeCodeCallback = EnterOneTimeCode;

                var response = await headlessAuthService.AuthenticateAsync(Email, Password);
                var authenticator = new AuthenticationService(response);
                bool success = await authenticator.AuthenticateAsync();
                Console.WriteLine("Authentication succeeded");

                if (!String.IsNullOrEmpty(TokenFilepath))
                {
                    success = await authenticator.DumpToJsonFileAsync(TokenFilepath);
                    if (!success)
                    {
                        Console.WriteLine($"Failed to dump tokens to {TokenFilepath}");
                        return 2;
                    }
                    Console.WriteLine($"Tokens saved to {TokenFilepath}");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Headless authentication failed, error: " + exc.Message);
                return 1;
            }

            return 0;
        }

        async static Task Main(string[] args)
        {
            await RunHeadlessAuth();
        }
    }


}
