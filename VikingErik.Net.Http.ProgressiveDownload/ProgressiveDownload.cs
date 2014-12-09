using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace VikingErik.Net.Http
{
    public class ProgressiveDownload
    {
        public bool IsRangeRequest
        {
            get
            {
                return _Request.Headers.Range != null &&
                _Request.Headers.Range.Ranges.Count > 0;
            }
        }

        public HttpResponseMessage ResultMessage(Stream stream, string mediaType)
        {
            try
            {
                if (IsRangeRequest)
                {
                    var content = new ByteRangeStreamContent(stream, _Request.Headers.Range, mediaType);
                    var response = _Request.CreateResponse(HttpStatusCode.PartialContent);
                    response.Content = content;

                    return response;
                }
                else
                {
                    var content = new StreamContent(stream);
                    var response = _Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = content;
                    response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(mediaType);

                    return response;
                }
            }
            catch (InvalidByteRangeException ibr)
            {
                return _Request.CreateErrorResponse(ibr);
            }
            catch (Exception e)
            {
                return _Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        public ProgressiveDownload(HttpRequestMessage request)
        {
            _Request = request;
        }

        HttpRequestMessage _Request;
    }
}
