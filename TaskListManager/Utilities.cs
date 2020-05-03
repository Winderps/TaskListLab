using System;
using System.Collections.Generic;
using System.Text;

namespace TaskListManager
{
    class Utilities
    {
        /// <summary>
        /// Prompt the user to select an item from the main menu
        /// </summary>
        /// <returns>The number of the menu item the user selected</returns>
        public static int GetMenuSelection()
        {
            int ret = 0;
            bool cont = true;
            while (cont)
            {
                DisplayMenu();
                Console.Write("Please select a menu option: ");
                try
                {
                    ret = int.Parse(Console.ReadLine());
                    cont = false;
                }
                catch
                {
                    Console.WriteLine("Input was not valid, try again");
                }
            }
            return ret;
        }

        /// <summary>
        /// Get a string input from the user
        /// </summary>
        /// <param name="prompt">The prompt to display to the user</param>
        /// <returns>A non-empty string inputted by the user</returns>
        public static string GetUserInput(string prompt)
        {
            while(true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine().Trim();
                if (input == string.Empty)
                {
                    Console.WriteLine("Input cannot be empty!");
                }
                else
                {
                    return input;
                }
            }
        }

        /// <summary>
        /// Gets a date as input from the user
        /// </summary>
        /// <returns>A DateTime object representing the user input</returns>
        public static DateTime GetUserInputDate(string prompt)
        {
            while (true)
            {
                string date = Utilities.GetUserInput(prompt);
                try
                {
                    return DateTime.Parse(date);
                }
                catch
                {
                    Console.WriteLine("That doesn't look like a date to me!");
                }
            }
        }

        /// <summary>
        /// Get a yes or no from the user
        /// </summary>
        /// <param name="prompt">The prompt to display to the user</param>
        /// <returns>True if 'y' is entered, or false if 'n' is entered</returns>
        public static bool GetYesNoInput(string prompt)
        {
            while (true)
            {
                string input = GetUserInput(prompt).ToLower();
                if (input.Equals("y") || input.Equals("n"))
                {
                    return input[0].Equals('y');
                }
                else
                {
                    Console.WriteLine("Input must be either 'y' or 'n'.");
                }
            }
        }

        /// <summary>
        /// Display the main menu
        /// </summary>
        public static void DisplayMenu()
        {
            Console.WriteLine("1. List Tasks");
            Console.WriteLine("2. List Tasks Due Before Date");
            Console.WriteLine("3. List Tasks By Team Member");
            Console.WriteLine("4. Add Task");
            Console.WriteLine("5. Edit Task");
            Console.WriteLine("6. Delete Task");
            Console.WriteLine("7. Mark Task Complete");
            Console.WriteLine("8. Quit");
        }
    }
}
