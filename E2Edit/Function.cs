using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace E2Edit
{
    internal enum DataType
    {
        Number,
        String,
        Entity,
        Vector2,
        Vector3,
        Vector4,
        Matrix2,
        Matrix3,
        Matrix4,
        Angle,
        Table,
        Array,
        Bone,
        Wirelink,
        Complex,
        Quaternion,
    }

    [XmlRoot("FunctionList")]
    public class FunctionList
    {
        [XmlElement("Function")]
        public Function[] Functions;
    }

    [XmlRoot]
    public class Function
    {
        [XmlElement]
        public string Arguments;
        [XmlAttribute]
        public string Name;
        [XmlElement]
        public string Description;

        public static IEnumerable<Function> LoadData(Stream stream)
        {
            
            return ((FunctionList) new XmlSerializer(typeof(FunctionList)).Deserialize(stream)).Functions;
        }

        private static IEnumerable<DataType> GetE2Types(IEnumerable<string> value)
        {
// ReSharper disable PossibleInvalidOperationException
            return (from t in value select GetE2Type(t) into dataType where dataType.HasValue select dataType.Value);
// ReSharper restore PossibleInvalidOperationException
        }

        private static DataType? GetE2Type(string s)
        {
            switch (s)
            {
                case "N":
                    return DataType.Number;
                case "S":
                    return DataType.String;
                case "E":
                    return DataType.Entity;
                case "V2":
                    return DataType.Vector2;
                case "V":
                    return DataType.Vector3;
                case "V4":
                    return DataType.Vector4;
                case "M2":
                    return DataType.Matrix2;
                case "M":
                    return DataType.Matrix3;
                case "M4":
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
                    return DataType.Wirelink;
                case "C":
                    return DataType.Complex;
                case "Q":
                    return DataType.Quaternion;
                default:
                    return null;
            }
        }
    }
}