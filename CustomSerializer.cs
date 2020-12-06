using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using System.IO;

namespace Lab14
{
   
    static class CustomSerializer 
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

        static private object Parse(object obj, string[]propOrFieldData) // propOrFieldData[0] has to be Type of property or field, [1] - value, if indexed - [1],[2] - indexed values
        {
            Type objType = obj.GetType();
            if (propOrFieldData.Length == 2)

            {

                PropertyInfo propInfo = objType.GetProperty(propOrFieldData[0]);
                FieldInfo fieldInfo = objType.GetField(propOrFieldData[0]);

                if (propInfo != null)
                {
                    var value = System.Convert.ChangeType(propOrFieldData[1], propInfo.PropertyType);
                    propInfo.SetValue(obj, value);
                }
                else
                {
                    var value = System.Convert.ChangeType(propOrFieldData[1], fieldInfo.FieldType);
                    fieldInfo.SetValue(obj, value);
                }

            }
            return obj;
        }
        
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
        static public object BinDeserialize(Type objType, string filename)
        {
            object obj = null;
            try
            {
                obj = Activator.CreateInstance(objType);
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
                obj = Parse(obj, propOrFieldData);
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
        static public object JSONDeserialize(Type objType, string filename)
        {
            object obj = null;
            try
            {
                obj = Activator.CreateInstance(objType);
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
                Trim(ref propOrFieldData[0]);
                Trim(ref propOrFieldData[1]);
               

                
                obj = Parse(obj, propOrFieldData);
            }

            return obj;
        }
    
    }
}
