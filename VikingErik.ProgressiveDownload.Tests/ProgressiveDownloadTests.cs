using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.Http;

namespace VikingErik.Net.Http.Tests
{
    [TestClass]
    public class ProgressiveDownloadTests
    {
        [TestMethod]
        public void TestProgressiveDownloadWithoutRangeRequest()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Range = null;

            var ms = new MemoryStream(_DummyData);
            ms.Seek(0, SeekOrigin.Begin);

            var vidStreamer = new ProgressiveDownload(request);
            using (var result = vidStreamer.ResultMessage(ms, "application/octet-stream"))
            {
                Assert.AreEqual(System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/octet-stream"), result.Content.Headers.ContentType);
                Assert.AreEqual(30, result.Content.Headers.ContentLength);
                Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            }
        }

        [TestMethod]
        public void TestProgressiveDownloadWithRangeRequest()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(0, 5);

            var ms = new MemoryStream(_DummyData);
            ms.Seek(0, SeekOrigin.Begin);

            var vidStreamer = new ProgressiveDownload(request);
            using (var result = vidStreamer.ResultMessage(ms, "application/octet-stream"))
            {
                Assert.AreEqual(System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/octet-stream"), result.Content.Headers.ContentType);
                Assert.AreEqual(6, result.Content.Headers.ContentLength);
                Assert.AreEqual(System.Net.HttpStatusCode.PartialContent, result.StatusCode);
            }
        }

        [TestMethod]
        public void TestProgressiveDownloadWithOpenRangeRequest()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(5, null);

            var ms = new MemoryStream(_DummyData);
            ms.Seek(0, SeekOrigin.Begin);

            var vidStreamer = new ProgressiveDownload(request);
            using (var result = vidStreamer.ResultMessage(ms, "application/octet-stream"))
            {
                Assert.AreEqual(System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/octet-stream"), result.Content.Headers.ContentType);
                Assert.AreEqual(_DummyData.Length - 5, result.Content.Headers.ContentLength);
                Assert.AreEqual(System.Net.HttpStatusCode.PartialContent, result.StatusCode);
            }
        }

        [TestMethod]
        public void TestProgressiveDownloadWithMultipleRangeRequest()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(0, 4);
            request.Headers.Range.Ranges.Add(new System.Net.Http.Headers.RangeItemHeaderValue(10, 14));

            var ms = new MemoryStream(_DummyData);
            ms.Seek(0, SeekOrigin.Begin);

            var vidStreamer = new ProgressiveDownload(request);
            using (var result = vidStreamer.ResultMessage(ms, "application/octet-stream"))
            {
                var multipartStart = "multipart/byteranges; boundary=";
                var responseContentType = result.Content.Headers.ContentType.ToString().Substring(0, multipartStart.Length);

                var resultContent = result.Content.ReadAsStringAsync().Result;

                Assert.AreEqual(multipartStart, responseContentType);
                Assert.AreEqual(System.Net.HttpStatusCode.PartialContent, result.StatusCode);
            }
        }

        [TestMethod]
        public void TestProgressiveDownloadWithBadRange()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(50, 100);

            var ms = new MemoryStream(_DummyData);
            ms.Seek(0, SeekOrigin.Begin);

            var vidStreamer = new ProgressiveDownload(request);
            using (var result = vidStreamer.ResultMessage(ms, "application/octet-stream"))
            {
                Assert.AreEqual(System.Net.HttpStatusCode.RequestedRangeNotSatisfiable, result.StatusCode);
            }
        }

        [TestMethod]
        public void TestProgressiveDownloadWithBadResource()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Range = null;

            MemoryStream ms = null;

            var vidStreamer = new ProgressiveDownload(request);
            using (var result = vidStreamer.ResultMessage(ms, "application/octet-stream"))
            {
                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            }
        }

        [TestMethod]
        public void TestProgressiveDownloadWithoutRangeRequestHasAcceptRangesBytesResponseHeader()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Range = null;

            var ms = new MemoryStream(_DummyData);
            ms.Seek(0, SeekOrigin.Begin);

            var vidStreamer = new ProgressiveDownload(request);
            using (var result = vidStreamer.ResultMessage(ms, "application/octet-stream"))
            {
                Assert.IsTrue(result.Headers.AcceptRanges.Contains("bytes"));
            }
        }

        [TestMethod]
        public void TestProgressiveDownloadWithRangeRequestHasAcceptRangesBytesResponseHeader()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(0, 5);

            var ms = new MemoryStream(_DummyData);
            ms.Seek(0, SeekOrigin.Begin);

            var vidStreamer = new ProgressiveDownload(request);
            using (var result = vidStreamer.ResultMessage(ms, "application/octet-stream"))
            {
                Assert.IsTrue(result.Headers.AcceptRanges.Contains("bytes"));
            }
        }

        public ProgressiveDownloadTests()
        {
            _DummyData = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        }

        byte[] _DummyData;
    }
}
