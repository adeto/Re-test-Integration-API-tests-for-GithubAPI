namespace Re_test_Integration_API_tests_for_GithubAPI
{
    internal class IssueResponse
    {
        public long id { get; set; }
        public long number { get; set; }
        public string body { get; set; }

        public string title { get; set; }
        public string state { get; set; }
    }
}