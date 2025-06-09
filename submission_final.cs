using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

/**
 * CSE445 Assignment 4 - XML Processing with Schema Validation and JSON Conversion
 * Author: KooshaPari
 * Features: XML validation, error detection, XML to JSON conversion
 * Compatible with .NET 4.7 and Newtonsoft.Json
 **/

namespace ConsoleApp1
{
    public class Program
    {
        // URLs for remote XML/XSD files - GitHub raw URLs  
        public static string xmlURL = "https://raw.githubusercontent.com/KooshaPari/CSE445-A4/main/Hotels.xml";
        public static string xmlErrorURL = "https://raw.githubusercontent.com/KooshaPari/CSE445-A4/main/HotelsErrors.xml";
        public static string xsdURL = "https://raw.githubusercontent.com/KooshaPari/CSE445-A4/main/Hotels.xsd";

        public static void Main(string[] args)
        {
            // Test valid XML validation
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            // Test invalid XML validation  
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            // Test XML to JSON conversion
            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1 - XML validation against XSD schema
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                // Get XSD content (from URL or local file)
                string xsdContent = GetFileContent(xsdUrl);

                // Create XmlSchema from XSD content
                XmlSchema schema;
                using (StringReader xsdReader = new StringReader(xsdContent))
                {
                    schema = XmlSchema.Read(xsdReader, null);
                }

                // Create XmlReaderSettings with schema validation
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(schema);
                settings.ValidationType = ValidationType.Schema;

                // Collect validation errors
                List<string> validationErrors = new List<string>();
                settings.ValidationEventHandler += (sender, e) =>
                {
                    validationErrors.Add(string.Format("Line {0}: {1}", e.Exception.LineNumber, e.Message));
                };

                // Get and validate XML content
                string xmlContent = GetFileContent(xmlUrl);

                using (StringReader xmlReader = new StringReader(xmlContent))
                using (XmlReader reader = XmlReader.Create(xmlReader, settings))
                {
                    // Read through entire document to trigger validation
                    while (reader.Read()) { }
                }

                // Return results
                if (validationErrors.Count == 0)
                {
                    return "No Error";
                }
                else
                {
                    return string.Join("; ", validationErrors);
                }
            }
            catch (Exception ex)
            {
                return string.Format("Validation error: {0}", ex.Message);
            }
        }

        // Q2.2 - Convert XML to JSON format
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                // Get XML content (from URL or local file)
                string xmlContent = GetFileContent(xmlUrl);

                // Load XML document
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                // Create JSON structure manually to match required format
                var hotelsJson = new
                {
                    Hotels = new
                    {
                        Hotel = CreateHotelArray(xmlDoc)
                    }
                };

                // Convert to JSON string
                string jsonText = JsonConvert.SerializeObject(hotelsJson, Formatting.Indented);
                return jsonText;
            }
            catch (Exception ex)
            {
                return string.Format("XML to JSON conversion error: {0}", ex.Message);
            }
        }

        // Helper method to get file content from URL or local file
        private static string GetFileContent(string path)
        {
            if (path.StartsWith("http://") || path.StartsWith("https://"))
            {
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(path);
                }
            }
            else
            {
                return File.ReadAllText(path);
            }
        }

        // Helper method to create hotel array from XML
        private static object[] CreateHotelArray(XmlDocument xmlDoc)
        {
            XmlNodeList hotelNodes = xmlDoc.SelectNodes("//Hotel");
            List<object> hotels = new List<object>();

            foreach (XmlNode hotelNode in hotelNodes)
            {
                var hotel = new Dictionary<string, object>();

                // Add Name
                XmlNode nameNode = hotelNode.SelectSingleNode("Name");
                if (nameNode != null)
                {
                    hotel["Name"] = nameNode.InnerText;
                }

                // Add Phone numbers as array
                XmlNodeList phoneNodes = hotelNode.SelectNodes("Phone");
                if (phoneNodes.Count > 0)
                {
                    List<string> phones = new List<string>();
                    foreach (XmlNode phoneNode in phoneNodes)
                    {
                        phones.Add(phoneNode.InnerText);
                    }
                    hotel["Phone"] = phones.ToArray();
                }

                // Add Address with nested structure
                XmlNode addressNode = hotelNode.SelectSingleNode("Address");
                if (addressNode != null)
                {
                    var address = new Dictionary<string, string>();

                    // Add address components
                    XmlNode numberNode = addressNode.SelectSingleNode("Number");
                    if (numberNode != null) address["Number"] = numberNode.InnerText;

                    XmlNode streetNode = addressNode.SelectSingleNode("Street");
                    if (streetNode != null) address["Street"] = streetNode.InnerText;

                    XmlNode cityNode = addressNode.SelectSingleNode("City");
                    if (cityNode != null) address["City"] = cityNode.InnerText;

                    XmlNode stateNode = addressNode.SelectSingleNode("State");
                    if (stateNode != null) address["State"] = stateNode.InnerText;

                    XmlNode zipNode = addressNode.SelectSingleNode("Zip");
                    if (zipNode != null) address["Zip"] = zipNode.InnerText;

                    // Add NearestAirport attribute with underscore prefix
                    XmlAttribute airportAttr = addressNode.Attributes["NearestAirport"];
                    if (airportAttr != null) address["_NearestAirport"] = airportAttr.Value;

                    hotel["Address"] = address;
                }

                // Add Rating attribute with underscore prefix (only if present)
                XmlAttribute ratingAttr = hotelNode.Attributes["Rating"];
                if (ratingAttr != null)
                {
                    hotel["_Rating"] = ratingAttr.Value;
                }

                hotels.Add(hotel);
            }

            return hotels.ToArray();
        }
    }
}
