using BinaryKits.Zpl.Protocol.Commands;
using BinaryKits.Zpl.Viewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BinaryKits.Zpl.Viewer
{
    public class ZplAnalyzerNew
    {
        private readonly string _labelStartCommand = "^XA";
        private readonly string _labelEndCommand = "^XZ";
        private readonly List<CommandBase> _commands;

        public ZplAnalyzerNew()
        {
            this._commands = new List<CommandBase>
            {
                new BarCodeFieldDefaultCommand(),
                new ChangeAlphanumericDefaultFontCommand(),
                new Code128BarCodeCommand(),
                new Code39BarCodeCommand(),
                new CommentCommand(),
                new DownloadGraphicsCommand(),
                new DownloadObjectsCommand(),
                new FieldBlockCommand(),
                new FieldDataCommand(),
                new FieldOriginCommand(),
                new FieldReversePrintCommand(),
                new FieldSeperatorCommand(),
                new FieldTypesetCommand(),
                new GraphicBoxCommand(),
                new GraphicCircleCommand(),
                new GraphicFieldCommand(),
                new ImageMoveCommand(),
                new Interleaved2Of5BarCodeCommand(),
                new LabelHomeCommand(),
                new QrCodeBarCodeCommand(),
                new RecallGraphicCommand(),
                new ScalableBitmappedFontCommand()
            };
        }

        public AnalyzeInfo1 Analyze(string zplData)
        {
            var zplCommands = this.SplitZplCommands(zplData);
            var unknownCommands = new List<string>();
            var errors = new List<string>();

            var labelInfos = new List<LabelInfo1>();

            var elements = new List<CommandBase>();
            for (var i = 0; i < zplCommands.Length; i++)
            {
                var currentCommand = zplCommands[i];

                if (this._labelStartCommand.Equals(currentCommand, StringComparison.OrdinalIgnoreCase))
                {
                    elements.Clear();
                    continue;
                }

                if (this._labelEndCommand.Equals(currentCommand, StringComparison.OrdinalIgnoreCase))
                {
                    labelInfos.Add(new LabelInfo1
                    {
                        ZplElements = elements.ToArray()
                    });
                    continue;
                }

                var command = this._commands.Where(o => o.IsCommandParsable(currentCommand)).SingleOrDefault();
                if (command == null)
                {
                    unknownCommands.Add(currentCommand);
                    continue;
                }

                var commandInstance = (CommandBase)Activator.CreateInstance(command.GetType());

                try
                {
                    commandInstance.ParseCommand(currentCommand);
                }
                catch (Exception exception)
                {
                    errors.Add($"Cannot parse command {currentCommand} {exception}");
                }

                elements.Add(commandInstance);
            }

            var analyzeInfo = new AnalyzeInfo1
            {
                LabelInfos = labelInfos.ToArray(),
                UnknownCommands = unknownCommands.ToArray(),
                Errors = errors.ToArray()
            };

            return analyzeInfo;
        }

        private string[] SplitZplCommands(string zplData)
        {
            if (string.IsNullOrEmpty(zplData))
            {
                return Array.Empty<string>();
            }

            var replacementString = string.Empty;
            var cleanZpl = Regex.Replace(zplData, @"\r\n?|\n", replacementString);
            return Regex.Split(cleanZpl, "(?=\\^)|(?=\\~)").Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }
    }
}
