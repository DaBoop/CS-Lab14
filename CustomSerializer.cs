using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using System.IO;
//using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;

namespace Lab14
{
   
    public static class CustomSerializer 
    {
        //static string commonFileName = "serialize.txt";
        static private void Trim(ref string s)
        {
            int length = s.Length;
            if (s[0] == '\"' && s[length-1] == '\"')
            {
                string temp = "";
                for (int i = 1; i < length - 1; i++)
                    temp += s[i];
                s = temp;
            }

            
        }

        static private object Parse(object obj, string[] propOrFieldData, Type elemType = null) // propOrFieldData[0] has to be Type of property or field, [1] - value, if indexed - [1],[2] - indexed values
        {
            Type objType = obj.GetType();
            PropertyInfo propInfo = objType.GetProperty(propOrFieldData[0]);
            FieldInfo fieldInfo = objType.GetField(propOrFieldData[0]);
            if (propOrFieldData.Length == 2)
            {
                
                if (propInfo != null)
                {
                    if (propInfo.CanWrite == false) return obj;
                    var value = System.Convert.ChangeType(propOrFieldData[1], propInfo.PropertyType);
                    propInfo.SetValue(obj, value);
                }
                else
                {
                    var value = System.Convert.ChangeType(propOrFieldData[1], fieldInfo.FieldType);
                    fieldInfo.SetValue(obj, value);
                }

            }
            else
            {

                if (obj is Array)
                {
                    var arr = Array.CreateInstance(elemType, propOrFieldData.Length - 1);
                    for (int i = 1; i < propOrFieldData.Length ; i++)
                    {

                        if (propInfo != null) // Only prop? No such overload of setvalue for field
                        {
                            var value = System.Convert.ChangeType(propOrFieldData[i], elemType);
                            if (propInfo.CanWrite == true)
                                propInfo.SetValue(obj, value, new object[] { i });
                            else
                            {
                                if (obj is Array && elemType != null)
                                {
                                    // var tempobj = System.Convert.ChangeType(obj, elemType.MakeArrayType()); // Што происходит
                                    // tempobj.SetValue(value, i);
                                    // return (object)tempobj;

                                    arr.SetValue(value, i - 1);



                                }
                                else return obj;
                                obj = (object)arr;
                            }
                        }
                    }
                }
            }
            return obj;
        }
        
        //TO DO: recursion for complex fields/properties
        static private void DataToString(ref string s, object obj) 
        {
            var props = obj.GetType().GetProperties().ToArray();
            var fields = obj.GetType().GetFields().ToArray();
            foreach (FieldInfo field in fields)
            {
                s += $"[{field.Name}:{field.GetValue(obj)}]";
            }
     
            foreach (PropertyInfo prop in props)
            {
                Array arr;
                arr = prop.GetValue(obj) as Array;
                if (arr != null)
                {
                    s += "[" + prop.Name;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        s += ":" + arr.GetValue(i);
                    }
                    s += "]";
                    continue;
                }
                var parameters = prop.GetIndexParameters();
                if (parameters.Length == 0)
                {
                    s += $"[{prop.Name}:{prop.GetValue(obj)}]";
                }
                else
                {
                   
                    s += "[" + prop.Name;
                    DataToString(ref s, prop.GetValue(obj,new object[]{ 0 }));
                    s += "]";
                }
            }
        } // can be merged with bin serialization?
        static public void BinSerialize(object obj, string filename)
        {
            //string s = "<";
            string s = "";
            DataToString(ref s, obj);
            //s += ">";
            File.WriteAllText(filename, s);
        }
        static public object BinDeserialize(Type objType, string filename, Type elemType = null)
        {
            object obj = null;
            try
            {
                if (elemType == null)
                obj = Activator.CreateInstance(objType);
                else
                {
                    obj = Array.CreateInstance(elemType, 100); // Храни костыль
                }
                
            }
            catch (Exception) { return null; }
            if (obj == null) return null;


            string s = File.ReadAllText(filename);
            char[] separators = new char[2] { '[', ']' };
            
