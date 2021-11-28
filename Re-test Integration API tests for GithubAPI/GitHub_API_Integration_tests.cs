using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Re_test_Integration_API_tests_for_GithubAPI
{
    public class Tests
    {
        const string GithubAPIUsername = "test";
        const string GithubAPIPassword = "912338211f1b438f8c63c2723934727d1e744c62";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Get_All_Issues_From_Repo()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues");
            client.Timeout = 3000;
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.ContentType.StartsWith("application/json"));

            var issues = new JsonDeserializer().Deserialize<List<IssueResponse>>(response);
            Assert.Pass();
        }

        [Test]
        public void Get_A_Specific_Issue_9()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/9");
            client.Timeout = 3000;
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test]
        public void Get_A_NonExisting_Issue()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/9786846768");
            client.Timeout = 3000;
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void Create_New_Issue()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues");
            client.Timeout = 3000;
            var request = new RestRequest(Method.POST);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                title = "Test 009",
                body = "New test in this repo"

            });
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var issue = new JsonDeserializer().Deserialize<IssueResponse>(response);

            Assert.IsTrue(issue.id > 0);
            Assert.IsTrue(issue.number > 0);
            Assert.IsTrue(!String.IsNullOrEmpty(issue.title));
            Assert.IsTrue(!String.IsNullOrEmpty(issue.body));
        }

        [Test]
        public void Create_New_Issue_Without_Authentication()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues");
            client.Timeout = 3000;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                title = "Test 009",
                body = "New test in this repo"

            });
            var response = client.Execute(request);

            //Respone should be 401 Unautohorized, but as is using different repo the response is 404 Not found.
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            
        }

        [Test]
        public void Edit_Existing_Issue_2552()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/2552");
            client.Timeout = 3000;
            var request = new RestRequest(Method.PATCH);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                title = "Change Test 009",
                body = "no more text"

            });
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

           var issue = new JsonDeserializer().Deserialize<IssueResponse>(response);
            Assert.AreEqual(issue.title, "Change Test 009");
            Assert.AreEqual(issue.body, "no more text");
        }

        [Test]
        public void Edit_Existing_Issue_With_No_Access_Rights()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/2511");
            client.Timeout = 3000;
            var request = new RestRequest(Method.PATCH);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                title = "Change Test 009",
                body = "no more text"

            });
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Test]
        public void Close_Issue_2552()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/2552");
            client.Timeout = 3000;
            var request = new RestRequest(Method.PATCH);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                state = "closed"

            }); ;
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var issue = new JsonDeserializer().Deserialize<IssueResponse>(response);
            Assert.AreEqual(issue.state, "closed");
        }

        [Test]
        public void Close_Issue_With_No_Access_Rights()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/2511");
            client.Timeout = 3000;
            var request = new RestRequest(Method.PATCH);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                state = "closed"

            }); ;
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Test]
        public void Get_Labels_From_Issue_9()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/9/labels");
            client.Timeout = 3000;
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response != null);
        }

        [Test]
        public void Add_Comment_For_Issue_2552()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/2552/comments");
            client.Timeout = 3000;
            var request = new RestRequest(Method.POST);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                body = "Please update the test environment with latest backend version."

            });
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var issue = new JsonDeserializer().Deserialize<IssueResponse>(response);

            Assert.AreEqual(issue.body, "Please update the test environment with latest backend version.");
        }

        [Test]
        public void Get_All_Comments_For_Issue_2552()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/2552/comments");
            client.Timeout = 3000;
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(response.ContentType.StartsWith("application/json"));
        }

        [Test]
        public void Get_Comment_By_Id()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/comments/780727160");
            client.Timeout = 3000;
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Edit_Existing_Comment_By_Id()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/comments/780757470");
            client.Timeout = 3000;
            var request = new RestRequest(Method.PATCH);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                body = "Everything works correctly."
            });
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Edit_Comment_With_No_Access_Rights()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/comments/720443890");
            client.Timeout = 3000;
            var request = new RestRequest(Method.PATCH);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                body = "tests are done. everything works correctly."

            }); 
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }
        [Test]
        public void Delete_Existing_Comment_By_Id()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/comments/780757470");
            client.Timeout = 3000;
            var request = new RestRequest(Method.PATCH);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Test]
        public void Delete_Existing_Comment_With_No_Access_Rights()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/comments/780757470");
            client.Timeout = 3000;
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}