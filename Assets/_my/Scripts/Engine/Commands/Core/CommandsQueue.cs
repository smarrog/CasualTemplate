using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Smr.Extensions;

namespace Smr.Commands {
    public class CommandsQueue : AbstractCommand {
        private class CommandHolder {
            public AbstractCommand Command { get; }
            public bool IsSyncPoint { get; }
            public CommandHolder(AbstractCommand command, bool isSyncPoint) {
                Command = command;
                IsSyncPoint = isSyncPoint;
            }
        }

        private readonly Queue<CommandHolder> _commands = new(); // полная очередь команд
        private readonly Queue<AbstractCommand> _currentlyExecutedCommands = new();
        private readonly List<UniTask<CommandResult>> _currentTasks = new(); // Promise для исполняемых в данный момент команд

        public CommandsQueue AddCommand(AbstractCommand command, bool isSyncPoint = true) {
            _commands.Enqueue(new CommandHolder(command, isSyncPoint));
            return this;
        }

        protected override void ExecuteInternal() {
            ExecuteCommands().HandleException();
        }

        private async UniTask ExecuteCommands() {
            while (_commands.Count > 0) {
                var commandHolder = _commands.Dequeue();
                var command = commandHolder.Command;
                _currentlyExecutedCommands.Enqueue(command);
                _currentTasks.Add(command.Execute());

                if (_commands.Count == 0 || commandHolder.IsSyncPoint) {
                    await WaitForExecutedCommands();
                }
            }
            NotifyComplete();
        }

        private async UniTask WaitForExecutedCommands() {
            var results = await UniTask.WhenAll(_currentTasks);
            _currentTasks.Clear();
            _currentlyExecutedCommands.Clear();

            foreach (var commandResult in results) {
                if (commandResult != CommandResult.Completed) {
                    NotifyFail();
                }
            }
        }

        protected override void CleanUpInternal(CommandResult result) {
            while (_currentlyExecutedCommands.Count > 0) {
                _currentlyExecutedCommands.Dequeue().Terminate();
            }
            while (_commands.Count > 0) {
                _commands.Dequeue().Command.Terminate();
            }
        }
    }
}