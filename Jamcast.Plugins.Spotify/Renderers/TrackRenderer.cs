﻿/*-
 * Copyright (c) 2015 Software Development Solutions, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ``AS IS'' AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
 * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
 */

using Jamcast.Extensibility.ContentDirectory;
using Jamcast.Extensibility.Metadata;
using Jamcast.Plugins.Spotify.API;

namespace Jamcast.Plugins.Spotify.Renderers
{
    public class TrackRenderer : ObjectRenderer
    {
        public override ServerObject GetMetadata()
        {
            Track t = new Track(this.ObjectData.ToString());
            AudioItem track = new AudioItem(new MediaServerLocation(typeof(SpotifyTrackHandler), new string[] { this.ObjectData.ToString() }), MediaFormats.LPCM);
            track.Title = t.Name;
            track.Artists.AddRange(t.Artists);
            track.Seconds = t.Seconds;
            track.TrackNumber = t.TrackNumber;
            if (t.Album != null)
            {
                track.Album = t.Album.Name;
                track.AlbumArtist = t.Album.Artist;
                var link = t.Album.GetAlbumArtLink();
                if (link != null)
                    track.AlbumArt = new ImageResource(new MediaServerLocation(typeof(AlbumArtHandler), new string[] { link }), MediaFormats.JPEG);
            }

            return track;
        }
    }
}