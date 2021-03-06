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

using System;

using Jamcast.Extensibility.ContentDirectory;
using Jamcast.Extensibility.Metadata;

namespace Jamcast.Plugins.Spotify.Renderers
{
    [ContainerRenderer(ContainerType.Album)]
    public class AlbumRenderer : ContainerRenderer
    {
        private IntPtr[] tracks;

        public override int PrepareGetChildren(int startIndex, int count)
        {
            tracks = Spotify.GetAlbumTracks(this.ObjectData.ToString());
            return tracks == null ? 0 : tracks.Length;
        }

        public override ObjectRenderInfo GetChildAt(int index)
        {
            return new ObjectRenderInfo(typeof(TrackRenderer), Spotify.GetTrackLink(tracks[index], 0));
        }

        public override ServerObject GetMetadata()
        {
            using (var album = Spotify.AlbumFromLink(this.ObjectData.ToString()))
            {
                var link = album.GetAlbumArtLink();
                return new AlbumContainer(album.Name, album.Artist, link == null ? null : new ImageResource(new MediaServerLocation(typeof(AlbumArtHandler), new string[] { link }), MediaFormats.JPEG));
            }
        }
    }
}