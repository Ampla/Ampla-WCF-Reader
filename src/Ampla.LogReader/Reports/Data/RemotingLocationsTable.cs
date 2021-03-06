﻿using System;
using System.Collections.Generic;
using System.Data;
using Ampla.LogReader.Remoting;

namespace Ampla.LogReader.Reports.Data
{
    public sealed class RemotingLocationsTable
    {
        public RemotingLocationsTable(IEnumerable<RemotingEntry> entries)
        {
            DataTable data = CreateTable("Locations");
            AddEntries(data, entries);
            Data = data;
        }

        private static DataTable CreateTable(string name)
        {
            DataTable table = new DataTable(name);
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("CallTimeUtc", typeof(DateTime));
            table.Columns.Add("CallTimeLocal", typeof(DateTime));
            table.Columns.Add("Identity", typeof(string));
            table.Columns.Add("Session", typeof(string));
            table.Columns.Add("Module", typeof(string));
            table.Columns.Add("Location", typeof(string));
            table.Columns.Add("Operation", typeof(string));
            table.Columns.Add("MetaData", typeof(string));
            table.Columns.Add("LocalDate", typeof(DateTime));
            table.Columns.Add("LocalHour", typeof(string));
            table.Columns.Add("DayOfWeek", typeof(string));
            return table;
        }

        public DataTable Data { get; private set; }

        private static void AddEntries(DataTable dataTable, IEnumerable<RemotingEntry> entries)
        {
            int count = 0;

            foreach (RemotingEntry entry in entries)
            {
                ILocationParameter location = null;
                switch (entry.Method)
                {
                    case "Query":
                        {
                            location = new QueryParameters(entry);
                            break;
                        }
                    case "Update":
                        {
                            location = new UpdateParameters(entry);
                            break;
                        }
                    case "GetNewSample":
                        {
                            location = new GetNewSampleParameters(entry);
                            break;
                        }
                }

                if (location != null)
                {
                    dataTable.Rows.Add(
                        ++count,
                        entry.CallTimeUtc,
                        entry.CallTimeLocal,
                        entry.Identity,
                        entry.Arguments[0].Value,
                        location.Module,
                        location.Location,
                        location.Operation,
                        location.MetaData,
                        entry.CallTimeLocal.Date,
                        TimeSpan.FromHours(entry.CallTimeLocal.Hour),
                        entry.CallTimeLocal.DayOfWeek
                        );
                }
            }

            dataTable.AcceptChanges();
        }
    }
}