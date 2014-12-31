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

using System.Collections.Generic;

using Jamcast.Extensibility.ContentDirectory;
using Jamcast.Extensibility.Metadata;
using Jamcast.Plugins.Spotify.API;

using libspotifydotnet;

namespace Jamcast.Plugins.Spotify.Renderers
{
    internal class PlaylistMenuRenderer : ContainerRenderer
    {
        private List<PlaylistContainer.PlaylistInfo> children = null;

        public override int PrepareGetChildren(int startIndex, int count)
        {
            if (this.ObjectData == null)
            {
                children = Spotify.GetPlaylists();
            }
            else
            {
                children = Spotify.GetPlaylists(ulong.Parse(this.ObjectData.ToString()));
            }
            return children == null ? 0 : children.Count;
        }

        public override ObjectRenderInfo GetChildAt(int index)
        {
            if (children[index].PlaylistType == libspotify.sp_playlist_type.SP_PLAYLIST_TYPE_PLAYLIST)
            {
                return new ObjectRenderInfo(typeof(PlaylistRenderer), Playlist.GetLink(children[index].Pointer));
            }
            else
            {
                return new ObjectRenderInfo(typeof(PlaylistMenuRenderer), children[index].FolderID.ToString());
            }
        }

        public override ServerObject GetMetadata()
        {
            if (this.ObjectData == null)
            {
                return new GenericContainer("Playlists");
            }
            else
            {
                return new GenericContainer(Spotify.GetPlaylistContainer(ulong.Parse(this.ObjectData.ToString())).Name);
            }
        }
    }
}