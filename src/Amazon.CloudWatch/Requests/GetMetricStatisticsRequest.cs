﻿using System;
using System.Collections.Generic;

namespace Amazon.CloudWatch
{
    public class GetMetricStatisticsRequest
    {
        public GetMetricStatisticsRequest(string nameSpace, string metricName)
        {
            #region Preconditions

            if (nameSpace == null)
                throw new ArgumentNullException(nameof(nameSpace));

            if (metricName == null)
                throw new ArgumentNullException(nameof(metricName));

            #endregion

            Namespace = nameSpace;
            MetricName = metricName;
        }
        
        // Required
        public string MetricName { get; }

        // Required
        public string Namespace { get; }

        public IList<Dimension> Dimensions { get; set; }

        public IList<Statistic> Statistics { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Unit { get; set; }

        /// <summary>
        /// The granularity, in seconds, of the returned data points. 
        /// A period can be as short as one minute (60 seconds) and must be a multiple of 60.
        /// The default value is 60.
        /// </summary>
        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(60);

        public AwsRequest ToParams()
        {
            var parameters = new AwsRequest {
                { "Action"     , "GetMetricStatistics" },

                // Required paramaeters
                { "Namespace"  , Namespace },
                { "MetricName" , MetricName },
                { "StartTime"  , StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ") },
                { "EndTime"    , EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ") },
                { "Period"     , (int)Period.TotalSeconds }
            }; 
        
            if (Unit != null)
            {
                parameters.Add("Unit", Unit);
            }

            if (Dimensions != null)
            {
                for (int i = 0; i < Dimensions.Count; i++)
                {
                    var dimension = Dimensions[i];

                    var prefix = "Dimensions.member." + (i + 1) + ".";

                    parameters.Add(prefix + "Name", dimension.Name);
                    parameters.Add(prefix + "Value", dimension.Value);
                }
            }

            if (Statistics != null)
            {
                for (int i = 0; i < Statistics.Count; i++)
                {
                    var stat = Statistics[i];

                    var prefix = "Statistics.member." + (i + 1);

                    parameters.Add(prefix, stat.Name);
                }
            }

            return parameters;
        }
    }
}