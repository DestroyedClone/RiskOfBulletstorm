using BepInEx.Configuration;
using RiskOfBulletstormRewrite.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskOfBulletstormRewrite.Interactables
{
    public abstract class InteractableBase<T> : InteractableBase where T : InteractableBase<T>
    {
        //This, which you will see on all the -base classes, will allow both you and other modders to enter through any class with this to access internal fields/properties/etc as if they were a member inheriting this -Base too from this class.
        public static T instance { get; private set; }

        public InteractableBase()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting InteractableBase was instantiated twice");
            instance = this as T;
        }
    }

    public abstract class InteractableBase
    {        /// <summary>
             /// UNUSED Name of its configuration category.
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
