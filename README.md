WebApi.ProgressiveDownloads
===========================

**Overview**

WebApi.ProgressiveDownloads is a library that leverages the HTTP Formatters and System.Net.Http classes to respond appropriately to HTTP requests that may contain BYTE RANGE requests. This library will also handle requests for resources without range requests but due to the limitations of the System.Net.Http classes, you're limited to providing only seekable stream objects as the content source. This reduces the flexibility of your API controller where you're not expecting range requests.

To parse and respond to RANGE requests, this library uses the ByteRangeSteamContent object. If a request is lacking a range header, the library responds with StreamContent since it provides the same calling parameters. This means if you want to support access to a resource by both regular HTTP requests and HTTP requests with a byte range header, you have to wrap your resource in a seekable stream object to be compatible with the ByteRangeStreamContent object. Since the library expects you to provide this stream object, it can fall back on StreamContent where a request is lacking the range header.

The limitation of only supporting seekable stream objects is purely driven by the limtations of the ByteRangeStreamContent class. It's possible to create a new class that will parse the range header and respond appropriately using various other objects like a byte array but this is outside the scope of this library right now. If someone wants to contribute some code and tests to support a wider range of content, that would be great but for not it seems just as easy to wrap byte arrays in a memory stream to pass off to the library.

**NuGet Package Information**

When I have a substantial change, I'll publish a new version [to NuGet](https://www.nuget.org/packages/VikingErik.Net.Http.ProgressiveDownload).

**Usage Information**

Using the library is very simple, assuming you have access to a seekable stream object containing your content. In the example below, we're using the FileStream object which satisfies the requirement. This is how I'm using the library at home to make video files available to my iOS devices for streaming to my AppleTV. Necessity is the mother of invention!
```csharp
[Route("api/video/{filename}")]
public class VideoController : ApiController
{
  public HttpResponseMessage Get(string filename)
  {
    var decodedFileName = Uri.UnescapeDataString(filename);
    var vidFile = File.OpenRead(Path.Combine(@"D:\Videos\", decodedFileName) + ".m4v");

    return new ProgressiveDownload(Request).ResultMessage(vidFile, "video/mp4");
  }
}
```

**Build Information**

I've signed the build with a password protected certificate. This is so the library can be used with trusted assemblies. I chose to keep this private so others can't sign updated assemblies as me. If you're trying to build this source yourself, simply open the project properties for the VikingErik.Net.Http.ProgressiveDownload project, go to the Signing tab and uncheck the option "Sign the assembly" (or replace the key with a new one of your own).

The project depends on WebApi NuGet packages and .NET 4.5. ~~When I get time I'll see if I can walk the NuGet package reference to Microsoft.AspNet.WebApi.Client and Microsoft.AspNet.WebApi.Core from 5.2.2 to a lower version and see what the minimum version is that this library can support.~~ I've downgraded the dependencies to 5.0.0 in the latest version published to NuGet for the WebApi dependencies. The Newtonsoft JSON dependency was also downgraded. All tests continue to pass. This should make the library more compatible with older WebApi deployments where people are hesitant to update their dependencies.
