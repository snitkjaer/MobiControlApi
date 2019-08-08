using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;

namespace MobiControlApi
{
    public partial class Api : LogAbstraction
    {
        // Get all custom attribute for this device
        public async Task<string> GetCustomAttributesJsonAsync(string deviceId)
        {
            // GET /devices/{deviceId}/customAttributes

            List<string> deviceIds = new List<string>();
            deviceIds.Add(deviceId);

            // Generate resourcePath
            string resourcePath = "devices/" + deviceId + "/customAttributes";

            // Call GetJsonAsync
            return await GetJsonAsync(resourcePath);
        }

        // Get all custom attribute for this device
        public async Task<CustomAttributes> GetCustomAttributesAsync(string deviceId)
        {
            string jsonCustomAttr = await GetCustomAttributesJsonAsync(deviceId);

            CustomAttributes customAttributes = new CustomAttributes(jsonCustomAttr);

            return customAttributes;

        }

        // Get all custom attribute for this device
        public async Task<string> GetCustomAttributeValueAsync(string deviceId, string attributeName)
        {

            CustomAttributes customAttributes = await GetCustomAttributesAsync(deviceId);
            return customAttributes.GetAttributeValue(attributeName);

        }



            // Set multiple custom attributes
            /*
                {
                    "Attributes": [
                    {
                        "AttributeName": "RkCertStatus",
                        "AttributeValue": "hejhh"
                    }
                    ]
                }
             */
            private async Task<bool> SetCustomAttributeAsync(string deviceId, JObject jsonCustomAttributes)
        {
            // PUT /devices/{deviceId}/customAttributes

            // Generate resourcePath
            string resourcePath = "devices/" + deviceId + "/customAttributes";
            string CustomAttributeBodyJson = jsonCustomAttributes.ToString();
            // Call GetJsonAsync
            return await PutJsonAsync(resourcePath, CustomAttributeBodyJson);
        }

        // Set specific custom attribute for this device
        public async Task<bool> SetCustomAttributeAsync(string deviceId, CustomAttribute customAttribute)
        {

            CustomAttributes customAttributes = new CustomAttributes();
            customAttributes.listCustomAttributes.Add(customAttribute);

            return await SetCustomAttributeAsync(deviceId, customAttributes.GetJson());
        }

        // Set specific custom attribute for this device
        public async Task<bool> SetCustomAttributeAsync(string deviceId, string CustomAttributeName, string CustomAttributeValue)
        {
            CustomAttribute customAttribute = new CustomAttribute(CustomAttributeName, CustomAttributeValue);

            return await SetCustomAttributeAsync(deviceId, customAttribute);
        }
    }

    /*
        [
          {
            "$type": "CustomAttributeInfo",
            "Name": "RkCertStatus",
            "OriginName": "",
            "IsInherited": false,
            "Value": "hejhh",
            "DataType": "Text"
          },
          {
            "$type": "CustomAttributeInfo",
            "Name": "PhoneNumber",
            "OriginName": "",
            "IsInherited": false,
            "Value": "26879816",
            "DataType": "Text"
          }
        ]
     */

    public class CustomAttribute
    {
        public string Name;
        public string Value;

        public CustomAttribute()
        { }

        public CustomAttribute(string Name, String Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

    }

    /*
        {
            "AttributeName": "Name",
            "AttributeValue": "Value"
        }
    */
    public class CustomAttributeUpload
    {
        public string AttributeName;
        public string AttributeValue;

        public CustomAttributeUpload(CustomAttribute customAttribute)
        {
            AttributeName = customAttribute.Name;
            AttributeValue = customAttribute.Value;
        }
    }




    public class CustomAttributes
    {
        public List<CustomAttribute> listCustomAttributes;
        private List<CustomAttributeUpload> listCustomAttributesUpload
        {
            get
            {
                List<CustomAttributeUpload> rv = new List<CustomAttributeUpload>();

                foreach (CustomAttribute customAttribute in listCustomAttributes)
                {
                    rv.Add(new CustomAttributeUpload(customAttribute));
                }

                return rv;

            }
        }


        public CustomAttributes()
        {
            listCustomAttributes = new List<CustomAttribute>();
        }

        public CustomAttributes(string jsonCustomAttr)
        {
            listCustomAttributes = JsonConvert.DeserializeObject<List<CustomAttribute>>(jsonCustomAttr);
        }


        public string GetAttributeValue(string CustomAttributeName)
        {
            if (listCustomAttributes.Exists(attr => attr.Name == CustomAttributeName))
                return listCustomAttributes.Find(attr => attr.Name == CustomAttributeName).Value;
            else
                return null;
        }


        /*
            {
                "Attributes": [
                {
                    "AttributeName": "RkCertStatus",
                    "AttributeValue": "hejhh"
                }
                ]
            }
         */
        public JObject GetJson()
        {

            JObject obj = new JObject(
                new JProperty("Attributes", JArray.FromObject(listCustomAttributesUpload))
            );


            return obj;

        }

    }

    public class CustomAttributeBody
    {
        public string customAttributeValue;

        public CustomAttributeBody(string customAttributeValue)
        {
            this.customAttributeValue = customAttributeValue;
        }

        public string ToJsonString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

}

