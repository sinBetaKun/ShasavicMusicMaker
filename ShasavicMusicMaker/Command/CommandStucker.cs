namespace ShasavicMusicMaker.Command
{
    internal class CommandStucker
    {
        private List<IEditorCommand> commands = [];

        private int commandIndex = -1;

        public int StuckMax { get; private set; } = 50;

        public bool CanUndo => commandIndex >= 0;

        public bool CanRedo => commandIndex + 1 < commands.Count;

        /// <summary>
        /// コマンドが登録されたときに実行される。
        /// </summary>
        public event EventHandler CommandSubscribed;

        /// <summary>
        /// 新たに実行するコマンドを登録する。
        /// </summary>
        /// <param name="command">実行し登録するコマンド</param>
        public void SubscribeCommand(IEditorCommand command)
        {
            if (commandIndex + 1 < commands.Count)
            {
                commands.RemoveRange(commandIndex + 1, commands.Count - commandIndex - 1);
            }
            else if (commands.Count >= StuckMax)
                commands.Remove(commands.First());

            commands.Add(command);
            command.Execute();
            commandIndex++;

            CommandSubscribed?.Invoke(this, EventArgs.Empty);
        }

        public void Undo()
        {
            if (CanUndo)
                commands[commandIndex--].Undo();
        }

        public void Redo()
        {
            if (CanRedo)
                commands[++commandIndex].Execute();
        }

        public void ChangeStuckMax(int stuckMax)
        {
            if (stuckMax < commands.Count)
            {
                int sub = commands.Count - stuckMax;
                commands.RemoveRange(0, sub);
                commandIndex -= sub;
            }

            StuckMax = stuckMax;
        }
    }
}
