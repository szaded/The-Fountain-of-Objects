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
        private Grid Grid;
        private Position position = new(0, 0); // Generate starting position
        private bool fountainOfObjects = false; // The Fountain is de-activated at the start of the game.

        public Game(int rows, int columns)
        {
            Grid = new Grid(rows, columns);
        }

        public void Play()
        {
            while (true) // Main gameplay loop
            {
                RoomState();

                while (true) // Player action loop.
                {
                    // First present the choice to activate the Fountain if in the correct room.
                    if (Grid.GetRoom(position.Row, position.Column) == Room.Fountain && !fountainOfObjects)
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
                    if (!CheckLegalMovement(input)) Console.WriteLine("\nYou can't move there!");
                    else
                    {
                        Movement(input);
                        break;
                    }
                }

                if (WinCheck()) break; // The player must turn on the Fountain of Objects and return to the Entrance.
            }

            bool CheckLegalMovement(Direction direction)
            {
                if (direction == Direction.North && position.Row == 0) return false; // Cannot move north to row -1
                if (direction == Direction.South && position.Row == 3) return false; // Cannot move south to row 4
                if (direction == Direction.East && position.Column == 3) return false; // Cannot move east to column 4
                if (direction == Direction.West && position.Column == 0) return false; // Cannot move west to column -1

                return true;
            }

            void Movement(Direction direction)
            {
                if (direction == Direction.North) position.Row--;
                if (direction == Direction.South) position.Row++;
                if (direction == Direction.East) position.Column++;
                if (direction == Direction.West) position.Column--;
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
            // Play intro
        }

        private void RoomState()
        {
            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine($"You are in the room at (Row={position.Row}, Column={position.Column})");
            Sense(Grid.GetRoom(position.Row, position.Column)); // Hear or see what is in the room.
        }

        private void Sense(Room room)
        {
            if (room == Room.Entrance) Console.WriteLine("You see light coming from the cavern entrance.");
            if (room == Room.Fountain && !fountainOfObjects) Console.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!");
            if (room == Room.Fountain && fountainOfObjects) Console.WriteLine("You hear the rushing waters from the Fountain of Objects. The Fountain of Objects is here and is activated!");
        }

        private bool WinCheck()
        {
            if (Grid.GetRoom(position.Row, position.Column) == Room.Entrance && fountainOfObjects == true)
            {
                Console.WriteLine("\nYou managed to turn on the Fountain of Objects and return safely! You win!");
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
        public int Row { get; set; }
        public int Column { get; set; }

        public Position(int x, int y)
        {
            Row = x;
            Column = y;
        }
    }

    public enum Room { Normal, Entrance, Fountain, Boundary } // Boundary is currently not in use anywhere.

    public enum Direction { North, South, East, West }
}

