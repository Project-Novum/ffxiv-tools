//
// Main.cs
//
// Authors:
//   Gregor Burger burger.gregor@gmail.com
//
// Copyright (C) 2006 Gregor Burger
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using MonoTorrent;
using MonoTorrent.TrackerServer;

namespace tracker;

/// <summary>
/// This is a sample implementation of how you could create a custom ITrackable
/// </summary>
public class CustomITrackable : ITrackable
{
    // I just want to keep the TorrentFiles in memory when i'm tracking the torrent, so i store
    // a reference to them in the ITrackable. This allows me to display information about the
    // files in a GUI without having to keep the entire (really really large) Torrent instance in memory.

    // We require the infohash and the name of the torrent so the tracker can work correctly

    public CustomITrackable(Torrent t)
    {
        // Note: I'm just storing the files, infohash and name. A typical Torrent instance
        // is ~100kB in memory. A typical CustomITrackable will be ~100 bytes.
        Files = t.Files;
        InfoHash = t.InfoHash;
        Name = t.Name;
    }

    /// <summary>
    /// The files in the torrent
    /// </summary>
    public IList<TorrentFile> Files { get; }

    /// <summary>
    /// The infohash of the torrent
    /// </summary>
    public InfoHash InfoHash { get; }

    /// <summary>
    /// The name of the torrent
    /// </summary>
    public string Name { get; }
}
