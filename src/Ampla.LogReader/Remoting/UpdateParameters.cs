﻿namespace Ampla.LogReader.Remoting
{
    public class UpdateParameters : ILocationParameter
    {
        public UpdateParameters(RemotingEntry remotingEntry)
        {
            if (remotingEntry.Method == "Update")
            {
                Operation = "Update";
                if (remotingEntry.Arguments.Length == 3)
                {
                    // ViewDescriptor (string)
                    RemotingArgument view = remotingEntry.Arguments[1];
                    if (view.TypeName == "Citect.Ampla.General.Common.ViewDescriptor")
                    {
                        ViewDescriptor viewDescriptor = new ViewDescriptor(view.Value);
                        Module = viewDescriptor.Module;
                    }
                    // EditedDataDescriptorCollection (xml)
                    RemotingArgument editDataDescriptor = remotingEntry.Arguments[2];
                    if (editDataDescriptor.TypeName == "Citect.Ampla.General.Common.EditedDataDescriptorCollection")
                    {
                        EditedDataDescriptorCollection collection = new EditedDataDescriptorCollection(editDataDescriptor.Value);
                        Location = collection.Location;
                        MetaData = collection.FieldValues;
                        Operation = collection.Operation;
                    }
                }
            }
        }

        public string Operation { get; private set; }

        public string Location { get; private set; }

        public string Module { get; private set; }

        public string MetaData { get; private set; }

    }
}