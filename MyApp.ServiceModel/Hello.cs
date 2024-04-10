using Microsoft.AspNetCore.Http;
using ServiceStack;
using ServiceStack.Text;
using ServiceStack.Text.Jsv;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.Encodings.Web;
namespace MyApp.ServiceModel
{

    public static class AppHostConfig
    {
        static AppHostConfig()
        {
            JsConfig.ExcludeTypeInfo = false;
            JsConfig.AssumeUtc = true;
            JsConfig.DateHandler = DateHandler.ISO8601;
            JsConfig.ThrowOnError = true;

            JsConfig.TypeWriter = (tt) =>
            {
                return string.Format("{0}.{1}", tt.Namespace, tt.Name);
            };

            JsConfig<Dto>.RawDeserializeFn = r =>
            {
                var fields = new Dto();
                JsonObject jobject = jsvJsonParse(r);

                foreach (string key in jobject.Keys)
                {

                    string json = jobject.Child(key);
                    JsonObject jjobject = jsvJsonParse(json);
                    try
                    {
                        json = jjobject.Child("Value");
                    }
                    catch { }
                }
                return fields;
            };

            JsConfig<bool>.DeSerializeFn = r =>
            {
                bool boolVal = false;
                if (Boolean.TryParse(r, out boolVal))
                {
                    return boolVal;
                }
                else
                {
                    return false;
                }

            };
            JsConfig<PropertyOrFieldDef>.DeSerializeFn = ToPropertyOrFieldDef;
        }

        public static void Initialize()
        {
        }

        private static bool isJsonNotJsv(string json)
        {
            foreach (char c in json)
            {
                if (c == '"')
                {
                    return true;
                }

                if (c == ':')
                {
                    return false;
                }
            }
            return false;
        }

        private static JsonObject jsvJsonParse(string json)
        {
            if (isJsonNotJsv(json))
            {
                return JsonObject.Parse(json);
            }
            else if (json.StartsWith(@"{"))
            {
                return JsvReader.GetParseFn(typeof(JsonObject))(json) as JsonObject;
            }
            return null;
        }

        [DataContract]
        [SDKObjectAttribute]

        public partial class PropertyOrFieldDef
        {
            [DataMember]
            public string Id { get; set; }

            [DataMember]
            public int ColumnWidth { get; set; }
        }

        private static string ValueFromeJsonOrJsv(string r, string key = "Value")
        {
            JsonObject jobject = jsvJsonParse(r);
            if (jobject != null)
            {
                return jobject[key];
            }
            return null;
        }

        public static PropertyOrFieldDef ToPropertyOrFieldDef(string r)
        {
            if (r != null)
            {
                if (r.StartsWith("{"))
                {
                    string _columnWidth = ValueFromeJsonOrJsv(r, "ColumnWidth");
                    if (string.IsNullOrWhiteSpace(_columnWidth))
                    {
                        return new PropertyOrFieldDef() { Id = ValueFromeJsonOrJsv(r, "Id") };
                    }
                    else
                    {
                        return new PropertyOrFieldDef() { Id = ValueFromeJsonOrJsv(r, "Id"), ColumnWidth = Int32.Parse(_columnWidth) };
                    }
                }
            }
            return new PropertyOrFieldDef() { Id = r.Trim() };
        }

    }

    [DataContract]
    [Route("/hello")]
    [Route("/hello/{Name}")]
    public class Hello : IReturn<HelloResponse>
    {
        public string Name { get; set; }
    }

    public class Dto
    {
        public Dictionary<string, string> StringDictionary { get; set; }

    }


    [DataContract]
    public class HelloResponse
    {
        public string Result { get; set; }

        [DataMember]
        [ApiMember(ExcludeInSchema = true, DataType = "object", Verb = "NONE")]
        public Dto Fields { get; set; }

    }

    [Route("/requiresauth")]
    [Route("/requiresauth/{Name}")]
    public class RequiresAuth : IReturn<RequiresAuthResponse>
    {
        public string Name { get; set; }
    }

    public class RequiresAuthResponse
    {
        public string Result { get; set; }
    }

    [Route("/requiresrole")]
    [Route("/requiresrole/{Name}")]
    public class RequiresRole : IReturn<RequiresRoleResponse>
    {
        public string Name { get; set; }
    }

    public class RequiresRoleResponse
    {
        public string Result { get; set; }
    }

    [Route("/requiresadmin")]
    [Route("/requiresadmin/{Name}")]
    public class RequiresAdmin : IReturn<RequiresAdminResponse>
    {
        public string Name { get; set; }
    }

    public class RequiresAdminResponse
    {
        public string Result { get; set; }
    }

}