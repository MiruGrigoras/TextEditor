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
                        editor.ArrowMoveCursor(0, 1);
                        break;
                    case ConsoleKey.UpArrow:
                        editor.ArrowMoveCursor(0, -1);
                        break;
                    case ConsoleKey.LeftArrow:
                        editor.ArrowMoveCursor(1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        editor.ArrowMoveCursor(-1, 0);
                        break;
                    default: break;
                }
                editor.checkForNewRowThroughChar();


            } while (consoleInput.Key != ConsoleKey.Escape); //close app on ESC
            
        }

        private void checkForNewRowThroughChar()
        {
            var currentPos = Console.GetCursorPosition();
            if(currentPos.Top != cursorRow)
            {
                cursorRow = currentPos.Top;
                totalRows++;
            }
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
    }
}