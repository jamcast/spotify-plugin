/*-
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
using System.Collections.Generic;
using Jamcast.Extensibility.ContentDirectory;
using Jamcast.Extensibility.Metadata;

namespace Jamcast.Plugins.Spotify.Renderers
{
    public class SearchRenderer : ContainerRenderer
    {
        private List<ObjectRenderInfo> _results = new List<ObjectRenderInfo>();

        public override ObjectRenderInfo GetChildAt(int index)
        {
            return _results[index];
        }

        public override int PrepareGetChildren(int startIndex, int count)
        {
            using (var search = Spotify.GetSearch(this.ObjectData.ToString()))
            {
                if (search != null)
                {
                    search.TrackPtrs.ForEach(ptr => { _results.Add(new ObjectRenderInfo(typeof(TrackRenderer), Spotify.GetTrackLink(ptr))); });                    
                    search.PlaylistResults.ForEach(result => { _results.Add(new ObjectRenderInfo(typeof(PlaylistRenderer), result)); });
                    search.ArtistPtrs.ForEach(ptr => { _results.Add(new ObjectRenderInfo(typeof(ArtistRenderer), Spotify.GetArtistLink(ptr))); });
                    search.AlbumPtrs.ForEach(ptr => { _results.Add(new ObjectRenderInfo(typeof(AlbumRenderer), Spotify.GetAlbumLink(ptr))); });
                }
                return _results.Count;
            }
        }

        public override ServerObject GetMetadata()
        {
            throw new NotImplementedException();
        }
    }
}