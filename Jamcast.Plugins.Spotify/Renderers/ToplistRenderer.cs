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

using Jamcast.Extensibility.ContentDirectory;
using Jamcast.Extensibility.Metadata;
using Jamcast.Plugins.Spotify.API;

using libspotifydotnet;

namespace Jamcast.Plugins.Spotify.Renderers
{
    [ContainerRenderer(ContainerType.Playlist)]
    public class ToplistRenderer : ContainerRenderer
    {
        private TopList toplist;

        public override int PrepareGetChildren(int startIndex, int count)
        {
            toplist = Spotify.GetToplist(this.ObjectData.ToString());
            return toplist == null ? 0 : toplist.Ptrs.Count;
        }

        public override ObjectRenderInfo GetChildAt(int index)
        {
            switch (toplist.ToplistType)
            {
                case libspotify.sp_toplisttype.SP_TOPLIST_TYPE_ALBUMS:
                    return new ObjectRenderInfo(typeof(AlbumRenderer), Spotify.GetAlbumLink(toplist.Ptrs[index]));

                case libspotify.sp_toplisttype.SP_TOPLIST_TYPE_ARTISTS:
                    return new ObjectRenderInfo(typeof(ArtistRenderer), Spotify.GetArtistLink(toplist.Ptrs[index]));

                case libspotify.sp_toplisttype.SP_TOPLIST_TYPE_TRACKS:
                    return new ObjectRenderInfo(typeof(TrackRenderer), Spotify.GetTrackLink(toplist.Ptrs[index], 0));

                default:
                    throw new ApplicationException("Unexpected toplist type");
            }
        }

        public override void EndGetChildren()
        {
            if (toplist != null)
                toplist.Dispose();
        }

        public override ServerObject GetMetadata()
        {
            string[] parts = this.ObjectData.ToString().Split("|".ToCharArray());
            return new GenericContainer(parts[1]);
        }
    }
}