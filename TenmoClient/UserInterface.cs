using System;
using System.Collections.Generic;
using TenmoClient.APIClients;
using TenmoClient.Data;

namespace TenmoClient
{
    public class UserInterface
    {
        private readonly ConsoleService consoleService = new ConsoleService();
        private readonly AuthService authService = new AuthService();
        private readonly AccountService accountService = new AccountService();

        private bool quitRequested = false;

        public void Start()
        {
            while (!quitRequested)
            {
                while (!authService.IsLoggedIn)
                {
                    ShowLogInMenu();
                }

                // If we got here, then the user is logged in. Go ahead and show the main menu
                ShowMainMenu();
            }
        }

        private void ShowLogInMenu()
        {
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.Write("Please choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int loginRegister))
            {
                Console.WriteLine("Invalid input. Please enter only a number.");
            }
            else if (loginRegister == 1)
            {
                HandleUserLogin();
            }
            else if (loginRegister == 2)
            {
                HandleUserRegister();
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        private void ShowMainMenu()
        {
            int menuSelection;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else
                {
                    switch (menuSelection)
                    {
                        case 1: // View Balance
                            ViewBalance();
                            break;
                        case 2: // View Past Transfers
                            ViewPastTransfers();
                            break;
                        case 3: // View Pending Requests
                            Console.WriteLine("NOT IMPLEMENTED!"); // TODO: Implement me
                            break;
                        case 4: // Send TE Bucks
                            SendTEBucks();
                            break;
                        case 5: // Request TE Bucks
                            Console.WriteLine("NOT IMPLEMENTED!"); // TODO: Implement me
                            break;
                        case 6: // Log in as someone else
                            Console.WriteLine();
                            UserService.ClearLoggedInUser(); //wipe out previous login info
                            return; // Leaves the menu and should return as someone else
                        case 0: // Quit
                            Console.WriteLine("Goodbye!");
                            quitRequested = true;
                            return;
                        default:
                            Console.WriteLine("That doesn't seem like a valid choice.");
                            break;
                    }
                }
            } while (menuSelection != 0);
        }

        private void HandleUserRegister()
        {
            bool isRegistered = false;

            while (!isRegistered) //will keep looping until user is registered
            {
                LoginUser registerUser = consoleService.PromptForLogin();
                isRegistered = authService.Register(registerUser);
            }

            Console.WriteLine("");
            Console.WriteLine("Registration successful. You can now log in.");
        }

        private void HandleUserLogin()
        {
            while (!UserService.IsLoggedIn) //will keep looping until user is logged in
            {
                LoginUser loginUser = consoleService.PromptForLogin();
                API_User user = authService.Login(loginUser);
                if (user != null)
                {
                    UserService.SetLogin(user);
                }
            }
        }

        private void ViewBalance()
        {
            decimal balance = accountService.GetBalance(UserService.UserId, UserService.Token);
            Console.WriteLine($"Your current account balance is: {balance.ToString("C")}");
        }

        private void SendTEBucks()
        {
            List<API_User> users = accountService.GetUsers(UserService.Token);

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Users");
            Console.WriteLine("ID          Name                           ");
            Console.WriteLine("-------------------------------------------");

            foreach (API_User user in users)
            {
                Console.WriteLine($"{user.UserId}".PadRight(12) + $"{user.Username}".PadRight(31));
            }

            Console.WriteLine("---------");
            Console.WriteLine();
            Console.WriteLine("Enter ID of user you are sending to (0 to cancel): ");
            int transferToUserId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter amount: ");
            decimal amountToTransfer = Convert.ToDecimal(Console.ReadLine());

            bool success = accountService.SendTransfer(UserService.Token, UserService.UserId, transferToUserId, amountToTransfer);
        }

        private void ViewPastTransfers()
        {
            List<API_TransferDetails> transfers = accountService.GetTransfers(UserService.Token);

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Transfers");
            Console.WriteLine("ID          From/To                 Amount");
            Console.WriteLine("-------------------------------------------");

            foreach (API_TransferDetails transfer in transfers)
            {
                string columnOne = transfer.TransferId.ToString().PadRight(12);
                string columnTwo;

                if (UserService.UserId == transfer.ToUserId)
                {
                    columnTwo = $"From: {transfer.FromUsername}".PadRight(23);
                }
                else
                {
                    columnTwo = $"To: {transfer.ToUserName}".PadRight(23);
                }

                string columnThree = transfer.TransferAmount.ToString("C").PadLeft(7);
                Console.WriteLine(columnOne + columnTwo + columnThree);
            }
            Console.WriteLine("---------");
            Console.WriteLine("Please enter transfer ID to view details(0 to cancel): ");
            int transferDetailsId = Convert.ToInt32(Console.ReadLine());

            foreach (API_TransferDetails transferdetail in transfers)
            {
                if (transferDetailsId == transferdetail.TransferId)
                {
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine("Transfer Details");
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine($"ID: {transferdetail.TransferId}");
                    Console.WriteLine($"From: {transferdetail.FromUsername}");
                    Console.WriteLine($"To: {transferdetail.ToUserName}");
                    Console.WriteLine($"Type: {transferdetail.Type}");
                    Console.WriteLine($"Status: {transferdetail.Status}");
                    Console.WriteLine($"Amount: {transferdetail.TransferAmount}");
                }
            }
        }
    }
}
