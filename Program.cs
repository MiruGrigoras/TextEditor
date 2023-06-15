using System;
namespace TextEditor
{
    class TextEditor
    {
        int totalRows;
        int columnsIntheCurrentRow;
        int cursorRow;

        public TextEditor()
        {
             cursorRow = totalRows = Console.CursorTop;
             columnsIntheCurrentRow = Console.CursorLeft;
        }
        static void Main(string[] args)
        {
            TextEditor editor = new();
            ConsoleKeyInfo consoleInput;

            do
            {
                consoleInput = Console.ReadKey();
                switch(consoleInput.Key)
                {
                    case ConsoleKey.Enter:
                        editor.EnterMoveCursor();
                        break;
                    case ConsoleKey.DownArrow:
                        editor.ArrowMoveCursor(0, 1);
                        break;
                    case ConsoleKey.UpArrow:
                        editor.ArrowMoveCursor(0, -1);
                        break;
                    case ConsoleKey.LeftArrow:
                        editor.ArrowMoveCursor(-1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        editor.ArrowMoveCursor(1, 0);
                        break;
                    default: 
                        editor.checkForNewRowThroughChar();
                        break;
                }
            } while (consoleInput.Key != ConsoleKey.Escape); //close app on ESC
        }

        private void checkForNewRowThroughChar()
        {
            var currentPos = Console.GetCursorPosition();
            if (currentPos.Top != cursorRow)
            {
                cursorRow = currentPos.Top;
                columnsIntheCurrentRow = 0;
                totalRows++;
            }
            else
            {
                columnsIntheCurrentRow++;
            }
        }

        private void EnterMoveCursor()
        {
            cursorRow += 1;
            totalRows++;
            columnsIntheCurrentRow = 0;
            Console.SetCursorPosition(0, cursorRow);
        }

        private void ArrowMoveCursor(int columnOffset, int rowOffset)
        {
            int newRow = cursorRow + rowOffset;
            int newColumn = Console.GetCursorPosition().Left + columnOffset;
            if (NewPositionInsideBounds(newRow, newColumn))
            {
                cursorRow = newRow;
                Console.SetCursorPosition(newColumn, newRow);
            }
        }

        private bool NewPositionInsideBounds(int newRow, int newColumn)
        {
            return newRow <= totalRows && newRow >= 0 && newColumn <= columnsIntheCurrentRow && newColumn >= 0;
        }
    }
}