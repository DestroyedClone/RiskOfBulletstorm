using BepInEx.Configuration;
using System;

namespace RiskOfBulletstormRewrite.Controllers
{
    public abstract class ControllerBase<T> : ControllerBase where T : ControllerBase<T>
    {
        //This, which you will see on all the -base classes, will allow both you and other modders to enter through any class with this to access internal fields/properties/etc as if they were a member inheriting this -Base too from this class.
        public static T Instance { get; private set; }

        public ControllerBase()
        {
            if (Instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ControllerBase was instantiated twice");
            Instance = this as T;
        }
    }

    public abstract class ControllerBase
    {   /// <summary>
        /// Name of its configuration category.
        /// <para>"Config: {}</para>
        /// </summary>
        public virtual string ConfigCategory { get; }

        /// <summary>
        /// This method structures your code execution of this class. An example implementation inside of it would be:
        /// <para>CreateConfig(config);</para>
        /// <para>CreateLang();</para>
        /// <para>Hooks();</para>
        /// <para>This ensures that these execute in this order, one after another, and is useful for having things available to be used in later methods.</para>
        /// </summary>
        /// <param name="config">The config file that will be passed into this from the main class.</param>
        public abstract void Init(ConfigFile config);

        public virtual void CreateConfig(ConfigFile config)
        { }

        protected virtual void CreateLang()
        {
        }

        public virtual void Hooks()
        { }
    }
}