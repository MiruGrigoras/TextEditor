using System;
namespace TextEditor
{
    class TextEditor
    {
        int totalRows;
        int totalColumns;
        int cursorRow;
        int cursorColumn;

        public TextEditor()
        {
             cursorRow = totalRows = Console.CursorTop;
             cursorColumn = totalColumns = Console.CursorLeft;
        }
        static void Main(string[] args)
        {
            TextEditor editor = new();
            ConsoleKeyInfo consoleInput;
            do
            {
                consoleInput = Console.ReadKey();

                //Console.WriteLine(x.Modifiers + " - " + ConsoleModifiers.Shift);
                switch(consoleInput.Key)
                {
                    case ConsoleKey.Enter:
                        editor.EnterMoveCursor();
                        break;
                    case ConsoleKey.DownArrow:
                        //Console.WriteLine("Down");
                        editor.ArrowMoveCursor(0, 1);
                        break;
                    case ConsoleKey.UpArrow:
                        //Console.WriteLine("Up");
                        editor.ArrowMoveCursor(0, -1);
                        break;
                    case ConsoleKey.LeftArrow:
                        //Console.WriteLine("Left");
                        editor.ArrowMoveCursor(1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        //Console.WriteLine("Right");
                        editor.ArrowMoveCursor(-1, 0);
                        break;
                }


            } while (consoleInput.Key != ConsoleKey.Escape); //close app on ESC
            Console.WriteLine("\nTotal lines and columns" + editor.totalRows + " - " + editor.totalColumns);
        }

        private void EnterMoveCursor()
        {
            cursorRow += 1;
            totalRows++;
            Console.SetCursorPosition(0, cursorRow);
        }

        private void ArrowMoveCursor(int columnOffset, int rowOffset)
        {
            int newRow = cursorRow + rowOffset;
            int newColumn = cursorColumn + columnOffset;
            if (NewPositionInsideBounds(newRow, newColumn))
            {
                cursorColumn = newColumn;
                cursorRow = newRow;
                Console.SetCursorPosition(cursorColumn, cursorRow);
            }
        }

        private bool NewPositionInsideBounds(int newRow, int newColumn)
        {
            return newRow <= totalRows && newRow >= 0 && newColumn <= totalColumns && newColumn >= 0;
        }

        private void AddNewRow()
        {
            totalRows++;
        }

        private void AddNewColumn()
        {
            totalRows++;
        }
    }
}