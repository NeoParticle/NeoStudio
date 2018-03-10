using NeoStudio.View;
using ScintillaNET;
using ScintillaNET.WPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NeoStudio
{
    internal static partial class ScintillaMethods
    {
        internal static void InitBase(DocumentView doc)
        {
            ScintillaWPF scintillaNet = doc.scintilla;
            scintillaNet.WrapMode = WrapMode.None;
            scintillaNet.IndentationGuides = IndentView.LookBoth;

            InitSyntaxColoring(scintillaNet);
            InitBookmarkMargin(scintillaNet);
            InitCodeFolding(scintillaNet);
            InitColors(scintillaNet);
            InitNumberMargin(scintillaNet);

        }

        internal static void InitBookmarkMargin(ScintillaWPF scintillaNet)
        {
            var margin = scintillaNet.Margins[ScintillaConstants.BREAKPOINT_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << ScintillaConstants.BREAKPOINT_MARKER);
            margin.Cursor = MarginCursor.Arrow;

            var marker = scintillaNet.Markers[ScintillaConstants.BREAKPOINT_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(IntToColor(0xAB616B));
            marker.SetForeColor(IntToColor(0xFFFFFF));
            marker.SetAlpha(100);

            marker = scintillaNet.Markers[ScintillaConstants.BREAKPOINT_BG];
            marker.Symbol = MarkerSymbol.Background;
            marker.SetBackColor(IntToColor(0xAB616B));

            scintillaNet.Indicators[ScintillaConstants.BREAKPOINT_INDICATOR].Style = IndicatorStyle.StraightBox;
            scintillaNet.Indicators[ScintillaConstants.BREAKPOINT_INDICATOR].Under = true;
            scintillaNet.Indicators[ScintillaConstants.BREAKPOINT_INDICATOR].ForeColor = System.Drawing.Color.Maroon;
            scintillaNet.Indicators[ScintillaConstants.BREAKPOINT_INDICATOR].OutlineAlpha = 50;
            scintillaNet.Indicators[ScintillaConstants.BREAKPOINT_INDICATOR].Alpha = 30;

            scintillaNet.Indicators[ScintillaConstants.ERROR_INDICATOR].Style = IndicatorStyle.SquiggleLow;
            scintillaNet.Indicators[ScintillaConstants.ERROR_INDICATOR].Under = true;
            scintillaNet.Indicators[ScintillaConstants.ERROR_INDICATOR].ForeColor = System.Drawing.Color.Red;
            scintillaNet.Indicators[ScintillaConstants.ERROR_INDICATOR].OutlineAlpha = 50;
            scintillaNet.Indicators[ScintillaConstants.ERROR_INDICATOR].Alpha = 30;
        }

        internal static void InitCodeFolding(ScintillaWPF scintillaNet)
        {
            scintillaNet.SetFoldMarginColor(true, IntToMediaColor(ScintillaConstants.FORE_COLOR));
            scintillaNet.SetFoldMarginHighlightColor(true, IntToMediaColor(0x2A211C));

            // Enable code folding
            scintillaNet.SetProperty("fold", "1");
            scintillaNet.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            scintillaNet.Margins[ScintillaConstants.FOLDING_MARGIN].Type = MarginType.Symbol;
            scintillaNet.Margins[ScintillaConstants.FOLDING_MARGIN].Mask = Marker.MaskFolders;
            scintillaNet.Margins[ScintillaConstants.FOLDING_MARGIN].Sensitive = true;
            scintillaNet.Margins[ScintillaConstants.FOLDING_MARGIN].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                scintillaNet.Markers[i].SetForeColor(IntToColor(ScintillaConstants.BACK_COLOR)); // styles for [+] and [-]
                scintillaNet.Markers[i].SetBackColor(IntToColor(ScintillaConstants.FORE_COLOR)); // styles for [+] and [-]
            }

            // Configure folding markers with respective symbols
            scintillaNet.Markers[Marker.Folder].Symbol = ScintillaConstants.CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            scintillaNet.Markers[Marker.FolderOpen].Symbol = ScintillaConstants.CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            scintillaNet.Markers[Marker.FolderEnd].Symbol = ScintillaConstants.CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            scintillaNet.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintillaNet.Markers[Marker.FolderOpenMid].Symbol = ScintillaConstants.CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            scintillaNet.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintillaNet.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            scintillaNet.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }

        internal static void InitColors(ScintillaWPF scintillaNet)
        {
            scintillaNet.CaretForeColor = System.Windows.Media.Colors.Black;
            scintillaNet.SetSelectionBackColor(true, IntToMediaColor(0x99C9EF));
        }

        internal static void InitNumberMargin(ScintillaWPF scintillaNet)
        {
            scintillaNet.Styles[ScintillaNET.Style.LineNumber].BackColor = IntToColor(ScintillaConstants.BACK_COLOR);
            scintillaNet.Styles[ScintillaNET.Style.LineNumber].ForeColor = IntToColor(ScintillaConstants.FORE_COLOR);
            scintillaNet.Styles[ScintillaNET.Style.IndentGuide].ForeColor = IntToColor(ScintillaConstants.FORE_COLOR);
            scintillaNet.Styles[ScintillaNET.Style.IndentGuide].BackColor = IntToColor(ScintillaConstants.BACK_COLOR);

            var nums = scintillaNet.Margins[ScintillaConstants.NUMBER_MARGIN];
            nums.Width = ScintillaConstants.LINE_NUMBERS_MARGIN_WIDTH;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            scintillaNet.MarginClick += TextArea_MarginClick;
        }

        internal static void InitSyntaxColoring(ScintillaWPF scintillaNet)
        {
            scintillaNet.StyleResetDefault();
            scintillaNet.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            scintillaNet.Styles[ScintillaNET.Style.Default].Size = 10;
            scintillaNet.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            scintillaNet.Styles[ScintillaNET.Style.Cpp.Default].ForeColor = System.Drawing.Color.Silver;
            scintillaNet.Styles[ScintillaNET.Style.Cpp.Comment].ForeColor = System.Drawing.Color.FromArgb(0, 128, 0); // Green
            scintillaNet.Styles[ScintillaNET.Style.Cpp.CommentLine].ForeColor = System.Drawing.Color.FromArgb(0, 128, 0); // Green
            scintillaNet.Styles[ScintillaNET.Style.Cpp.CommentLineDoc].ForeColor = System.Drawing.Color.FromArgb(128, 128, 128); // Gray
            scintillaNet.Styles[ScintillaNET.Style.Cpp.Number].ForeColor = System.Drawing.Color.Olive;
            scintillaNet.Styles[ScintillaNET.Style.Cpp.Word].ForeColor = System.Drawing.Color.Blue;
            scintillaNet.Styles[ScintillaNET.Style.Cpp.Word2].ForeColor = System.Drawing.Color.FromArgb(43, 145, 175);
            scintillaNet.Styles[ScintillaNET.Style.Cpp.String].ForeColor = System.Drawing.Color.FromArgb(163, 21, 21); // Red
            scintillaNet.Styles[ScintillaNET.Style.Cpp.Character].ForeColor = System.Drawing.Color.FromArgb(163, 21, 21); // Red
            scintillaNet.Styles[ScintillaNET.Style.Cpp.Verbatim].ForeColor = System.Drawing.Color.FromArgb(163, 21, 21); // Red
            scintillaNet.Styles[ScintillaNET.Style.Cpp.StringEol].BackColor = System.Drawing.Color.Pink;
            scintillaNet.Styles[ScintillaNET.Style.Cpp.Operator].ForeColor = System.Drawing.Color.Purple;
            scintillaNet.Styles[ScintillaNET.Style.Cpp.Preprocessor].ForeColor = System.Drawing.Color.Maroon;
            scintillaNet.Lexer = Lexer.Cpp;

            // Set the keywords
            scintillaNet.SetKeywords(0, "abstract as base break case catch checked continue default delegate do else event explicit extern false finally fixed for foreach goto if implicit in interface internal is lock namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc switch this throw true try typeof unchecked unsafe using virtual while bool byte char class const decimal double enum float int long sbyte short static string struct uint ulong ushort void BigInteger");
            scintillaNet.SetKeywords(1, "DisplayName");
            //bool byte char class const decimal double enum float int long sbyte short static string struct uint ulong ushort void BigInteger
            scintillaNet.SetKeywords(2, "DisplayName");

            scintillaNet.CharAdded += ScintillaNet_CharAdded;
            scintillaNet.InsertCheck += ScintillaNet_InsertCheck;
        }

        #region Events
        private static void TextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            ScintillaNET.WPF.ScintillaWPF scintilla = ((MainWindow)App.Current.MainWindow).ActiveDocument.scintilla;

            if (e.Margin == ScintillaConstants.BREAKPOINT_MARGIN)
            {
                const uint mask = (1 << ScintillaConstants.BREAKPOINT_MARKER);
                Line line = scintilla.Lines[scintilla.LineFromPosition(e.Position)];

                scintilla.IndicatorCurrent = ScintillaConstants.BREAKPOINT_INDICATOR;
                if ((line.MarkerGet() & mask) > 0)
                {
                    line.MarkerDelete(ScintillaConstants.BREAKPOINT_MARKER);
                    line.MarkerDelete(ScintillaConstants.BREAKPOINT_BG);
                    scintilla.IndicatorClearRange(line.Position, line.Length);
                }
                else
                {
                    int breakPointStartPosition, breakPointHighlightLength;
                    if (ScintillaBreakPointValidator.IsBreakPointAllowed(scintilla, line, out breakPointStartPosition, out breakPointHighlightLength))
                    {
                        line.MarkerAdd(ScintillaConstants.BREAKPOINT_MARKER);
                        scintilla.IndicatorFillRange(breakPointStartPosition, breakPointHighlightLength);
                    }
                }
            }

        }

        private static void ScintillaNet_InsertCheck(object sender, InsertCheckEventArgs e)
        {
            DocumentView documentView = ((MainWindow)App.Current.MainWindow).ActiveDocument;
            if (documentView == null)
                return;

            ScintillaNET.WPF.ScintillaWPF scintilla = documentView.scintilla;
            var currentPos = scintilla.CurrentPosition;
            if (e.Text == "\u0013")
            {
                e.Text = string.Empty;
                return;
            }
            
            if ((e.Text.EndsWith("\r") || e.Text.EndsWith("\n")))
            {
                var curLine = scintilla.LineFromPosition(e.Position);
                var curLineText = scintilla.Lines[curLine].Text.Replace("\r", "").Replace("\n","");

                var indent = Regex.Match(curLineText, @"^\s*");
                e.Text += indent.Value;

                if (Regex.IsMatch(curLineText, @"{\s*$"))
                    e.Text += '\t';
            }
        }

        private static void ScintillaNet_CharAdded(object sender, CharAddedEventArgs e)
        {
            DocumentView documentView = ((MainWindow)App.Current.MainWindow).ActiveDocument;
            if (documentView == null)
                return;

            if (e.Char == 19)
            {
                documentView.Save();
                return;
            }
            InsertMatchedChars(e);
            CSharp.Compile();
        }
        #endregion


        private static void InsertMatchedChars(CharAddedEventArgs e)
        {
            ScintillaNET.WPF.ScintillaWPF scintilla = ((MainWindow)App.Current.MainWindow).ActiveDocument.scintilla;
            var caretPos = scintilla.CurrentPosition;
            var docStart = caretPos == 1;
            var docEnd = caretPos == scintilla.Text.Length;

            var charPrev = docStart ? scintilla.GetCharAt(caretPos) : scintilla.GetCharAt(caretPos - 2);
            var charNext = scintilla.GetCharAt(caretPos);

            var isCharPrevBlank = charPrev == ' ' || charPrev == '\t' ||
                                  charPrev == '\n' || charPrev == '\r';

            var isCharNextBlank = charNext == ' ' || charNext == '\t' ||
                                  charNext == '\n' || charNext == '\r' ||
                                  docEnd;

            var isEnclosed = (charPrev == '(' && charNext == ')') ||
                                  (charPrev == '{' && charNext == '}') ||
                                  (charPrev == '[' && charNext == ']');

            var isSpaceEnclosed = (charPrev == '(' && isCharNextBlank) || (isCharPrevBlank && charNext == ')') ||
                                  (charPrev == '{' && isCharNextBlank) || (isCharPrevBlank && charNext == '}') ||
                                  (charPrev == '[' && isCharNextBlank) || (isCharPrevBlank && charNext == ']');

            var isCharOrString = (isCharPrevBlank && isCharNextBlank) || isEnclosed || isSpaceEnclosed;

            var charNextIsCharOrString = charNext == '"' || charNext == '\'';

            switch (e.Char)
            {
                case '(':
                    if (charNextIsCharOrString) return;
                    scintilla.InsertText(caretPos, ")");
                    break;
                case '{':
                    if (charNextIsCharOrString) return;
                    scintilla.InsertText(caretPos, "}");
                    break;
                case '[':
                    if (charNextIsCharOrString) return;
                    scintilla.InsertText(caretPos, "]");
                    break;
                case '"':
                    // 0x22 = "
                    if (charPrev == 0x22 && charNext == 0x22)
                    {
                        scintilla.DeleteRange(caretPos, 1);
                        scintilla.GotoPosition(caretPos);
                        return;
                    }

                    if (isCharOrString)
                        scintilla.InsertText(caretPos, "\"");
                    break;
                case '\'':
                    // 0x27 = '
                    if (charPrev == 0x27 && charNext == 0x27)
                    {
                        scintilla.DeleteRange(caretPos, 1);
                        scintilla.GotoPosition(caretPos);
                        return;
                    }

                    if (isCharOrString)
                        scintilla.InsertText(caretPos, "'");
                    break;
            }
        }

        /// <summary>
        /// Converts a Win32 colour to a Drawing.Color
        /// </summary>
        /// <param name="rgb">A Win32 color.</param>
        /// <returns>A System.Drawing color.</returns>
        public static System.Drawing.Color IntToColor(int rgb)
        {
            return System.Drawing.Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        /// <summary>
        /// Converts a Win32 colour to a Media Color
        /// </summary>
        /// <param name="rgb">A Win32 color.</param>
        /// <returns>A System.Media color.</returns>
        public static System.Windows.Media.Color IntToMediaColor(int rgb)
        {
            return System.Windows.Media.Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }
    }
}
