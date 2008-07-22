﻿using System;
using System.Xml;

namespace Alsing.Serialization
{
    public abstract class ObjectManager
    {
        public abstract bool CanSerialize(SerializerEngine engine, object item);
        public abstract MetaObject SerializerGetObject(SerializerEngine engine, object item);


        public abstract bool CanDeserialize(DeserializerEngine engine, XmlNode objectNode);
        public abstract object DeserializerCreateObject(DeserializerEngine engine, XmlNode objectNode);
        public abstract void DeserializerSetupObject(DeserializerEngine engine, XmlNode objectNode, object instance);
        
        public abstract bool CanDeserializeValue(DeserializerEngine engine, XmlNode node);
        public abstract object DeserializerGetValue(DeserializerEngine engine, XmlNode node,Type fieldType);
    }

    public abstract class ObjectManager<T> : ObjectManager where T : MetaObject, new()
    {
        public override MetaObject SerializerGetObject(SerializerEngine engine, object item)
        {
            var current = new T();
            engine.RegisterObject(current, item);
            current.Build(engine, item);
            return current;
        }

        public override object DeserializerCreateObject(DeserializerEngine engine, XmlNode objectNode)
        {
            string typeAlias = objectNode.Attributes["type"].Value;
            Type type = engine.TypeLookup[typeAlias];

            //ignore if type is missing
            if (type == null)
                return null;

            object instance = Activator.CreateInstance(type);

            return instance;
        }

        public override bool CanDeserializeValue(DeserializerEngine engine, XmlNode node)
        {
            XmlAttribute idRefAttrib = node.Attributes[Constants.IdRef];

            if (idRefAttrib == null)
                return false;

            string id = idRefAttrib.Value;

            var manager = engine.InstanceManager[id];
            return manager == this;
        }

        public override object DeserializerGetValue(DeserializerEngine engine, XmlNode node, Type fieldType)
        {
            XmlAttribute idRefAttrib = node.Attributes[Constants.IdRef];
            string id = idRefAttrib.Value;

            return engine.ObjectLookup[id];
        }
    }
}