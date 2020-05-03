using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TaskListManager
{
    class Task
    {
        #region private variables
        #region static variables
        private static List<Task> _tasks;
        #endregion static variables

        #region member variables
        private string _name;
        private string _desc;
        private DateTime _dueDate;
        private bool _complete;
        #endregion member variables
        #endregion private variables

        #region fields
        /// <summary>
        /// The name of the person assigned to the task
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }

        /// <summary>
        /// A short description of the task
        /// </summary>
        public string Desc { get { return _desc; } set { _desc = value; } }

        /// <summary>
        /// The date when the task is due
        /// </summary>
        public DateTime DueDate { get { return _dueDate; } set { _dueDate = value; } }

        /// <summary>
        /// Whether or not the task is complete
        /// </summary>
        public bool Complete { get { return _complete; } set { _complete = value; } }

        /// <summary>
        /// The current list of tasks
        /// </summary>
        public static List<Task> Tasks { get { return _tasks; } set { _tasks = value; } }
        #endregion fields

        #region member functions

        /// <summary>
        /// Parameterless constructor for deserialization.
        /// </summary>
        public Task()
        {

        }

        /// <summary>
        /// Create a new Task (default due date is set to 1 day from DateTime.Now
        /// </summary>
        /// <param name="name">The name of the team member assigned to the task</param>
        /// <param name="desc">A short description of the task</param>
        public Task(string name, string desc)
        {
            _name = name;
            _desc = desc;
            _dueDate = DateTime.Now.AddDays(1);
        }

        /// <summary>
        /// Create a new Task
        /// </summary>
        /// <param name="name">The name of the team member assigned to the task</param>
        /// <param name="desc">A short description of the task</param>
        /// <param name="dueDate">When the task is due</param>
        public Task(string name, string desc, DateTime dueDate)
        {
            _name = name;
            _desc = desc;
            _dueDate = dueDate;
            _complete = false;
        }

        public string FormatForDisplay(int tab1, int tab2, int tab3, int tab4)
        {
            string complete = (_complete ? "Complete" : "Incomplete");
            return new string(' ', tab1 - _name.Length) + _name
                + new string(' ', tab2 - DueDate.ToShortDateString().Length) + DueDate.ToShortDateString()
                + new string(' ', tab3 - complete.Length) + complete
                + new string(' ', tab4 - _desc.Length) + _desc;
        }
        #endregion member functions

        #region static functions
        /// <summary>
        /// Serialize a list of Tasks to a JSON file
        /// </summary>
        /// <param name="tasks">The list of tasks to serialize</param>
        /// <param name="filename">The JSON file to save to</param>
        public static void SaveTasks(List<Task> tasks, string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            StreamWriter sw = new StreamWriter(File.OpenWrite(filename));
            sw.Write(JsonSerializer.Serialize(tasks));
            sw.Flush();
            sw.Close();
            Console.WriteLine("Current tasks list saved.");
        }

        /// <summary>
        /// Load a list of tasks from a JSON file
        /// </summary>
        /// <param name="filename">The JSON file to load from</param>
        /// <returns></returns>
        public static List<Task> LoadTasks(string filename)
        {
            StreamReader sr = new StreamReader(File.OpenRead(filename));
            List<Task> ret = JsonSerializer.Deserialize<List<Task>>(sr.ReadToEnd());
            sr.Close();
            return ret;
        }

        /// <summary>
        /// Allow the user to mark a task from the list as complete
        /// </summary>
        public static void CompleteTask()
        {
            ListIncompleteTasks();
            List<Task> incompleteTasks = GetIncompleteTasks();
            Task toComplete = GetTaskSelection(incompleteTasks, "Please enter the number of the task to mark as complete (enter -1 to cancel): ");
            if (toComplete == null) // user cancelled the operation
            {
                return;
            }
            DisplayTask(toComplete);
            if (Utilities.GetYesNoInput("Are you sure you would like to mark this task as complete (y/n): "))
            {
                toComplete.Complete = true;
                Console.WriteLine($"{toComplete.Desc} has been completed by {toComplete.Name}");
                SaveTasks(_tasks, "tasks.json");
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }
        }

        /// <summary>
        /// Prompt the user to delete one of the tasks
        /// </summary>
        public static void DeleteTask()
        {
            ListTasks();
            Task toDelete = GetTaskSelection(_tasks, "Please enter the number of the task you would like to delete (enter -1 to cancel): ");
            if (toDelete == null)
            {
                return;
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("------- THIS IS PERMANENT AND CANNOT BE UNDONE -------");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine();
            Console.ResetColor();
            DisplayTask(toDelete);
            if (Utilities.GetYesNoInput("Are you sure you would like to delete this task (y/n): "))
            {

                _tasks.Remove(toDelete);
                Console.WriteLine($"{toDelete.Desc} has been deleted.");
                SaveTasks(_tasks, "tasks.json");
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }
        }

        private static void DisplayTask(Task t)
        {
            Console.WriteLine($"{"Team Member",25}{"Due Date",20}{"Status",20}{"Description",55}");
            Console.WriteLine(t.FormatForDisplay(25, 20, 20, 55));
        }

        /// <summary>
        /// Prompt the user to add a new task to the list
        /// </summary>
        public static void AddNewTask()
        {
            string desc = Utilities.GetUserInput("Please give a short description for the new task: ");
            string name = Utilities.GetUserInput("Who is assigned to this task: ");
            DateTime dueDate = Utilities.GetUserInputDate("When is this task due: ");
            _tasks.Add(new Task(name, desc, dueDate));
            SaveTasks(_tasks, "tasks.json");
        }

        /// <summary>
        /// List the current tasks
        /// </summary>
        public static void ListTasks()
        {
            int i = 0;
            Console.WriteLine($"{"Team Member",25}{"Due Date",20}{"Status",20}{"Description",55}");

            //this loops through the list of tasks and displays them formatted into columns
            _tasks.ForEach(x =>
                Console.WriteLine($"{++i}." + x.FormatForDisplay(25- (i.ToString().Length + 1), 20, 20, 55))
                );
        }

        /// <summary>
        /// List tasks matching certain criteria
        /// </summary>
        /// <param name="predicate">A function determining which tasks to list</param>
        public static void ListTasks(Func<Task, bool> predicate)
        {
            int i = 0;
            foreach(Task t in _tasks)
            {
                if (predicate(t))
                {
                    Console.WriteLine($"{++i}." + t.FormatForDisplay(25-(i.ToString().Length+1), 20, 20, 55));
                }
            }
        }

        /// <summary>
        /// Lists only incomplete tasks
        /// </summary>
        public static void ListIncompleteTasks()
        {
            ListTasks(x => x.Complete == false);
        }

        /// <summary>
        /// Get a list of incomplete tasks
        /// </summary>
        /// <returns>A List of Tasks containing only tasks that are not marked as complete</returns>
        private static List<Task> GetIncompleteTasks()
        {
            return _tasks.Where(x => x.Complete == false).ToList();
        }

        /// <summary>
        /// Let the user select a task from a list
        /// </summary>
        /// <param name="taskList">The list of tasks to display</param>
        /// <param name="prompt">A string to display to the console to prompt the user</param>
        /// <returns>The Task object selected by the user OR null if they entered -1</returns>
        public static Task GetTaskSelection(List<Task> taskList, string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                try
                {
                    int selection = int.Parse(Console.ReadLine());
                    if (selection == -1) // the user cancelled the operation
                    {
                        return null;
                    }
                    return taskList[selection-1];
                }
                catch (FormatException)
                {
                    Console.WriteLine("That's not a number!");
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("That is not the number of a valid task!");
                }
            }
        }
        #endregion static functions
    }
}