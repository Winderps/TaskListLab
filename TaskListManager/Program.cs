using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TaskListManager
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("tasks.json"))
            {
                Console.WriteLine("Found an existing tasks file. Loading it...");
                Task.Tasks = Task.LoadTasks("tasks.json");
            }
            else
            {
                Console.WriteLine("Couldn't find an existing task list, creating one...");
                Task.Tasks = new List<Task>();
                Task.SaveTasks(Task.Tasks, "tasks.json");
            }

            while(true)
            {
                int selection = Utilities.GetMenuSelection();
                DoMenuOption(selection);
            }
        }

        /// <summary>
        /// Runs the code associated with the selected menu option
        /// </summary>
        /// <param name="selection">The number of the menu option selected</param>
        private static void DoMenuOption(int selection)
        {
            Console.Clear();
            switch(selection)
            {
                case 1: // list tasks
                    Task.ListTasks();
                    break;
                case 2: // list tasks by due date
                    DateTime date = Utilities.GetUserInputDate("List tasks due before what date: ");
                    Task.ListTasks(x => x.DueDate < date);
                    break;
                case 3: // list tasks by team member
                    string teamMember = Utilities.GetUserInput("Which team member's tasks would you like to see: ");
                    Task.ListTasks(x => x.Name.ToLower().Contains(teamMember.ToLower()));
                    break;
                case 4: // add a new task
                    Task.AddNewTask();
                    break;
                case 5: // delete task
                    Task.DeleteTask();
                    break;
                case 6: // mark task complete
                    Task.CompleteTask();
                    break;
                case 7: // quit
                    Environment.Exit(0);
                    break;
                default:
                    // the user has selected an invalid menu option
                    Console.WriteLine("Invalid option selected.");
                    break;
            }
        }
    }
}