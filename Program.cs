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
                    // First present the choice to activate the Fountain if in the correct room.
                    if (_position.CurrentRoom == Room.Fountain && !fountainOfObjects)
                    {
                        ActivateFountainChoice();
                    }
                    
                    Console.Write("Which way do you want to go? (north, south, east, west) ");

                    Direction input = Console.ReadLine() switch
                    {
                        "north" => Direction.North,
                        "south" => Direction.South,
                        "east" => Direction.East,
                        "west" => Direction.West,
                    };

                    // Prevents player from going outside of the grid.
                    if (!CheckLegalMove(input)) Console.WriteLine("\nYou can't move there!");
                    else
                    {
                        _position.Move(input);
                        break;
                    }
                }

                if (WinCheck()) break; // The player must turn on the Fountain of Objects and return to the Entrance.
            }

            bool CheckLegalMove(Direction direction)
            {
                return direction switch
                {
                    // Updated to a switch statement and based on _grid.Row and _grid.Column (scalable)
                    Direction.North => _position.Row > 0,
                    Direction.South => _position.Row < _grid.Row - 1,
                    Direction.East => _position.Column < _grid.Column - 1,
                    Direction.West => _position.Column > 0,
                    _ => false
                };

                // OLD CODE
                //if (direction == Direction.North && position.Row == 0) return false; // Cannot move north to row -1
                //if (direction == Direction.South && position.Row == 3) return false; // Cannot move south to row 4
                //if (direction == Direction.East && position.Column == 3) return false; // Cannot move east to column 4
                //if (direction == Direction.West && position.Column == 0) return false; // Cannot move west to column -1
                //return true;
            }

            void ActivateFountainChoice()
            {
                Console.Write("Do you want to activate the fountain? (yes/no) ");
                string? response = Console.ReadLine();

                switch (response)
                {
                    case "yes":
                        fountainOfObjects = true;
                        Console.WriteLine("You hear the rushing waters from the Fountain of objects. It has been reactivated!");
                        break;
                    case "no":
                        break;
                }
            }
        }

        private void Intro()
        {
            // TO-DO: intro text to the game.
        }

        private void RoomState()
        {
            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine($"You are in the room at (Row={_position.Row}, Column={_position.Column})");
            Sense(_position.CurrentRoom); // Hear or see what is in the room.
        }

        private void Sense(Room room)
        {
            if (room == Room.Entrance) Console.WriteLine("You see light coming from the cavern entrance.");
            if (room == Room.Fountain && !fountainOfObjects) Console.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!");
            if (room == Room.Fountain && fountainOfObjects) Console.WriteLine("You hear the rushing waters from the Fountain of Objects. The Fountain of Objects is here and is activated!");
        }

        private bool WinCheck()
        {
            if (_position.CurrentRoom == Room.Entrance && fountainOfObjects == true)
            {
                Console.WriteLine("\nThe Fountain of Objects has been reactivated, and you have escaped with your life!\nYou win!");
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

        public void Move(Direction direction)
        {
            // Updated to a switch statement. Formatted to a single line, looks cleaner this way.
            // Moved to Position class for better encapsulation.
            // Game doesn't need to know HOW the movement works, only that it can tell Position to change.
            switch (direction)
            {
                case Direction.North: Row--; break;
                case Direction.South: Row++; break;
                case Direction.East: Column++; break;
                case Direction.West: Column--; break;
            }
        }
    }

    public enum Room { Normal, Entrance, Fountain, Boundary } // Boundary is currently not in use anywhere.

    public enum Direction { North, South, East, West }
}

