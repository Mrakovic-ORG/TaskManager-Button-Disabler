using System;

namespace TaskManager_Button_Disabler
{
    internal static class Program
    {
        private static bool _started = false;
        private static readonly TBD Disabler = new TBD();

        public static void Main()
        {
            go_back:
            Console.Clear();

            var status = _started ? "Stop" : "Start";
            Console.WriteLine($"Keypress Enter to {status}.");

            var check = Console.ReadKey();
            switch (check.Key)
            {
                case ConsoleKey.Enter:
                    Enter();
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }

            goto go_back;
        }

        private static void Enter()
        {
            if (_started)
            {
                Disabler.Stop();
                _started = false;
            }
            else
            {
                Disabler.Start();
                _started = true;
            }
        }
    }
}
