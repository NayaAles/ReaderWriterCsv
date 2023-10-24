using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ReaderWriterCsv
{
    public static class ReaderWriterCsv
    {
        // If class.fieldName.Contains("Id") => not included
        // Don't use titles in input
        public static List<T> ReadFromCsv<T>(string path, char separator)
        {
            var inDatas = new List<T>();

            var fields = GetFields<T>()
                .Where(x => !x.Contains("<Id>"))
                .ToList();
            var fieldsCount = fields
                .Count();

            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!String.IsNullOrEmpty(line))
                        inDatas.Add(ConvertToData<T>(fields, fieldsCount, line, separator));
                }
            }

            return inDatas;
        }

        private static T ConvertToData<T>(List<string> fields, int fieldsCount, string line, char separator)
        {
            var item = (T)Activator.CreateInstance(typeof(T));

            while (Regex.Match(line, @$"{separator}{separator}").Success)
                line = Regex.Replace(line, @$"{separator}{separator}", $"{separator} {separator}")
                    .ToString();

            var array = Regex.Matches(line, $"\"([^\"]+).|[^{separator}]+")
                .Cast<Match>()
                .Select(x => Regex.Replace(x.ToString(), "\"", string.Empty))
                .ToArray();

            if (fieldsCount - 1 == array.Length)
                fieldsCount = array.Length;

            for (int i = 0; i < fieldsCount; i++)
            {
                var rightData = array[i].Equals(" ") ? null :
                    (array[i].Equals("") ? null : array[i].Trim());

                if (item != null)
                {
                    var value = item.GetType()
                      .GetField(fields[i], BindingFlags.Instance | BindingFlags.NonPublic);

                    if (value != null && !String.IsNullOrEmpty(rightData))
                    {
                        var fieldType = value.FieldType;
                        var resultData = TypeDescriptor.GetConverter(fieldType)
                            .ConvertFrom(rightData);

                        value.SetValue(item, resultData);
                    }
                }
            }

            return item;
        }

        // Экранирование if class.field.Contains(separator)
        public static void SaveToCsv<T>(List<T> outDatas, string pathOut, char separator, bool addTitles)
        {
            var fields = GetFields<T>()
                .Where(x => !x.Contains("<Id>"))
                .ToList();
            var fieldsCount = fields
                .Count();

            var titles = fields
                .Select(x => Regex.Match(x, @"<([^>]+)").Groups[1].ToString())
                .ToList();

            using (var writer = new StreamWriter(pathOut))
            {
                if (addTitles)
                    writer.WriteLine(String.Join($"{separator}", titles));

                for (int i = 0; i < outDatas.Count; i++)
                {
                    string exit = "";
                    for (int j = 0; j < fieldsCount; j++)
                    {
                        string value = "";
                        var item = outDatas[i].GetType()
                        .GetField(fields[j], BindingFlags.Instance | BindingFlags.NonPublic);

                        if (item != null)
                        {
                            var itemValue = item.GetValue(outDatas[i]);
                            if (itemValue != null)
                                value = itemValue.ToString();
                        }

                        if (j == fieldsCount - 1)
                            exit = String.Concat(exit, value.Contains(separator) ? $"\"{value}\"" : value);
                        else
                            exit = String.Concat(exit, value.Contains(separator) ? $"\"{value}\"" : value, separator);
                    }

                    writer.WriteLine(exit);
                }
            }
        }

        private static string[] GetFields<T>()
        {
            var fields = typeof(T).GetRuntimeFields()
                .Select(x => x.Name)
                .ToArray();

            return fields;
        }
    }
}