            string[] propsAndFields = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach(string stringPropOrField in propsAndFields)
            {
                if (stringPropOrField == "")
                    continue;
                string[] propOrFieldData = stringPropOrField.Split(":");
                obj = Parse(obj, propOrFieldData, elemType);
            }
            
            return obj;
        }

        static public void JSONSerialize(object obj, string filename)
        {
            static string fieldToJSON (FieldInfo info, object obj)
            {

                if (info.FieldType == typeof(string))
                    return new string($"\"{info.Name}\":\"{info.GetValue(obj)}\"");
                else
                    return new string($"\"{info.Name}\":{info.GetValue(obj)}");
            }
            static string propToJSON(PropertyInfo info, object obj)
            {
                Array arr;
                arr = info.GetValue(obj) as Array;
                if (arr != null)
                {
                    string s = "";
                    s += $"\"{info.Name}\": [\n";
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (info.PropertyType == typeof(string))
                            s += $"\"{arr.GetValue(i)},\"\n";
                        else
                            s += $"{arr.GetValue(i)},\n";
                        //s +=  arr.GetValue(i);
                    }
                    s += "]";
                    return s;
                }
                else
                if (info.PropertyType == typeof(string))
                    return new string($"\"{info.Name}\":\"{info.GetValue(obj)}\"");
                else
                    return new string($"\"{info.Name}\":{info.GetValue(obj)}");
            }


            var props = obj.GetType().GetProperties().ToArray();
            var fields = obj.GetType().GetFields().ToArray();

            string s = "{\n";

            foreach (FieldInfo field in fields)
            {
                s+= fieldToJSON(field, obj) + "\n";
            }
            foreach (PropertyInfo prop in props)
            {
                s+= propToJSON(prop, obj) + "\n";
            }

            s += "}";

            File.WriteAllText(filename, s);
        }
        static public object JSONDeserialize(Type objType, string filename, Type elemType = null)
        {
            object obj = null;
            try
            {
                if (elemType == null)
                    obj = Activator.CreateInstance(objType);
                else
                {
                    obj = Array.CreateInstance(elemType, 100); // Храни костыль
                }

            }
            catch (Exception) { return null; }
            if (obj == null) return null;


            string s = File.ReadAllText(filename);

            string[] propsAndFields = s.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (string stringPropOrField in propsAndFields)
            {
                
                if (stringPropOrField == "")
                    continue;
                var propOrFieldData = stringPropOrField.Split(':');
                if (propOrFieldData.Length < 2)
                    continue;
                //propOrFieldData[0].Trim('\"'); Почему, шарп, почему ты не работаеш
                //propOrFieldData[1].Trim('\"');
                char[] separators = new char[] { '[', ']', '\n' }; // Не работает с массивом, но у меня нет сыл это делать, то что ниже какой-то шок забейте - я в тильте
                var propOrFieldDataArr = propOrFieldData[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                var list = new List<string>();
                list.Add(propOrFieldData[0]);
                var toParse = list.Concat(propOrFieldDataArr).ToArray();
                for (int i = 0; i < toParse.Length; i++)
                {
                    Trim(ref toParse[i]);
                }


                obj = Parse(obj, toParse, elemType);
            }

            return obj;
        }

        /*static public void SOAPSerialize(object obj, string filename)
        { 
            // Притворитесь что тут что-то есть
            SoapFormatter soapFormatter = new SoapFormatter();
            using (Stream fStream = new FileStream(filename,
            FileMode.Create, FileAccess.Write, FileShare.None))
            {
                soapFormatter.Serialize(fStream, objGraph);
            }
        }*/

        static public void XMLSerialize(object obj, string filename)
        { 
            // Я не буду тратить СТОЛЬКО времени
            XmlSerializer xSer = new XmlSerializer(obj.GetType());
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                xSer.Serialize(fs, obj);
            }
        }
        static public object XMLDeserialize(Type objectType, string filename)
        {
            XmlSerializer xSer = new XmlSerializer(objectType);
            // Я не буду тратить СТОЛЬКО времени
            object obj = null;
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
               obj = xSer.Deserialize(fs);
            }
            return obj;
        }


    }
}
