﻿using ClassicUO.AssetsLoader;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ClassicUO.Game.Renderer
{
    public class SpriteTexture : Texture2D
    {      
        public SpriteTexture(in int width, in int height, bool is32bit = true) : base(TextureManager.Device, width,
            height, false, is32bit ? SurfaceFormat.Color : SurfaceFormat.Bgra5551)
        {
            ID = TextureManager.NextID;
        }

        public long Ticks { get; set; }
        public int ID { get; }
    }

    public static class TextureManager
    {
        private const long TEXTURE_TIME_LIFE = 3000;

        private static int _updateIndex;

        private static readonly Dictionary<ushort, SpriteTexture[][][]> _animTextureCache = new Dictionary<ushort, SpriteTexture[][][]>();
        //private static readonly SpriteTexture[] _staticTextureCache = new SpriteTexture[Art.ART_COUNT];
        //private static readonly SpriteTexture[] _landTextureCache = new SpriteTexture[Art.ART_COUNT];
        //private static readonly SpriteTexture[] _gumpTextureCache = new SpriteTexture[Gumps.GUMP_COUNT];
        //private static readonly SpriteTexture[] _textmapTextureCache = new SpriteTexture[TextmapTextures.TEXTMAP_COUNT];
        ////private static readonly SpriteTexture[] _soundTextureCache = new SpriteTexture[]
        //private static readonly SpriteTexture[] _lightTextureCache = new SpriteTexture[Light.LIGHT_COUNT];


        private static readonly Dictionary<ushort, SpriteTexture> _staticTextureCache = new Dictionary<ushort, SpriteTexture>();
        private static readonly Dictionary<ushort, SpriteTexture> _landTextureCache = new Dictionary<ushort, SpriteTexture>();
        private static readonly Dictionary<ushort, SpriteTexture> _gumpTextureCache = new Dictionary<ushort, SpriteTexture>();
        private static readonly Dictionary<ushort, SpriteTexture> _textmapTextureCache = new Dictionary<ushort, SpriteTexture>();
        private static readonly Dictionary<ushort, SpriteTexture> _lightTextureCache = new Dictionary<ushort, SpriteTexture>();


        private static readonly Dictionary<AnimationFrame, SpriteTexture> _animations = new Dictionary<AnimationFrame, SpriteTexture>();


        public static GraphicsDevice Device { get; set; }


        private static int _first = 0;

        public static int NextID
        {
            get
            {
                return _first++;
            }
        }


        public static void Update()
        {

            if (_updateIndex == 0)
            {
                var list = _animations.Where(s => World.Ticks - s.Value.Ticks >= TEXTURE_TIME_LIFE).ToList();

                foreach (var t in list)
                {
                    t.Value.Dispose();
                    _animations.Remove(t.Key);
                }

                _updateIndex++;
            }
            else
            {       
                void check(in Dictionary<ushort, SpriteTexture> dict)
                {
                    var toremove = dict.Where(s => World.Ticks - s.Value.Ticks >= TEXTURE_TIME_LIFE).ToList();
                    foreach (var t in toremove)
                    {
                        dict[t.Key].Dispose();
                        dict[t.Key] = null;
                        dict.Remove(t.Key);
                    }
                    _updateIndex++;
                }

                if (_updateIndex == 1)
                    check(_staticTextureCache);
                else if (_updateIndex == 2)
                    check(_landTextureCache);
                else if (_updateIndex == 3)
                    check(_gumpTextureCache);
                else if (_updateIndex == 4)
                    check(_textmapTextureCache);
                else if (_updateIndex == 5)
                    check(_lightTextureCache);
                else
                    _updateIndex = 0;
            }
        }

        public static SpriteTexture GetOrCreateAnimTexture(in AnimationFrame frame)
        {
            if (!_animations.TryGetValue(frame, out var sprite))
            {
                sprite = new SpriteTexture(frame.Width, frame.Heigth, false)
                {
                    Ticks = World.Ticks
                };
                sprite.SetData(frame.Pixels);
                _animations[frame] = sprite;
            }
            else
                sprite.Ticks = World.Ticks;

            return sprite;
        }

     
        public static SpriteTexture GetOrCreateStaticTexture(in ushort g)
        {
            if (!_staticTextureCache.TryGetValue(g, out var texture))
            {
                ushort[] pixels = Art.ReadStaticArt(g, out short w, out short h);

                texture = new SpriteTexture(w, h, false)
                {
                    Ticks = World.Ticks
                };

                texture.SetData(pixels);
                _staticTextureCache[g] = texture;
            }

            return texture;
        }

        public static SpriteTexture GetOrCreateLandTexture(in ushort g)
        {
            if (!_landTextureCache.TryGetValue(g, out var texture))
            {
                ushort[] pixels = Art.ReadLandArt(g);
                texture = new SpriteTexture(44, 44, false)
                {
                    Ticks = World.Ticks
                };
                texture.SetData(pixels);
                _landTextureCache[g] = texture;
            }

            return texture;
        }

        public static SpriteTexture GetOrCreateGumpTexture(in ushort g)
        {
            if (!_gumpTextureCache.TryGetValue(g, out var texture))
            {
                ushort[] pixels = Gumps.GetGump(g, out int w, out int h);
                texture = new SpriteTexture(w, h, false)
                {
                    Ticks = World.Ticks
                };
                texture.SetData(pixels);
                _gumpTextureCache[g] = texture;
            }

            return texture;
        }

        public static SpriteTexture GetOrCreateTexmapTexture(in ushort g)
        {
            if (!_textmapTextureCache.TryGetValue(g, out var texture))
            {
                ushort[] pixels = TextmapTextures.GetTextmapTexture(g, out int size);
                texture = new SpriteTexture(size, size, false)
                {
                    Ticks = World.Ticks
                };
                texture.SetData(pixels);
                _textmapTextureCache[g] = texture;
            }

            return texture;
        }
    }
}