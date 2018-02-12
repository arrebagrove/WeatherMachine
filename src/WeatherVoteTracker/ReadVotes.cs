using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace WeatherVoteTracker
{
    public static class ReadVotes
    {


        public class SchoolInfo
        {
            public string County
            {
                get;
                set;
            }

            public string School
            {
                get;
                set;
            }

        }

        public class VoteResult : TableEntity
        {
            public string County
            {
                get;
                set;
            }

            public DateTimeOffset EventTime
            {
                get;
                set;
            }

            public string School
            {
                get;
                set;
            }

            public int Votes
            {
                get;
                set;
            }

        }


        public static List<SchoolInfo> SchoolList = new List<SchoolInfo>()
        {
                new SchoolInfo()
                {
                    County = "Chautauqua",
                    School = "Southwestern"
                },
                new SchoolInfo()
                {
                    County = "Niagara",
                    School = "Meadow"
                },
                new SchoolInfo()
                {
                    County = "Erie",
                    School = "Glenwood"
                },
                new SchoolInfo()
                {
                    County = "Erie",
                    School = "St. Amelia"
                }
        };



        [FunctionName("ReadVotes")]
        public static async Task Run(
            [TimerTrigger("0 59 * * * *")]TimerInfo myTimer,
            [Table("WeatherMachineVoteResults", Connection = "WeatherMachineStorageConnection")] ICollector<ReadVotes.VoteResult> results,
            TraceWriter log)
        {
            var now = DateTimeOffset.Now;
            log.Info("ReadVotes starting at " + now.ToString());


            foreach (var item in SchoolList)
            {
                var votes = await GetVotes(item);

                log.Info($"{item.County}\\{item.School}: {votes} votes");

                results.Add(new VoteResult() {
                    PartitionKey = now.ToString("yyyyMMdd"),
                    RowKey = now.ToString("yyyyMMdd-HHmmss-" + item.County + "-" + item.School),
                    EventTime = now,
                    County = item.County,
                    School = item.School,
                    Votes = votes
                });
            }
        }


        public static async Task<int> GetVotes(SchoolInfo school)
        {
            var sUrl = $"https://weathermachine.com/Confirmation.aspx?county={school.County}&school={school.School}";

            WebClient client = new WebClient();

            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");

            Stream data = client.OpenRead(sUrl);
            StreamReader reader = new StreamReader(data);
            string s = await reader.ReadToEndAsync();

            data.Close();
            reader.Close();


            var rx = new Regex(@"<span id=""lblVotes"">(\d+)\s+votes</span>", RegexOptions.IgnoreCase);
            var match = rx.Match(s);

            if (match.Success)
            {
                return Int32.Parse(match.Groups[1].Value);
            }

            // Invalid reading
            return -1;
        }

    }
}
