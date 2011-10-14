﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    [Serializable]
    class TextureDoesNotExistException : Exception
    {
        private string name;

        public TextureDoesNotExistException(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return String.Format("Textrue {0} does not exist.", name);
        }
    }

    [Serializable]
    class EffectDoesNotExistException : Exception
    {
        private string name;

        public EffectDoesNotExistException(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return String.Format("Effect {0} does not exist.", name);
        }
    }

    class Level
    {

        #region Constructor
        internal Level()
        {
            LoadBehavior = () => { };
            UpdateBehavior = () => { };
            EndBehavior = () => { };
        }
        internal Level(string n, Behavior loadBehavior, Behavior updateBehavior, Behavior endBehavior, Condition winCondition)
        {
            mName = n;
            LoadBehavior = loadBehavior;
            UpdateBehavior = updateBehavior;
            EndBehavior = endBehavior;
            WinCondition = winCondition;
        }
        #endregion Constructor

        #region Behaviors
        /// <summary>
        /// Level's load action
        /// </summary>
        internal Behavior LoadBehavior { get; set; }
        /// <summary>
        /// Level's update Behavior
        /// </summary>
        internal Behavior UpdateBehavior { get; set; }
        /// <summary>
        /// Level's End Behavior
        /// </summary>
        internal Behavior EndBehavior { get; set; }
        #endregion Behaviors

        #region Properties
        /// <summary>
        /// Get's level's name
        /// </summary>
        internal string Name { get { return mName; } }
        /// <summary>
        /// Gets and Sets Level's current Background
        /// </summary>
        internal Background Background { get; set; }

        /// <summary>
        /// A Condition to check if the level has been won
        /// </summary>
        internal Condition WinCondition { get; set; }

        /// <summary>
        /// Tells whether the current level is loaded or not
        /// </summary>
        internal bool Loaded { get; set; }
        #endregion Properties

        #region Variables
        /// <summary>
        /// Level's name
        /// </summary>
        string mName = "";

        /// <summary>
        /// Vector of all WorldObjects drawn on the level.
        /// </summary>
        internal List<WorldObject> mSprites = new List<WorldObject>();

        /// <summary>
        /// This level's actors. 
        /// </summary>
        protected Dictionary<string, Actor> mActors = new Dictionary<string, Actor>();

        /// <summary>
        /// This level's Animations
        /// </summary>
        protected Dictionary<string, Animation> mAnims = new Dictionary<string, Animation>();

        /// <summary>
        /// Textures that have been loaded by the level.
        /// </summary>
        protected Dictionary<string, Texture2D> mTextures = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Effects that have been loaded by the level.
        /// </summary>
        protected Dictionary<string, Effect> mEffects = new Dictionary<string, Effect>();

        protected List<WorldObject> ToAdd = new List<WorldObject>();
        protected List<WorldObject> ToRemove = new List<WorldObject>();

        internal Camera Camera = new Camera();

        #endregion Variables


        internal void Load()
        {
            This.Game.AudioManager.Stop();
            mSprites.Clear();
            mActors.Clear();
            mAnims.Clear();
            LoadBehavior();
            Loaded = true;
        }

        internal virtual void Update()
        {
            mSprites.Sort();
            if (Loaded)
            {
                UpdateBehavior();
                if (WinCondition())
                {
                    Unload();
                }
                foreach (Sprite sp in mSprites)
                {
                    if (!WinCondition())
                        sp.UpdateBehavior();
                    else
                    {
                        Unload();
                        return;
                    }
                }
                foreach (var item in ToRemove)
                {
                    mSprites.Remove(item);
                }
                ToRemove.Clear();
                foreach (var item in ToAdd)
                {
                    mSprites.Add(item);
                }
                ToAdd.Clear();
                Collision.Update();
                foreach (Sprite sp in mSprites)
                {
                    sp.DoCollisions();
                }
            }
            else
            {
                /// \todo Show load screen
            }
        }

        internal void Unload()
        {
            This.Game.AudioManager.Stop();
            mSprites.Clear();
            mActors.Clear();
            mAnims.Clear();
            EndBehavior();
        }

        #region Methods

        #region Draw
        internal virtual void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            This.Game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.GetTransformation(This.Game.GraphicsDevice));

            /** Draw Background */
            if (Background != null)
            {
                Background.Draw(gameTime);
            }

            List<WorldObject> staticSprites = new List<WorldObject>();

            #region Draw Sprites

            foreach (var sprite in mSprites)
            {
                if (!sprite.Static)
                {
                    sprite.Draw(gameTime);
                }
                else
                {
                    staticSprites.Add(sprite);
                }
            }

            This.Game.spriteBatch.End();
            #endregion

            #region Draw Static Sprites
            if (staticSprites.Count > 0)
            {
                This.Game.spriteBatch.Begin();

                foreach (var sprite in staticSprites)
                {
                    sprite.Draw(gameTime);
                }

                This.Game.spriteBatch.End();
            }
            #endregion

            /** Draw Boundary Data */
            Collision.Draw(Camera.GetTransformation(This.Game.GraphicsDevice));
        }

        #endregion Drawing

        #region Management
        internal void AddSprite(Sprite sp)
        {
            ToAdd.Add(sp);
        }

        internal Sprite GetSprite(string name)
        {
            return (mSprites.Find(delegate(WorldObject s) { return s.Name == name; }) as Sprite);
        }

        /// <summary>
        /// Retrieves all sprites with the specified type.
        /// @todo Get only sprites within a certain distance of a point, for efficiency's sake.
        ///     Possibly could make use of Bruce's collision code.
        /// </summary>
        /// <param name="typename">The type name to select by.</param>
        /// <returns></returns>
        internal List<Sprite> GetSpritesByType(string typename)
        {
            return (mSprites.FindAll(
                delegate(WorldObject s) { return s.GetType().Name == typename; }).ConvertAll<Sprite>(
                    delegate(WorldObject s) { return s as Sprite; }));
        }

        /// <summary>
        /// Retrieves all sprites with the specified type.
        /// @todo Get only sprites within a certain distance of a point, for efficiency's sake.
        ///     Possibly could make use of Bruce's collision code.
        /// </summary>
        /// <param name="type">The type name to select by.</param>
        /// <returns></returns>
        internal List<Sprite> GetSpritesByType(Type type)
        {
            return (mSprites.FindAll(
                delegate(WorldObject s) { return type.IsAssignableFrom(s.GetType()); }).ConvertAll<Sprite>(
                    delegate(WorldObject s) { return s as Sprite; }));
        }

        internal void RemoveSprite(Sprite sp)
        {
            ToRemove.Add(sp);
        }

        internal void AddAnimation(Animation anim)
        {
            mAnims[anim.Name] = anim;
        }

        internal Animation GetAnimation(string name)
        {
            Animation a;
            if (mAnims.TryGetValue(name, out a))
            {
                return a;
            }
            else
            {
                throw new AnimationDoesNotExistException(name);
            }
        }

        internal void RemoveAnimation(Animation anim)
        {
            try
            {
                mAnims.Remove(anim.Name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }

        internal void AddActor(string name, Actor actor)
        {
            mActors[name] = actor;
        }

        internal void RemoveActor(string name)
        {
            mActors.Remove(name);
        }

        /// <summary>
        /// Returns the texture if it exists loads from disk if it not. Can throw an exception
        /// </summary>
        /// <param name="name">Name of the texture</param>
        /// <returns>The requested texture</returns>
        internal Texture2D GetTexture(string name)
        {
            Texture2D output;
            if (mTextures.TryGetValue(name, out output))
                return output;
            else
            {
                try
                {
                    Texture2D tex = This.Game.Content.Load<Texture2D>(name);
                    mTextures[name] = tex;
                    return tex;
                }
                catch
                {
                    throw new TextureDoesNotExistException(name);
                }
            }
        }

        /// <summary>
        /// Returns the effect if it exists loads from disk if it not. Can throw an exception
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal Effect GetEffect(string name)
        {
            Effect output;
            if (mEffects.TryGetValue(name, out output))
                return output;
            else
            {
                try
                {
                    Effect tex = This.Game.Content.Load<Effect>(name);
                    mEffects[name] = tex;
                    return tex;
                }
                catch
                {
                    throw new EffectDoesNotExistException(name);
                }
            }
        }
        #endregion Management

        public override string ToString()
        {
            return Name;
        }
        #endregion Methods
    }
}
