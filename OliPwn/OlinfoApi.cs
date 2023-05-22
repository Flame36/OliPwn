using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Text;
using System.Net.Http.Headers;

namespace OliPwn
{
    public class OlinfoApi
    {
        readonly HttpClient client = new();
        static readonly SemaphoreSlim semaphore = new(1);
        [ThreadStatic]
        static DateTime nextSubmissionMin;

        public OlinfoApi() { nextSubmissionMin = new(0); }

        static async Task<bool> GetSuccess(HttpResponseMessage res)
        {
            JToken? success = JObject.Parse(await res.Content.ReadAsStringAsync())["success"];
            return success is null ? throw new UnreachableException("Success not in response") : (bool)success;
        }

        public async Task Login(string username, string password)
        {
            JObject payloadJson = new()
            {
                ["action"] = "login",
                ["username"] = username,
                ["password"] = password,
                ["keep_signed"] = true
            };
            StringContent payload = new(payloadJson.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage res = await client.PostAsync("https://training.olinfo.it/api/user", payload);
            
            res.EnsureSuccessStatusCode();
            if (!(await GetSuccess(res)))
                throw new ArgumentException("Incorrect username or password");
        }

        public async Task<OlinfoTask> GetTaskDataAsync(string taskName)
        {
            JObject payloadJson = new()
            {
                ["name"] = taskName,
                ["action"] = "get"
            };
            StringContent payload = new(payloadJson.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage res = await client.PostAsync("https://training.olinfo.it/api/task", payload);

            res.EnsureSuccessStatusCode();
            if (!(await GetSuccess(res)))
                throw new Exception("Something went wrong");

            JObject json = JObject.Parse(await res.Content.ReadAsStringAsync());

            return new OlinfoTask()
            {
                Name = (string)json["name"],
                TimeLimit = (double)json["time_limit"],
                MemoryLimit = (double)json["memory_limit"],
                FileName = (string)json["submission_format"][0]
            };
        }

        public async Task<int> SubmitFileAsync(OlinfoTask task, string file)
        {
            await semaphore.WaitAsync();
            try
            {
                TimeSpan toWait = nextSubmissionMin - DateTime.UtcNow;
                if (toWait > TimeSpan.Zero)
                    await Task.Delay(toWait);
                JObject payloadJson = new()
                {
                    ["files"] = new JObject()
                    {
                        [task.FileName] = new JObject()
                        {
                            ["filename"] = "ace.cpp",
                            ["language"] = "C++17 / g++",
                            ["data"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(file))
                        }
                    },
                    ["action"] = "new",
                    ["task_name"] = task.Name
                };
                StringContent payload = new(payloadJson.ToString(Formatting.None), Encoding.UTF8, "application/json");

                HttpResponseMessage res = await client.PostAsync("https://training.olinfo.it/api/submission", payload);
                nextSubmissionMin = DateTime.UtcNow + TimeSpan.FromSeconds(20);

                res.EnsureSuccessStatusCode();
                if (!(await GetSuccess(res)))
                    throw new Exception("Something went wrong");

                JObject json = JObject.Parse(await res.Content.ReadAsStringAsync());
                return (int)json["id"];
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<List<SubmissionResult>> GetSubmissionResultAsync(int submissionId)
        {
            JObject payloadJson = new()
            {
                ["action"] = "details",
                ["id"] = submissionId
            };
            StringContent payload = new(payloadJson.ToString(Formatting.None), Encoding.UTF8, "application/json");

            HttpResponseMessage res = await client.PostAsync("https://training.olinfo.it/api/submission", payload);

            res.EnsureSuccessStatusCode();
            if (!(await GetSuccess(res)))
                throw new Exception("Something went wrong");

            List<SubmissionResult> results = new List<SubmissionResult>();
            JObject json = JObject.Parse(await res.Content.ReadAsStringAsync());

            foreach (JToken detail in json["score_details"])
            {
                foreach (JToken testcase in detail["testcases"])
                {
                    results.Add(
                        new SubmissionResult()
                        {
                            Outcome = (string)testcase["outcome"],
                            Text = (string)testcase["text"],
                            Time = (double)testcase["time"],
                            Memory = (double)testcase["memory"]
                        });
                }
            }

            return results;
        }
    }
}
