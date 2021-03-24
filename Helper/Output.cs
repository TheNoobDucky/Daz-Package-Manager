using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Helpers
{
    public class Output
    {
        public enum Level
        {
            None,
            Status,
            Alert,
            Info,
            Debug,
            Warning,
            Error,
            Bug,
        }

        public static void RegisterDebugField(FlowDocument debugField)
        {
            Output.debugField = debugField;
        }
        public static void Write(string debugText, Level level, double indent = 0.0)
        {
            var brush = level switch
            {
                Level.Status => Brushes.Green,
                Level.Alert => Brushes.Black,
                Level.Info => Brushes.Gray,
                Level.Debug => Brushes.Blue,
                Level.Warning => Brushes.Orange,
                Level.Error => Brushes.Red,
                Level.Bug => Brushes.Yellow,
                _ => Brushes.White,
            };

            if (level == Level.Info)
            {
                indent = 20.0;
            }

            Write(debugText, brush, indent);
        }


        private static void Write(string debugText, Brush brush = null, double indent = 0.0)
        {
            if (brush == null)
            {
                brush = Brushes.Black;
            }
            if (WriteDebug) // debugField == null error handling not implemented
            {
                debugField.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var paragraph = new Paragraph()
                    {
                        Padding = new Thickness(0),
                        Margin = new Thickness(0)
                    };
                    paragraph.TextIndent = indent;
                    paragraph.Inlines.Add(new Run(debugText)
                    {
                        Foreground = brush
                    });

                    debugField.Blocks.Add(paragraph);
                }));
            }
        }
        public static bool WriteDebug { get; set; }
        private static FlowDocument debugField;
    }
}
