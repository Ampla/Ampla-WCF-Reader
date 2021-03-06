﻿using System;
using System.Globalization;
using System.Xml;
using Ampla.LogReader.Xml;
using NUnit.Framework;

namespace Ampla.LogReader.Wcf
{
    [TestFixture]
    public class WcfCallUnitTests : TestFixture
    {
        private XmlNode xmlNode;
        private const string fileName = @".\Wcf\Resources\SingleEntry.log";
        private const string exceptionFileName = @".\Wcf\Resources\ExceptionEntry.log";
        private const string innerExceptionFileName = @".\Wcf\Resources\InnerExceptionEntry.log";
        private const string businessErrorFileName = @".\Wcf\Resources\BusinessErrorEntry.log";
        private const string tcpFaultFileName = @".\Wcf\Resources\TcpFaultEntry.log";

        protected override void OnSetUp()
        {
            XmlFragmentTextReader reader = new XmlFragmentTextReader("Xml", fileName);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);

            xmlNode = xmlDoc.SelectSingleNode("/Xml/WCFCall");

            Assert.That(xmlNode, Is.Not.Null);
            base.OnSetUp();
        }

        [Test]
        public void LoadCallTime()
        {
            WcfCall call = WcfCall.LoadFromXml(xmlNode);

            DateTime callTime = XmlHelper.GetDateTimeUtc(xmlNode, "CallTime", DateTime.MinValue);
            DateTime expected = DateTime.Parse("2013-04-29T02:45:06.4647667Z", null,
                                               DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

            Assert.That(callTime, Is.GreaterThan(DateTime.MinValue));
            Assert.That(callTime.Kind, Is.EqualTo(DateTimeKind.Utc), "CallTime: {0}", callTime);
            Assert.That(call.CallTime, Is.EqualTo(expected));
            Assert.That(callTime, Is.EqualTo(expected));
        }

        [Test]
        public void LoadUrl()
        {
            WcfCall call = WcfCall.LoadFromXml(xmlNode);

            string url = XmlHelper.GetValue(xmlNode, "Url", string.Empty);

            Assert.That(url, Is.Not.Empty);
            Assert.That(url, Is.StringContaining("localhost"));
            Assert.That(call.Url, Is.EqualTo(url));
        }

        [Test]
        public void LoadAction()
        {
            WcfCall call = WcfCall.LoadFromXml(xmlNode);

            string action = XmlHelper.GetValue(xmlNode, "Action", string.Empty);

            Assert.That(action, Is.Not.Empty);
            Assert.That(action, Is.StringContaining("Ampla"));
            Assert.That(call.Action, Is.EqualTo(action));
        }

        [Test]
        public void LoadMethod()
        {
            WcfCall call = WcfCall.LoadFromXml(xmlNode);

            string method = XmlHelper.GetValue(xmlNode, "Method", "No Method");

            Assert.That(method, Is.Empty);
            Assert.That(call.Method, Is.Not.EqualTo(method));
            Assert.That(call.Action, Is.StringEnding(call.Method));
        }

        [Test]
        public void LoadDuration()
        {
            WcfCall call = WcfCall.LoadFromXml(xmlNode);

            double duration = XmlHelper.GetValue(xmlNode, "Duration", 0D);

            Assert.That(duration, Is.EqualTo(7.0004D));
            Assert.That(call.Duration, Is.EqualTo(TimeSpan.FromMilliseconds(duration)));
        }

        [Test]
        public void LoadResponseMessageLength()
        {
            WcfCall call = WcfCall.LoadFromXml(xmlNode);

            double responseMessageLength = XmlHelper.GetValue(xmlNode, "ResponseMessageLength", 0D);

            Assert.That(responseMessageLength, Is.EqualTo(4.45D));
            Assert.That(call.ResponseMessageLength, Is.EqualTo(responseMessageLength));
        }

        [Test]
        public void LoadIsFault()
        {
            WcfCall call = WcfCall.LoadFromXml(xmlNode);

            bool isFault = XmlHelper.GetValue(xmlNode, "IsFault", true);

            Assert.That(isFault, Is.EqualTo(false));
            Assert.That(call.IsFault, Is.EqualTo(isFault));
        }

        [Test]
        public void LoadFaultMessage()
        {
            WcfCall call = WcfCall.LoadFromXml(xmlNode);

            string faultMessage = XmlHelper.GetInnerXml(xmlNode, "FaultMessage");

            Assert.That(faultMessage, Is.Empty);
            Assert.That(call.Fault, Is.Null);
        }

        [Test]
        public void LoadRequestMessage()
        {
            WcfCall call = WcfCall.LoadFromXml(xmlNode);

            string requestMessage = XmlHelper.GetOuterXml(xmlNode, "RequestMessage");

            Assert.That(requestMessage, Is.Not.Empty);
            Assert.That(requestMessage, Is.StringContaining("http://schemas.xmlsoap.org/soap/envelope/"));
            Assert.That(call.RequestMessage, Is.EqualTo(requestMessage));
        }

        [Test]
        public void Source()
        {
            WcfLogReader reader = new WcfLogReader(fileName);
            reader.ReadAll();

            Assert.That(reader.Entries.Count, Is.EqualTo(1));
            WcfCall call = reader.Entries[0];

            const string source = fileName;

            Assert.That(source, Is.Not.Empty);
            Assert.That(call.Source, Is.EqualTo(source));
        }

        [Test]
        public void FaultObject()
        {
            WcfLogReader reader = new WcfLogReader(fileName);
            reader.ReadAll();

            Assert.That(reader.Entries.Count, Is.EqualTo(1));
            WcfCall call = reader.Entries[0];

            Assert.That(call.IsFault, Is.False);
            Assert.That(call.Fault, Is.Null);
        }

        [Test]
        public void FaultObjectWithException()
        {
            WcfLogReader reader = new WcfLogReader(exceptionFileName);
            reader.ReadAll();

            Assert.That(reader.Entries.Count, Is.EqualTo(1));
            WcfCall call = reader.Entries[0];

            Assert.That(call.IsFault, Is.True);
            Assert.That(call.Fault, Is.Not.Null);
            Assert.That(call.Fault.FaultCode, Is.EqualTo("a:InternalServiceFault"), "FaultCode");
            Assert.That(call.Fault.FaultString, Is.EqualTo("User 'User' is currently disabled."), "FaultString");
            Assert.That(call.Fault.Details, Is.Not.Empty, "StackTrace");
        }

        [Test]
        public void FaultObjectWithBusinessError()
        {
            WcfLogReader reader = new WcfLogReader(businessErrorFileName);
            reader.ReadAll();

            Assert.That(reader.Entries.Count, Is.EqualTo(1));
            WcfCall call = reader.Entries[0];

            Assert.That(call.IsFault, Is.True);
            Assert.That(call.Fault, Is.Not.Null);
            Assert.That(call.Fault.FaultCode, Is.EqualTo("a:Client"), "FaultCode");
            Assert.That(call.Fault.FaultString, Is.EqualTo("A business error has occured. The field '' does not exist."), "FaultString");
            Assert.That(call.Fault.Details, Is.Not.Empty, "StackTrace");
        }

        [Test]
        public void TcpFaultWithError()
        {
            WcfLogReader reader = new WcfLogReader(tcpFaultFileName);
            reader.ReadAll();

            Assert.That(reader.Entries.Count, Is.EqualTo(1));
            WcfCall call = reader.Entries[0];

            Assert.That(call.IsFault, Is.True);
            Assert.That(call.Fault, Is.Not.Null);
            Assert.That(call.Fault.FaultCode, Is.EqualTo("Receivera:InternalServiceFault"), "FaultCode");
            Assert.That(call.Fault.FaultString, Is.EqualTo("Unable to acquire a license for this client connection. DemoPeriodComplete"), "FaultString");
            Assert.That(call.Fault.Details, Is.Not.Empty, "StackTrace");
        }


        [Test]
        public void FaultWithInnerException()
        {
            WcfLogReader reader = new WcfLogReader(innerExceptionFileName);
            reader.ReadAll();

            Assert.That(reader.Entries.Count, Is.EqualTo(1));
            WcfCall call = reader.Entries[0];

            Assert.That(call.IsFault, Is.True);
            Assert.That(call.Fault, Is.Not.Null);
            Assert.That(call.Fault.FaultCode, Is.EqualTo("a:InternalServiceFault"), "FaultCode");
            Assert.That(call.Fault.FaultString, Is.EqualTo("The value 'Value123' cannot be converted to the 'System.Boolean' data type."), "FaultString");
            Assert.That(call.Fault.Details, Is.Not.Empty, "StackTrace");
        }
    }
}