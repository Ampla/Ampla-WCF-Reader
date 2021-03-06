﻿using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Ampla.LogReader.EventLogs
{
    [TestFixture]
    public class EventLogReaderUnitTests : TestFixture
    {
        [Test]
        public void ReadApplication()
        {
            ILocalEventLogSystem eventLogSystem = new LocalEventLogSystem();
            EventLog eventLog = eventLogSystem.GetEventLog("Application");
            Assert.That(eventLog, Is.Not.Null);

            EventLogReader reader = new EventLogReader(eventLog);
            Assert.That(reader.Entries, Is.Empty);

            reader.ReadAll();

            Assert.That(reader.Entries, Is.Not.Empty);
        }

        [Test]
        public void ReadEntries()
        {
            ILocalEventLogSystem eventLogSystem = new LocalEventLogSystem();
            EventLog eventLog = eventLogSystem.GetEventLogs().FirstOrDefault(ev => ev.Entries.Count > 0);
            Assert.That(eventLog, Is.Not.Null, "Unable to find an Event Log with something in it.");
            
            EventLogReader reader = new EventLogReader(eventLog);
            Assert.That(reader.Entries, Is.Empty);

            int count = eventLog != null ? eventLog.Entries.Count : -1;
            Assert.That(count, Is.GreaterThan(0), "No entries in the EventLog");

            reader.ReadAll();

            Assert.That(reader.Entries, Is.Not.Empty);
            Assert.That(reader.Entries.Count, Is.GreaterThanOrEqualTo(count).And.LessThanOrEqualTo(count + 2));

            reader.ReadAll();

            Assert.That(reader.Entries, Is.Not.Empty);
            Assert.That(reader.Entries.Count, Is.GreaterThanOrEqualTo(count).And.LessThanOrEqualTo(count + 2));
        }


        [Test]
        public void Read10Entries()
        {
            ILocalEventLogSystem eventLogSystem = new LocalEventLogSystem();
            EventLog eventLog = eventLogSystem.GetEventLogs().FirstOrDefault(ev => ev.Entries.Count > 10);
            Assert.That(eventLog, Is.Not.Null, "Unable to find an Event Log with something in it.");

            EventLogReader reader = new EventLogReader(eventLog, 10);
            Assert.That(reader.Entries, Is.Empty);

            int count = eventLog != null ? eventLog.Entries.Count : -1;
            Assert.That(count, Is.GreaterThan(0), "No entries in the EventLog");

            reader.ReadAll();

            Assert.That(reader.Entries, Is.Not.Empty);
            Assert.That(reader.Entries.Count, Is.EqualTo(10));

            reader.ReadAll();

            Assert.That(reader.Entries, Is.Not.Empty);
            Assert.That(reader.Entries.Count, Is.EqualTo(10));
        }

    }
}