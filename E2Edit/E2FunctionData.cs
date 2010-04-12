using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace E2Edit
{
    internal enum DataType
    {
        Normal,
        Entity,
        Vector,
        Array,
        Angle
    }

    internal class E2FunctionData
    {
        public IEnumerable<DataType> Arguments;
        public string Name;
        public DataType? ReturnType;
        public DataType? ThisType;

        public static IEnumerable<E2FunctionData> LoadData(Stream stream)
        {
            var retVal = new List<E2FunctionData>();
            var streamReader = new StreamReader(stream);
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                Match match = Regex.Match(line, @"(?<Name>[a-z][A-Za-z]*)\(((?<This>[a-z]):)?(?<Arguments>[a-z]+)?\)",
                                          RegexOptions.Compiled);
                if (match.Success)
                {
                    Debug.WriteLine("{0}:{1}({2})", match.Groups["This"], match.Groups["Name"],
                                    match.Groups["Arguments"]);
                    retVal.Add(new E2FunctionData
                                   {
                                       Name = match.Groups["Name"].Value,
                                       ThisType = GetE2Type(match.Groups["This"].Value),
                                       Arguments = GetE2Types(match.Groups["Arguments"].Value)
                                   });
                }
                else Debug.WriteLine("Line '{0}' did not match.", new[] {line});
            }
            return retVal;
        }

        private static IEnumerable<DataType> GetE2Types(IEnumerable<char> value)
        {
// ReSharper disable PossibleInvalidOperationException
            return (from t in value select GetE2Type(t) into dataType where dataType.HasValue select dataType.Value);
// ReSharper restore PossibleInvalidOperationException
        }

        private static DataType? GetE2Type(string s)
        {
            return string.IsNullOrEmpty(s) ? null : GetE2Type(s[0]);
        }

        private static DataType? GetE2Type(char p)
        {
            switch (p)
            {
                case 'e':
                    return DataType.Entity;
                case 'n':
                    return DataType.Normal;
                case 'v':
                    return DataType.Vector;
                case 'a':
                    return DataType.Angle;
                default:
                    return null;
            }
        }
    }
}