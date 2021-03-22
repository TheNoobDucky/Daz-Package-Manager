using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Helpers
{
    public class Output
    {
        public static void RegisterDebugField(FlowDocument debugField)
        {
            Output.debugField = debugField;
        }
        public static void Write(string debugText, Brush brush =  null, double indent = 0.0)
        {
            if (brush == null)
            {
                brush = Brushes.Black;
            }
            if (WriteDebug) // debugField == null error handling not implemented
            {
                //debugField.Text += debugText + '\n';
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
