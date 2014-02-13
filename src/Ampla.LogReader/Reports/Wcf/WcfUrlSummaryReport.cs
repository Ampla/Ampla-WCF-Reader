﻿using System.Collections.Generic;
using Ampla.LogReader.ReportWriters;
using Ampla.LogReader.Statistics;
using Ampla.LogReader.Wcf;

namespace Ampla.LogReader.Reports.Wcf
{
    public class WcfUrlSummaryReport : Report<WcfCall>
    {
        public WcfUrlSummaryReport(List<WcfCall> entries, IReportWriter writer)
            : base(entries, writer)
        {
        }

        protected override void RenderReport(IReportWriter reportWriter)
        {
            GroupByAnalysis<WcfCall, WcfSummaryStatistic, string> analysis = new GroupByAnalysis<WcfCall, WcfSummaryStatistic, string>
                {
                    WhereFunc = entry => true,
                    GroupByFunc = entry => entry.Url,
                    //entry.Method, //entry => entry.CallTime.ToLocalTime().ToShortDateString(),
                    StatisticFactory = key => new WcfSummaryStatistic(key)
                };

            foreach (var wcfCall in Entries)
            {
                analysis.Add(wcfCall);
            }

            using (reportWriter.StartReport("Wcf Url Summary"))
            {
                List<WcfSummaryStatistic> summaries = analysis.Sort(WcfSummaryStatistic.CompareByCountDesc());
                if (summaries.Count > 0)
                {
                    reportWriter.Write("Url");
                    foreach (Result result in summaries[0].Results)
                    {
                        reportWriter.Write(result.Topic);
                    }

                    foreach (WcfSummaryStatistic summary in summaries)
                    {
                        using (reportWriter.StartSection(summary.Name))
                        {
                            foreach (Result result in summary.Results)
                            {
                                reportWriter.Write(result);
                            }
                        }
                    }
                }
            }
        }
    }
}