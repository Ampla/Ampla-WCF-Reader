﻿using System.Collections.Generic;
using System.Xml;
using Ampla.LogReader.Xml;

namespace Ampla.LogReader.Remoting
{
    public class EditedDataDescriptorCollection
    {
        private readonly string editedData;

        public EditedDataDescriptorCollection(string editedData)
        {
            this.editedData = editedData;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(editedData);

            Location = XmlHelper.GetText(xmlDoc, "/EditedDataDescriptorCollection/@Location");
            SetId = XmlHelper.GetText(xmlDoc, "/EditedDataDescriptorCollection/@SetId");
            List<string> fields = new List<string>();
            foreach (XmlNode xmlField in XmlHelper.GetNodes(xmlDoc, "/EditedDataDescriptorCollection/EditedDataDescriptor"))
            {
                string field = XmlHelper.GetText(xmlField, "@name");
                string value = XmlHelper.GetText(xmlField, "@editedValue");
                fields.Add(field + "={" + value + "}");
            }
            FieldValues = string.Join(",", fields);
        }

        public string Location { get; private set; }
        public string SetId { get; private set; }
        public string FieldValues { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return editedData;
        }
    }
}