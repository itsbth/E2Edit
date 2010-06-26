using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Xml.Serialization;
using E2Edit.Resources;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;

namespace E2Edit.Editor
{
    public enum DataType
    {
        Void,
        Number,
        String,
        Entity,
        Vector2,
        Vector,
        Vector4,
        Matrix2,
        Matrix,
        Matrix4,
        Angle,
        Table,
        Array,
        Bone,
        WireLink,
        ComplexNumber,
        Quaternion,
    }

    [XmlRoot("FunctionList")]
    public class FunctionList
    {
        [XmlElement("Function")] public Function[] Functions;
    }

    [XmlRoot]
    public class Function : ICompletionData
    {
        private const string Icon = "E2.Icons.Type.";
        [XmlIgnore] public IList<DataType> Arguments;
        [XmlIgnore] public DataType ThisType;
        [XmlAttribute] public string Name;
        [XmlElement] public string Description;
        [XmlElement] public string Return;

        #region ICompletionData Members

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            var context = new InsertionContext(textArea, completionSegment.Offset);
            var sni = new SnippetContainerElement();
            sni.Elements.Add(new SnippetTextElement{Text = Name + "("});
            foreach (var argument in Arguments)
            {
                var element = new SnippetReplaceableTextElement{Text = argument.ToString()};
                sni.Elements.Add(element);
                sni.Elements.Add(new SnippetTextElement{Text = ", "});
            }
            if (Arguments.Count > 0) sni.Elements.RemoveAt(sni.Elements.Count - 1);
            sni.Elements.Add(new SnippetTextElement{Text = ")"});
            textArea.Document.Remove(completionSegment);
            sni.Insert(context);
            context.RaiseInsertionCompleted(null);
        }

        public ImageSource Image
        {
            get { return Resource.GetBitmapImage(Icon + GetE2Type(Return)); }
        }

        public string Text
        {
            get { return Name; }
        }

        public object Content
        {
            get { return Name; }
        }

        object ICompletionData.Description
        {
            get
            {
                var b = new TextBlock();
                b.Inlines.Add(new InlineUIContainer(GetDatatypeIcon(ThisType)){BaselineAlignment = BaselineAlignment.Center, });
                b.Inlines.Add(":" + Name + "(");
                foreach (var argument in Arguments)
                {
                    b.Inlines.Add(new InlineUIContainer(GetDatatypeIcon(argument)) { BaselineAlignment = BaselineAlignment.Center });
                    b.Inlines.Add(", ");
                }
                if (Arguments.Count > 0) b.Inlines.Remove(b.Inlines.LastInline);
                b.Inlines.Add(")");
                b.Inlines.Add(new LineBreak());
                b.Inlines.Add(!String.IsNullOrEmpty(Description) ? Description : "<No description>");
                return b;
            }
        }

        private Image GetDatatypeIcon(DataType argument)
        {
            return new Image { Source = Resource.GetBitmapImage(Icon + argument), Width = 16.0d, Height = 16.0d };
        }

        private string GetArgumentString()
        {
            if (Arguments.Count == 0) return "";
            var buff = new StringBuilder();
            foreach (var argument in Arguments)
            {
                buff.Append(argument);
                buff.Append(", ");
            }
            return buff.Remove(buff.Length - 2, 2).ToString();
        }

        public double Priority
        {
            get { return 1.0d; }
        }

        #endregion

        [XmlElement(ElementName = "Arguments")]
        public string ArgumentsString
        {
            get
            {
                return "";
            }
            set
            {
                if (value.IndexOf(':') != -1)
                {
                    string[] parts = value.Split(new[] {':'}, 2);
                    ThisType = GetE2Type(parts[0]);
                    value = parts.Length > 1 ? parts[1] : String.Empty;
                }
                else
                {
                    ThisType = DataType.Void;
                }
                var split = new Regex("(X[A-Z]{2}|[A-Z][0-9]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                MatchCollection matches = split.Matches(value);
                IList<string> argList = new List<string>(matches.Count);
                foreach (var match in matches)
                {
                    argList.Add(((Match) match).Captures[0].Value);
                }
                Arguments = GetE2Types(argList);
            }
        }

        public static IList<Function> LoadData(Stream stream)
        {
            return ((FunctionList) new XmlSerializer(typeof (FunctionList)).Deserialize(stream)).Functions;
        }

        private static IList<DataType> GetE2Types(IEnumerable<string> value)
        {
// ReSharper disable PossibleInvalidOperationException
            return (from t in value select GetE2Type(t)).ToList();
// ReSharper restore PossibleInvalidOperationException
        }

        private static DataType GetE2Type(string s)
        {
            switch (s.ToUpper())
            {
                case "N":
                    return DataType.Number;
                case "S":
                    return DataType.String;
                case "E":
                    return DataType.Entity;
                case "XV2":
                    return DataType.Vector2;
                case "V":
                    return DataType.Vector;
                case "XV4":
                    return DataType.Vector4;
                case "XM2":
                    return DataType.Matrix2;
                case "M":
                    return DataType.Matrix;
                case "XM4":
                    return DataType.Matrix4;
                case "A":
                    return DataType.Angle;
                case "T":
                    return DataType.Table;
                case "R":
                    return DataType.Array;
                case "B":
                    return DataType.Bone;
                case "XWL":
                    return DataType.WireLink;
                case "C":
                    return DataType.ComplexNumber;
                case "Q":
                    return DataType.Quaternion;
                default:
                    return DataType.Void;
            }
        }
    }
}