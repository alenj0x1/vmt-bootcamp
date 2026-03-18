using CsharpIntroduction.Startup.Interfaces;
using CsharpIntroduction.Startup.Models;

namespace CsharpIntroduction.Startup.Classes
{
    public class Application(string name, string version) : IApplication
    {
        //public Application(string name, string version)
        //{
        //    _nameApp = name;
        //    _versionApp = version;
        //}

        private List<Command> _commands = [];

        private void InitCommands()
        {
            _commands.Add(new Command()
            {
                Id = 1,
                Description = "Mostrar un mensaje",
                Usage = "<message>",
                Return = "Message: <message>"
            });

            //_commands.Add(new Command()
            //{
            //    Id = 2,
            //    Description = "Sumar una cantidad",
            //    Usage = "<number> <number>",
            //    Return = "Result: <number>"
            //});
        }

        public void ShowHelp()
        {
            var message = $"""
                ---------------------------------

                {name} {version}

                ---------------------------------

                Comandos:
                {string.Join("\n", _commands.Select((cmd) => $"{cmd.Id}. {cmd.Description}\n{cmd.Usage}\n{cmd.Return}"))}

                ---------------------------------
                """;
            Console.WriteLine(message);
        }

        public void ShowMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"Message: {message}");
            Console.ResetColor();
        }

        public string ShowQuestion(string question, ConsoleColor color = ConsoleColor.White)
        {
            ShowMessage(question, color);

            return Console.ReadLine();
        }

        public void Start()
        {
            InitCommands();

            while (true)
            {
                ShowHelp();

                try
                {
                    var comandoId = Convert.ToInt32(ShowQuestion("Seleccione una opción: ", ConsoleColor.Blue));
                    var argumento = ShowQuestion("Argumento: ", ConsoleColor.Yellow);

                    if (string.IsNullOrEmpty(argumento))
                    {
                        throw new ArgumentNullException();
                    }

                    var findCommand = _commands.Where((cmd) => cmd.Id == comandoId).FirstOrDefault();

                    if (findCommand is null)
                    {
                        throw new Exception("Comando no encontrado");
                    }

                    if (findCommand.Id == comandoId)
                    {
                        ShowMessage(argumento);
                    }
                }
                catch (ArgumentNullException)
                {
                    ShowMessage("El argumento es obligatorio, no puede ser vacio.");
                }
                catch (FormatException)
                {
                    ShowMessage("No está utilizando un formato adecuado. Agrege una opción correcta");
                }
                catch (Exception e)
                {
                    ShowMessage(e.Message);
                }
            }
        }
    }
}
