﻿using System;
using System.Collections.Generic;

namespace Amazon.Ec2
{
    public abstract class DescribeRequest
    {
        public int? MaxResults { get; set; }

        public string NextToken { get; set; }

        public List<Filter> Filters { get; } = new List<Filter>();

        protected void AddIds(AwsRequest request, string idName, List<string> ids)
        {
            var i = 1;

            foreach (var id in ids)
            {
                // e.g. VpcId.1

                request.Add(idName + "." + i, id);

                i++;
            }
        }

        protected AwsRequest GetParameters(string actionName)
        {
            #region Preconditions

            if (actionName == null)
                throw new ArgumentNullException(nameof(actionName));

            #endregion

            var parameters = new AwsRequest {
                { "Action", actionName }
            };

            var i = 1;

            foreach (var filter in Filters)
            {
                var prefix = "Filter." + i + ".";

                parameters.Add(prefix + "Name", filter.Name);
                parameters.Add(prefix + "Value", filter.Value);

                i++;
            }

            if (MaxResults != null)
            {
                parameters.Add("MaxResults", MaxResults.Value);
            }

            if (NextToken != null)
            {
                parameters.Add("NextToken", NextToken);
            }

            return parameters;
        }
    }
}
