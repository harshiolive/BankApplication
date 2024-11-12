using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleBankingApp
{
    class Program
    {
        static List<User> users = new List<User>();
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n--- Banking Application ---");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Register();
                        break;
                    case "2":
                        Login();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        static void Register()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            if (users.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already exists.");
                return;
            }

            users.Add(new User(username, password));
            Console.WriteLine("Registration successful.");
        }

        static void Login()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            User user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                Console.WriteLine("Invalid credentials.");
                return;
            }

            Console.WriteLine("Login successful.");
            UserMenu(user);
        }

        static void UserMenu(User user)
        {
            while (true)
            {
                Console.WriteLine("\n--- User Menu ---");
                Console.WriteLine("1. Open Account");
                Console.WriteLine("2. Deposit");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. Statement");
                Console.WriteLine("5. Calculate Interest");
                Console.WriteLine("6. Check Balance");
                Console.WriteLine("0. Logout");
                Console.Write("Select an option: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        user.OpenAccount();
                        break;
                    case "2":
                        user.Deposit();
                        break;
                    case "3":
                        user.Withdraw();
                        break;
                    case "4":
                        user.Statement();
                        break;
                    case "5":
                        user.CalculateInterest();
                        break;
                    case "6":
                        user.CheckBalance();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }

    class User
    {
        public string Username { get; }
        public string Password { get; }
        private List<Account> accounts = new List<Account>();

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public void OpenAccount()
        {
            Console.Write("Enter account holder's name: ");
            string holderName = Console.ReadLine();
            Console.Write("Enter account type (Savings/Checking): ");
            string type = Console.ReadLine();
            Console.Write("Enter initial deposit: ");
            decimal initialDeposit = decimal.Parse(Console.ReadLine());

            Account account = new Account(holderName, type, initialDeposit);
            accounts.Add(account);
            Console.WriteLine($"Account created with Account Number: {account.AccountNumber}");
        }

        public void Deposit()
        {
            Account account = SelectAccount();
            if (account == null) return;

            Console.Write("Enter deposit amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());
            account.Deposit(amount);
        }

        public void Withdraw()
        {
            Account account = SelectAccount();
            if (account == null) return;

            Console.Write("Enter withdrawal amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());
            account.Withdraw(amount);
        }

        public void Statement()
        {
            Account account = SelectAccount();
            if (account == null) return;

            account.PrintStatement();
        }

        public void CalculateInterest()
        {
            Account account = SelectAccount();
            if (account == null) return;

            account.AddMonthlyInterest();
        }

        public void CheckBalance()
        {
            Account account = SelectAccount();
            if (account == null) return;

            Console.WriteLine($"Current Balance: {account.Balance}");
        }

        private Account SelectAccount()
        {
            if (accounts.Count == 0)
            {
                Console.WriteLine("No accounts available.");
                return null;
            }

            Console.WriteLine("Select an account by Account Number:");
            foreach (var acc in accounts)
            {
                Console.WriteLine($"- {acc.AccountNumber} ({acc.Type})");
            }

            string accountNumber = Console.ReadLine();
            Account account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                Console.WriteLine("Account not found.");
            }

            return account;
        }
    }

    class Account
    {
        private static int nextAccountNumber = 1001;
        private static int nextTransactionId = 1;
        private const decimal InterestRate = 0.01m;

        public string AccountNumber { get; }
        public string HolderName { get; }
        public string Type { get; }
        public decimal Balance { get; private set; }
        private List<Transaction> transactions = new List<Transaction>();

        public Account(string holderName, string type, decimal initialDeposit)
        {
            AccountNumber = (nextAccountNumber++).ToString();
            HolderName = holderName;
            Type = type;
            Balance = initialDeposit;
        }

        public void Deposit(decimal amount)
        {
            Balance += amount;
            transactions.Add(new Transaction(nextTransactionId++, DateTime.Now, "Deposit", amount));
            Console.WriteLine("Deposit successful.");
        }

        public void Withdraw(decimal amount)
        {
            if (amount > Balance)
            {
                Console.WriteLine("Insufficient balance.");
                return;
            }

            Balance -= amount;
            transactions.Add(new Transaction(nextTransactionId++, DateTime.Now, "Withdrawal", amount));
            Console.WriteLine("Withdrawal successful.");
        }

        public void PrintStatement()
        {
            Console.WriteLine($"\n--- Statement for Account {AccountNumber} ---");
            foreach (var transaction in transactions)
            {
                Console.WriteLine($"{transaction.Date} | {transaction.Type} | Amount: {transaction.Amount}");
            }
        }

        public void AddMonthlyInterest()
        {
            if (Type.ToLower() == "savings")
            {
                decimal interest = Balance * InterestRate;
                Deposit(interest);
                Console.WriteLine($"Interest added: {interest}");
            }
            else
            {
                Console.WriteLine("Interest calculation only available for Savings accounts.");
            }
        }
    }

    class Transaction
    {
        public int Id { get; }
        public DateTime Date { get; }
        public string Type { get; }
        public decimal Amount { get; }

        public Transaction(int id, DateTime date, string type, decimal amount)
        {
            Id = id;
            Date = date;
            Type = type;
            Amount = amount;
        }
    }
}
