﻿using System;
using System.Collections.Generic;
using System.Text;

using Carbon.Json;
using Carbon.Data;

namespace Amazon.DynamoDb
{
    public class UpdateExpression
    {
        private readonly JsonObject attributeNames;
        private readonly AttributeCollection attributeValues;

        private readonly StringBuilder set = null;
        private readonly StringBuilder remove = null;
        private readonly StringBuilder add = null;
        private readonly StringBuilder delete = null;

        public UpdateExpression(
            IEnumerable<Change> changes,
            JsonObject attributeNames,
            AttributeCollection attributeValues)
        {
            this.attributeNames = attributeNames;
            this.attributeValues = attributeValues;

            foreach (var change in changes)
            {
                /*
                An update expression consists of sections. 
                Each section begins with a SET, REMOVE, ADD or DELETE keyword. 
                You can include any of these sections in an update expression in any order. 
                However, each section keyword can appear only once. 
                */

                if (change.Operation == DataOperation.Remove)
                {
                    // REMOVE (attributes)
                    // e.g. REMOVE Title, RelatedItems[2], Pictures.RearView
                    // DELETE (elements in set)
                    // e.g. Color :c (deleted :c from color set)

                    if (change.Value == null)
                    {
                        // Remove attribute

                        if (remove == null) remove = new StringBuilder("REMOVE ");

                        WriteChange(change, remove);

                    }
                    else
                    {
                        // Delete element
                        if (delete == null) delete = new StringBuilder("DELETE ");

                        WriteChange(change, delete);
                    }

                }
                else if (change.Operation == DataOperation.Add)
                {
                    if (add == null) add = new StringBuilder("ADD ");

                    WriteChange(change, add);

                }
                else if (change.Operation == DataOperation.Replace)
                {
                    if (set == null) set = new StringBuilder("SET ");

                    WriteChange(change, set);
                }
                else
                {
                    throw new Exception("Unexpected change operation: " + change.Operation);
                }
            }
        }


        public void WriteChange(Change change, StringBuilder sb)
        {
            if (sb == null) sb = new StringBuilder();

            if (sb[sb.Length - 1] != ' ')
            {
                sb.Append(", ");
            }

            WriteName(change.Name, sb);

            if (change.Value != null)
            {
                if (change.Operation == DataOperation.Replace)
                {
                    sb.Append(" = ");
                }
                else
                {
                    sb.Append(" ");
                }

                WriteValue(change.Value, sb);
            }
        }

        private void WriteValue(object value, StringBuilder sb)
            => sb.WriteValue(value, attributeValues);

        private void WriteName(string name, StringBuilder sb)
            => sb.WriteName(name, attributeNames);

        public override string ToString()
        {
            /*
             SET set-action , ... 
           | REMOVE remove-action , ...  
           | ADD add-action , ... 
           | DELETE delete-action , ...  
           */

            var sb = new StringBuilder();

            if (set != null)    sb.AppendLine(set.ToString());
            if (remove != null) sb.AppendLine(remove.ToString());
            if (add != null)    sb.AppendLine(add.ToString());
            if (delete != null) sb.AppendLine(delete.ToString());

            return sb.Remove(sb.Length - 2, 2).ToString();
        }
    }

}
