using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Application;
using Horseshoe.NET.ConsoleX;

namespace TestConsole
{
    class ConfigTests : Routine
    {
        public override Title Title => "Config Tests";

        public override RoutineTitleRenderPolicy RenderTitlePolicy => RoutineTitleRenderPolicy.RenderOnLoop;

        public override Action<MenuSelection<Routine>> OnMenuSelectionRunComplete => (selection) => PromptContinue(padBefore: 2);

        public override bool Looping => true;

        public override bool ClearScreenOnLoop => false;

        public override IEnumerable<Routine> Menu => new[]
        {
            Routine.Build
            (
                "List config collections",
                () =>
                {
                    var configSection = Config.GetSection<MyConfigTest.Section>("MyConfigTestSection", required: true);
                    RenderMenuTitle("Collection");
                    foreach(var element in configSection.Elements.Cast<MyConfigTest.Element>())
                    {
                        Console.WriteLine("Foo = " + element.Foo + "; Bar = " + element.Bar);
                    }
                    Console.WriteLine();
                    RenderMenuTitle("Keyed Collection");
                    foreach(var keyedElement in configSection.KeyedElements.Cast<MyConfigTest.KeyedElement>())
                    {
                        Console.WriteLine("Foo = " + keyedElement.Foo + "; Bar = " + keyedElement.Bar);
                    }
                }
            ),
        };
    }
}

namespace TestConsole.MyConfigTest
{
    public class Section : ConfigurationSection
    {
        [ConfigurationProperty("elements")]
        [ConfigurationCollection(typeof(Collection))]
        public Collection Elements => (Collection)base["elements"];

        [ConfigurationProperty("keyedElements")]
        [ConfigurationCollection(typeof(KeyedCollection))]
        public KeyedCollection KeyedElements => (KeyedCollection)base["keyedElements"];
    }

    public class Collection : ConfigurationElementCollection
    {
        int _counter;

        protected override ConfigurationElement CreateNewElement()
        {
            return new Element();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return _counter++;
        }

        public Element this[int index]
        {
            get
            {
                return (Element)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public Element this[string Name]
        {
            get
            {
                return (Element)BaseGet(Name);
            }
        }

        public int IndexOf(Element element)
        {
            return BaseIndexOf(element);
        }

        public void Add(Element element)
        {
            BaseAdd(element);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(Element element)
        {
            if (BaseIndexOf(element) >= 0)
            {
                BaseRemove(element);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }

    public class KeyedCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new KeyedElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((KeyedElement)element).Foo;
        }

        public KeyedElement this[int index]
        {
            get
            {
                return (KeyedElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public KeyedElement this[string Name]
        {
            get
            {
                return (KeyedElement)BaseGet(Name);
            }
        }

        public int IndexOf(KeyedElement element)
        {
            return BaseIndexOf(element);
        }

        public void Add(KeyedElement element)
        {
            BaseAdd(element);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(KeyedElement element)
        {
            if (BaseIndexOf(element) >= 0)
            {
                BaseRemove(element);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }

    public class Element : ConfigurationElement
    {
        [ConfigurationProperty("foo")]
        public string Foo
        {
            get
            {
                return (string)this["foo"];
            }
            set
            {
                this["foo"] = value;
            }
        }

        [ConfigurationProperty("bar")]
        public string Bar
        {
            get
            {
                return (string)this["bar"];
            }
            set
            {
                this["bar"] = value;
            }
        }
    }

    public class KeyedElement : ConfigurationElement
    {
        [ConfigurationProperty("foo", IsRequired = true, IsKey = true)]
        public string Foo
        {
            get
            {
                return (string)this["foo"];
            }
            set
            {
                this["foo"] = value;
            }
        }

        [ConfigurationProperty("bar")]
        public string Bar
        {
            get
            {
                return (string)this["bar"];
            }
            set
            {
                this["bar"] = value;
            }
        }
    }
}