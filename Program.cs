namespace TextEditor
{
    struct InputKey
    {
        public ConsoleKeyInfo key { get; init; }
        public int row { get; init; }
        public int col { get; init; }

        public InputKey(ConsoleKeyInfo key, int row, int col)
        {
            this.key = key;
            this.row = row;
            this.col = col;
        }
    }
    class TextEditor
    {
        int totalRows;
        int columnsIntheCurrentRow;
        int cursorRow;
        List<InputKey> inputKeys = new List<InputKey>(); 
        
        // contains each row key and the number of coresponding characters on that row 
        Dictionary<int, int> numberOfColumnsPerRow = new Dictionary<int, int>();

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
                switch (consoleInput.Key)
                {
                    case ConsoleKey.Enter:
                        editor.inputKeys.Add(new InputKey(consoleInput, Console.GetCursorPosition().Top, Console.GetCursorPosition().Left));
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
                    case ConsoleKey.Backspace:
                        editor.HandleBackspace();
                        break;
                    default: 
                        editor.inputKeys.Add(new InputKey(consoleInput, Console.GetCursorPosition().Top, Console.GetCursorPosition().Left));
                        editor.CheckForNewRowThroughChar();
                        break;
                }
                //editor.RewriteText();
            } while (consoleInput.Key != ConsoleKey.Escape); //close app on ESC
        }

        private void HandleBackspace()
        {
            //Backspace already knows to move the cursor once to the left
            var (cursorLeft, cursorTop) = Console.GetCursorPosition();
            int keyIndex = cursorLeft;
            for(int i=0; i < cursorTop; i++)
            {
                keyIndex += numberOfColumnsPerRow[i];
            }
            if (keyIndex >= 0)
            {
                inputKeys.RemoveAt(keyIndex);
                if(cursorLeft == numberOfColumnsPerRow[cursorRow]-1)
                {
                    Console.Write(' ');
                    ArrowMoveCursor(-1, 0);
                }
                else
                {
                    int i, offset = inputKeys.Count - cursorLeft;
                    for(i = keyIndex; i < inputKeys.Count; i++)
                        if(inputKeys[i].key.KeyChar == '\r')
                            Console.Write(" \n");
                        else Console.Write(inputKeys[i].key.KeyChar);
                    Console.Write(' ');
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                }
                numberOfColumnsPerRow[cursorRow]--;
            }
            
        }

        private void RewriteText()
        {
            Console.Clear();
            foreach(var key in inputKeys)
            {
                Console.Write(key.key.KeyChar);
            }
        }

        private void ChangeKey<TKey, TValue>(IDictionary<TKey, TValue> dic,
                                      TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            dic[toKey] = value;
        }

        private void CheckForNewRowThroughChar()
        {
            var (_, Top) = Console.GetCursorPosition();
            if (Top != cursorRow)
            {
                cursorRow = Top;
                columnsIntheCurrentRow = 0;
                if (numberOfColumnsPerRow.ContainsKey(cursorRow)){
                    for(int i = numberOfColumnsPerRow.Count-1; i>=cursorRow; i--)
                    {
                        ChangeKey(numberOfColumnsPerRow, i, i + 1);
                    }
                }
                numberOfColumnsPerRow.Add(cursorRow, columnsIntheCurrentRow);
                totalRows++;
            }
            else
            {
                columnsIntheCurrentRow++;
                numberOfColumnsPerRow[cursorRow] = columnsIntheCurrentRow;
            }
        }

        private void EnterMoveCursor()
        {

            columnsIntheCurrentRow++;
            numberOfColumnsPerRow[cursorRow] = columnsIntheCurrentRow;
            cursorRow += 1;
            totalRows++;
            columnsIntheCurrentRow = 0;
            numberOfColumnsPerRow.Add(cursorRow, columnsIntheCurrentRow);
            Console.SetCursorPosition(0, cursorRow);
        }

        private void ArrowMoveCursor(int columnOffset, int rowOffset)
        {
            int newRow = cursorRow + rowOffset;
            int newColumn = Console.GetCursorPosition().Left + columnOffset;
            
            if (NewPositionInsideBounds(newRow, newColumn))
            {
                if(newColumn > numberOfColumnsPerRow[newRow])
                {
                    if(newRow + 1 <= totalRows && rowOffset == 0)
                    {
                        newRow += 1;
                        newColumn = 0;
                    }
                    else
                    {
                        newColumn = numberOfColumnsPerRow[newRow];
                    }
                }
                else if(newColumn < 0){
                    if (newRow - 1 >= 0 && rowOffset == 0)
                    {
                        newRow -= 1;
                        newColumn = numberOfColumnsPerRow[newRow];
                    }
                    else
                    {
                        newColumn = 0;
                    }
                }
                cursorRow = newRow;
                Console.SetCursorPosition(newColumn, newRow);
            }
        }

        private bool NewPositionInsideBounds(int newRow, int newColumn)
        {
            return newRow <= totalRows && newRow >= 0;
        }
    }
}