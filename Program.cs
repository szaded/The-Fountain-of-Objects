namespace The_Fountain_of_Objects
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(4, 4);
            game.Play();

            // Press any key to close the game.
            Console.ReadKey();
        }
    }

    public class Game
    {
        readonly private Grid _grid; // Changed to an immutable field.
        private Position _position;
        private bool fountainOfObjects = false; // The Fountain is de-activated at the start of the game.

        public Game(int rows, int columns)
        {
            _grid = new Grid(rows, columns);
            _position = new(0, 0, _grid); // Player starts at the entrance. Added Grid parameter to allow Position to finds it's own position on the Grid.
        }

        public void Play()
        {
            while (true) // Main gameplay loop
            {
                RoomState();

                while (true) // Handles user input and the player's position on the grid.
                {
                   

                    ColouredText.Write("Which way do you want to go? (move north/south/east/west, or enable fountain) ", TextType.Input);

                    PlayerAction input = Console.ReadLine() switch
                    {
                        "move north" => PlayerAction.MoveNorth,
                        "move south" => PlayerAction.MoveSouth,
                        "move east" => PlayerAction.MoveEast,
                        "move west" => PlayerAction.MoveWest,
                        "enable fountain" => PlayerAction.EnableFountain,
                    };

                    // Prevents player from going outside of the grid.
                    if (input == PlayerAction.EnableFountain) ActivateFountain();
                    else if (!CheckLegalMove(input)) ColouredText.WriteLine("\nYou can't do that!", TextType.Descriptive);
                    else
                    {
                        _position.Move(input);
                        break;
                    }
                }

                if (WinCheck()) break; // The player must turn on the Fountain of Objects and return to the Entrance.
            }

            bool CheckLegalMove(PlayerAction direction)
            {
                return direction switch
                {
                    // Updated to a switch statement and based on _grid.Row and _grid.Column (scalable)
                    PlayerAction.MoveNorth => _position.Row > 0,
                    PlayerAction.MoveSouth => _position.Row < _grid.Row - 1,
                    PlayerAction.MoveEast => _position.Column < _grid.Column - 1,
                    PlayerAction.MoveWest => _position.Column > 0,
                    _ => false
                };
            }

            void ActivateFountain()
            {
                if (_position.CurrentRoom != Room.Fountain)
                {
                    ColouredText.WriteLine("The Fountain is not in this room.", TextType.Descriptive);
                }
                else if (!fountainOfObjects)
                {
                    fountainOfObjects = true;
                    ColouredText.WriteLine("You hear the rushing waters from the Fountain of Objects. It has been reactivated!", TextType.Fountain);
                }
                else
                {
                    ColouredText.WriteLine("The Fountain is already active.", TextType.Fountain);
                }
            }
        }

        private void Intro()
        {
            // TO-DO: intro text to the game.
        }

        private void RoomState()
        {
            ColouredText.WriteLine("\n--------------------------------------------------", TextType.Descriptive);
            ColouredText.WriteLine($"You are in the room at (Row={_position.Row}, Column={_position.Column})", TextType.Descriptive);
            Sense(_position.CurrentRoom); // Hear or see what is in the room.
        }

        private void Sense(Room room)
        {
            if (room == Room.Entrance) ColouredText.WriteLine("You see light coming from the cavern entrance.", TextType.Entrance);
            if (room == Room.Fountain && !fountainOfObjects) ColouredText.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!", TextType.Fountain);
            if (room == Room.Fountain && fountainOfObjects) ColouredText.WriteLine("You hear the rushing waters from the Fountain of Objects. The Fountain of Objects is here and is activated!", TextType.Fountain);
        }

        private bool WinCheck()
        {
            if (_position.CurrentRoom == Room.Entrance && fountainOfObjects == true)
            {
                ColouredText.WriteLine("\nThe Fountain of Objects has been reactivated, and you have escaped with your life!\nYou win!", TextType.Narrative);
                return true;
            }
            else return false;
        }
    }

    public class Grid
    {
        public int Row { get; } // Read-only properties, cannot be changed once initialised.
        public int Column { get; } // As above.
        private Room[,] _grid;

        public Grid(int rows, int columns)
        {
            Row = rows;
            Column = columns;
            _grid = new Room[Row, Column]; // Generate RxC grid of Rooms.

            // Define specific rooms, default enum is Normal.
            _grid[0, 0] = Room.Entrance;
            _grid[0, 2] = Room.Fountain;
        }

        public Room GetRoom(int row, int column)
        {
            return _grid[row, column];
        }
    }

    public class Position
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        private readonly Grid _grid;

        public Position(int x, int y, Grid grid)
        {
            Row = x;
            Column = y;
            _grid = grid;
        }

        public Room CurrentRoom => _grid.GetRoom(Row, Column);

        public void Move(PlayerAction direction)
        {
            // Updated to a switch statement. Formatted to a single line, looks cleaner this way.
            // Moved to Position class for better encapsulation.
            // Game doesn't need to know HOW the movement works, only that it can tell Position to change.
            switch (direction)
            {
                case PlayerAction.MoveNorth: Row--; break;
                case PlayerAction.MoveSouth: Row++; break;
                case PlayerAction.MoveEast: Column++; break;
                case PlayerAction.MoveWest: Column--; break;
            }
        }
    }

    public class ColouredText
    {
        public static void WriteLine(string text, TextType textType)
        {
            Console.ForegroundColor = TextColour(textType);
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Write(string text, TextType textType)
        {
            Console.ForegroundColor = TextColour(textType);
            Console.Write(text);
            Console.ResetColor();
        }

        private static ConsoleColor TextColour(TextType textType)
        {
            ConsoleColor colour = textType switch
            {
                TextType.Narrative => ConsoleColor.Magenta, // Intro, ending
                TextType.Descriptive => ConsoleColor.White,
                TextType.Input => ConsoleColor.Cyan,
                TextType.Entrance => ConsoleColor.Yellow,
                TextType.Fountain => ConsoleColor.Blue,
                TextType.Default => ConsoleColor.White,
                _ => ConsoleColor.White
            };

            return colour;
        }
    }

    public enum Room { Normal, Entrance, Fountain, Boundary } // Boundary is currently not in use anywhere.

    public enum PlayerAction { MoveNorth, MoveSouth, MoveEast, MoveWest, EnableFountain }

    public enum TextType { Narrative, Descriptive, Input, Entrance, Fountain, Default}
}

