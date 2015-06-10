﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Xml.Schema.Linq;

namespace Csharper.SMSServiceGate.Wrappers.SendSMSReq
{

    /// <summary>
    /// <para>
    /// Regular expression: (login, pwd, (originator, phone_to, message, sync)+)
    /// </para>
    /// </summary>
    public partial class request : XTypedElement, IXMetaData
    {

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XTypedList<originatorLocalType> originatorField;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XTypedList<phone_toLocalType> phone_toField;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XTypedList<messageLocalType> messageField;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XTypedList<syncLocalType> syncField;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Dictionary<XName, System.Type> localElementDictionary = new Dictionary<XName, System.Type>();

        public static explicit operator request(XElement xe) { return XTypedServices.ToXTypedElement<request>(xe, LinqToXsdTypeManager.Instance as ILinqToXsdTypeManager); }

        static request()
        {
            BuildElementDictionary();
        }

        /// <summary>
        /// <para>
        /// Regular expression: (login, pwd, (originator, phone_to, message, sync)+)
        /// </para>
        /// </summary>
        public request()
        {
        }

        /// <summary>
        /// <para>
        /// Occurrence: required
        /// </para>
        /// <para>
        /// Regular expression: (login, pwd, (originator, phone_to, message, sync)+)
        /// </para>
        /// </summary>
        public string login
        {
            get
            {
                XElement x = this.GetElement(XName.Get("login", ""));
                return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
            }
            set
            {
                this.SetElement(XName.Get("login", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
            }
        }

        /// <summary>
        /// <para>
        /// Occurrence: required
        /// </para>
        /// <para>
        /// Regular expression: (login, pwd, (originator, phone_to, message, sync)+)
        /// </para>
        /// </summary>
        public string pwd
        {
            get
            {
                XElement x = this.GetElement(XName.Get("pwd", ""));
                return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
            }
            set
            {
                this.SetElement(XName.Get("pwd", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
            }
        }

        /// <summary>
        /// <para>
        /// Occurrence: required
        /// </para>
        /// <para>
        /// Setter: Appends
        /// </para>
        /// <para>
        /// Regular expression: (login, pwd, (originator, phone_to, message, sync)+)
        /// </para>
        /// </summary>
        public IList<request.originatorLocalType> originator
        {
            get
            {
                if ((this.originatorField == null))
                {
                    this.originatorField = new XTypedList<originatorLocalType>(this, LinqToXsdTypeManager.Instance, XName.Get("originator", ""));
                }
                return this.originatorField;
            }
            set
            {
                if ((value == null))
                {
                    this.originatorField = null;
                }
                else
                {
                    if ((this.originatorField == null))
                    {
                        this.originatorField = XTypedList<originatorLocalType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("originator", ""));
                    }
                    else
                    {
                        XTypedServices.SetList<originatorLocalType>(this.originatorField, value);
                    }
                }
            }
        }

        /// <summary>
        /// <para>
        /// Occurrence: required
        /// </para>
        /// <para>
        /// Setter: Appends
        /// </para>
        /// <para>
        /// Regular expression: (login, pwd, (originator, phone_to, message, sync)+)
        /// </para>
        /// </summary>
        public IList<request.phone_toLocalType> phone_to
        {
            get
            {
                if ((this.phone_toField == null))
                {
                    this.phone_toField = new XTypedList<phone_toLocalType>(this, LinqToXsdTypeManager.Instance, XName.Get("phone_to", ""));
                }
                return this.phone_toField;
            }
            set
            {
                if ((value == null))
                {
                    this.phone_toField = null;
                }
                else
                {
                    if ((this.phone_toField == null))
                    {
                        this.phone_toField = XTypedList<phone_toLocalType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("phone_to", ""));
                    }
                    else
                    {
                        XTypedServices.SetList<phone_toLocalType>(this.phone_toField, value);
                    }
                }
            }
        }

        /// <summary>
        /// <para>
        /// Occurrence: required
        /// </para>
        /// <para>
        /// Setter: Appends
        /// </para>
        /// <para>
        /// Regular expression: (login, pwd, (originator, phone_to, message, sync)+)
        /// </para>
        /// </summary>
        public IList<request.messageLocalType> message
        {
            get
            {
                if ((this.messageField == null))
                {
                    this.messageField = new XTypedList<messageLocalType>(this, LinqToXsdTypeManager.Instance, XName.Get("message", ""));
                }
                return this.messageField;
            }
            set
            {
                if ((value == null))
                {
                    this.messageField = null;
                }
                else
                {
                    if ((this.messageField == null))
                    {
                        this.messageField = XTypedList<messageLocalType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("message", ""));
                    }
                    else
                    {
                        XTypedServices.SetList<messageLocalType>(this.messageField, value);
                    }
                }
            }
        }

        /// <summary>
        /// <para>
        /// Occurrence: required
        /// </para>
        /// <para>
        /// Setter: Appends
        /// </para>
        /// <para>
        /// Regular expression: (login, pwd, (originator, phone_to, message, sync)+)
        /// </para>
        /// </summary>
        public IList<request.syncLocalType> sync
        {
            get
            {
                if ((this.syncField == null))
                {
                    this.syncField = new XTypedList<syncLocalType>(this, LinqToXsdTypeManager.Instance, XName.Get("sync", ""));
                }
                return this.syncField;
            }
            set
            {
                if ((value == null))
                {
                    this.syncField = null;
                }
                else
                {
                    if ((this.syncField == null))
                    {
                        this.syncField = XTypedList<syncLocalType>.Initialize(this, LinqToXsdTypeManager.Instance, value, XName.Get("sync", ""));
                    }
                    else
                    {
                        XTypedServices.SetList<syncLocalType>(this.syncField, value);
                    }
                }
            }
        }

        /// <summary>
        /// <para>
        /// Occurrence: required
        /// </para>
        /// </summary>
        public string method
        {
            get
            {
                XAttribute x = this.Attribute(XName.Get("method", ""));
                return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
            }
            set
            {
                this.SetAttribute(XName.Get("method", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Dictionary<XName, System.Type> IXMetaData.LocalElementsDictionary
        {
            get
            {
                return localElementDictionary;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        XName IXMetaData.SchemaName
        {
            get
            {
                return XName.Get("request", "");
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        SchemaOrigin IXMetaData.TypeOrigin
        {
            get
            {
                return SchemaOrigin.Element;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILinqToXsdTypeManager IXMetaData.TypeManager
        {
            get
            {
                return LinqToXsdTypeManager.Instance;
            }
        }

        public void Save(string xmlFile)
        {
            XTypedServices.Save(xmlFile, Untyped);
        }

        public void Save(System.IO.TextWriter tw)
        {
            XTypedServices.Save(tw, Untyped);
        }

        public void Save(System.Xml.XmlWriter xmlWriter)
        {
            XTypedServices.Save(xmlWriter, Untyped);
        }

        public static request Load(string xmlFile)
        {
            return XTypedServices.Load<request>(xmlFile);
        }

        public static request Load(System.IO.TextReader xmlFile)
        {
            return XTypedServices.Load<request>(xmlFile);
        }

        public static request Parse(string xml)
        {
            return XTypedServices.Parse<request>(xml);
        }

        public override XTypedElement Clone()
        {
            return XTypedServices.CloneXTypedElement<request>(this);
        }

        private static void BuildElementDictionary()
        {
            localElementDictionary.Add(XName.Get("login", ""), typeof(string));
            localElementDictionary.Add(XName.Get("pwd", ""), typeof(string));
            localElementDictionary.Add(XName.Get("originator", ""), typeof(originatorLocalType));
            localElementDictionary.Add(XName.Get("phone_to", ""), typeof(phone_toLocalType));
            localElementDictionary.Add(XName.Get("message", ""), typeof(messageLocalType));
            localElementDictionary.Add(XName.Get("sync", ""), typeof(syncLocalType));
        }

        ContentModelEntity IXMetaData.GetContentModel()
        {
            return ContentModelEntity.Default;
        }

        public partial class originatorLocalType : XTypedElement, IXMetaData
        {

            public static explicit operator originatorLocalType(XElement xe) { return XTypedServices.ToXTypedElement<originatorLocalType>(xe, LinqToXsdTypeManager.Instance as ILinqToXsdTypeManager); }

            public originatorLocalType()
            {
            }

            public string TypedValue
            {
                get
                {
                    XElement x = this.Untyped;
                    return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
                }
                set
                {
                    this.SetValue(value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
                }
            }

            /// <summary>
            /// <para>
            /// Occurrence: required
            /// </para>
            /// </summary>
            public uint num_message
            {
                get
                {
                    XAttribute x = this.Attribute(XName.Get("num_message", ""));
                    return XTypedServices.ParseValue<uint>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt).Datatype);
                }
                set
                {
                    this.SetAttribute(XName.Get("num_message", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt).Datatype);
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            XName IXMetaData.SchemaName
            {
                get
                {
                    return XName.Get("originator", "");
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            SchemaOrigin IXMetaData.TypeOrigin
            {
                get
                {
                    return SchemaOrigin.Fragment;
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            ILinqToXsdTypeManager IXMetaData.TypeManager
            {
                get
                {
                    return LinqToXsdTypeManager.Instance;
                }
            }

            public override XTypedElement Clone()
            {
                return XTypedServices.CloneXTypedElement<originatorLocalType>(this);
            }

            ContentModelEntity IXMetaData.GetContentModel()
            {
                return ContentModelEntity.Default;
            }
        }

        public partial class phone_toLocalType : XTypedElement, IXMetaData
        {

            public static explicit operator phone_toLocalType(XElement xe) { return XTypedServices.ToXTypedElement<phone_toLocalType>(xe, LinqToXsdTypeManager.Instance as ILinqToXsdTypeManager); }

            public phone_toLocalType()
            {
            }

            public string TypedValue
            {
                get
                {
                    XElement x = this.Untyped;
                    return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
                }
                set
                {
                    this.SetValueWithValidation(value, "TypedValue", phoneNumber.TypeDefinition);
                }
            }

            /// <summary>
            /// <para>
            /// Occurrence: required
            /// </para>
            /// </summary>
            public uint num_message
            {
                get
                {
                    XAttribute x = this.Attribute(XName.Get("num_message", ""));
                    return XTypedServices.ParseValue<uint>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt).Datatype);
                }
                set
                {
                    this.SetAttribute(XName.Get("num_message", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt).Datatype);
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            XName IXMetaData.SchemaName
            {
                get
                {
                    return XName.Get("phone_to", "");
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            SchemaOrigin IXMetaData.TypeOrigin
            {
                get
                {
                    return SchemaOrigin.Fragment;
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            ILinqToXsdTypeManager IXMetaData.TypeManager
            {
                get
                {
                    return LinqToXsdTypeManager.Instance;
                }
            }

            public override XTypedElement Clone()
            {
                return XTypedServices.CloneXTypedElement<phone_toLocalType>(this);
            }

            ContentModelEntity IXMetaData.GetContentModel()
            {
                return ContentModelEntity.Default;
            }
        }

        public partial class messageLocalType : XTypedElement, IXMetaData
        {

            public static explicit operator messageLocalType(XElement xe) { return XTypedServices.ToXTypedElement<messageLocalType>(xe, LinqToXsdTypeManager.Instance as ILinqToXsdTypeManager); }

            public messageLocalType()
            {
            }

            public string TypedValue
            {
                get
                {
                    XElement x = this.Untyped;
                    return XTypedServices.ParseValue<string>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
                }
                set
                {
                    this.SetValue(value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).Datatype);
                }
            }

            /// <summary>
            /// <para>
            /// Occurrence: required
            /// </para>
            /// </summary>
            public uint num_message
            {
                get
                {
                    XAttribute x = this.Attribute(XName.Get("num_message", ""));
                    return XTypedServices.ParseValue<uint>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt).Datatype);
                }
                set
                {
                    this.SetAttribute(XName.Get("num_message", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt).Datatype);
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            XName IXMetaData.SchemaName
            {
                get
                {
                    return XName.Get("message", "");
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            SchemaOrigin IXMetaData.TypeOrigin
            {
                get
                {
                    return SchemaOrigin.Fragment;
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            ILinqToXsdTypeManager IXMetaData.TypeManager
            {
                get
                {
                    return LinqToXsdTypeManager.Instance;
                }
            }

            public override XTypedElement Clone()
            {
                return XTypedServices.CloneXTypedElement<messageLocalType>(this);
            }

            ContentModelEntity IXMetaData.GetContentModel()
            {
                return ContentModelEntity.Default;
            }
        }

        public partial class syncLocalType : XTypedElement, IXMetaData
        {

            public static explicit operator syncLocalType(XElement xe) { return XTypedServices.ToXTypedElement<syncLocalType>(xe, LinqToXsdTypeManager.Instance as ILinqToXsdTypeManager); }

            public syncLocalType()
            {
            }

            public decimal TypedValue
            {
                get
                {
                    XElement x = this.Untyped;
                    return XTypedServices.ParseValue<decimal>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Integer).Datatype);
                }
                set
                {
                    this.SetValue(value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Integer).Datatype);
                }
            }

            /// <summary>
            /// <para>
            /// Occurrence: required
            /// </para>
            /// </summary>
            public uint num_message
            {
                get
                {
                    XAttribute x = this.Attribute(XName.Get("num_message", ""));
                    return XTypedServices.ParseValue<uint>(x, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt).Datatype);
                }
                set
                {
                    this.SetAttribute(XName.Get("num_message", ""), value, XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.UnsignedInt).Datatype);
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            XName IXMetaData.SchemaName
            {
                get
                {
                    return XName.Get("sync", "");
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            SchemaOrigin IXMetaData.TypeOrigin
            {
                get
                {
                    return SchemaOrigin.Fragment;
                }
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            ILinqToXsdTypeManager IXMetaData.TypeManager
            {
                get
                {
                    return LinqToXsdTypeManager.Instance;
                }
            }

            public override XTypedElement Clone()
            {
                return XTypedServices.CloneXTypedElement<syncLocalType>(this);
            }

            ContentModelEntity IXMetaData.GetContentModel()
            {
                return ContentModelEntity.Default;
            }
        }
    }

    public sealed class phoneNumber
    {

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static Xml.Schema.Linq.SimpleTypeValidator TypeDefinition = new Xml.Schema.Linq.AtomicSimpleTypeValidator(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), new Xml.Schema.Linq.RestrictionFacets(((Xml.Schema.Linq.RestrictionFlags)(8)), null, 0, 0, null, null, 0, null, null, 0, new string[] {
                    "\\+\\d+"}, 0, XmlSchemaWhiteSpace.Preserve));

        private phoneNumber()
        {
        }
    }

    public class LinqToXsdTypeManager : ILinqToXsdTypeManager
    {

        static Dictionary<XName, System.Type> elementDictionary = new Dictionary<XName, System.Type>();

        private static XmlSchemaSet schemaSet;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static LinqToXsdTypeManager typeManagerSingleton = new LinqToXsdTypeManager();

        static LinqToXsdTypeManager()
        {
            BuildElementDictionary();
        }

        XmlSchemaSet ILinqToXsdTypeManager.Schemas
        {
            get
            {
                if ((schemaSet == null))
                {
                    XmlSchemaSet tempSet = new XmlSchemaSet();
                    System.Threading.Interlocked.CompareExchange(ref schemaSet, tempSet, null);
                }
                return schemaSet;
            }
            set
            {
                schemaSet = value;
            }
        }

        Dictionary<XName, System.Type> ILinqToXsdTypeManager.GlobalTypeDictionary
        {
            get
            {
                return XTypedServices.EmptyDictionary;
            }
        }

        Dictionary<XName, System.Type> ILinqToXsdTypeManager.GlobalElementDictionary
        {
            get
            {
                return elementDictionary;
            }
        }

        Dictionary<System.Type, System.Type> ILinqToXsdTypeManager.RootContentTypeMapping
        {
            get
            {
                return XTypedServices.EmptyTypeMappingDictionary;
            }
        }

        public static LinqToXsdTypeManager Instance
        {
            get
            {
                return typeManagerSingleton;
            }
        }

        private static void BuildElementDictionary()
        {
            elementDictionary.Add(XName.Get("request", ""), typeof(request));
        }

        protected internal static void AddSchemas(XmlSchemaSet schemas)
        {
            schemas.Add(schemaSet);
        }

        public static System.Type GetRootType()
        {
            return elementDictionary[XName.Get("request", "")];
        }
    }

    public partial class XRootNamespace
    {

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XDocument doc;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XTypedElement rootObject;


        public request request { get { return rootObject as request; } }

        private XRootNamespace()
        {
        }

        public XRootNamespace(request root)
        {
            this.doc = new XDocument(root.Untyped);
            this.rootObject = root;
        }

        public XDocument XDocument
        {
            get
            {
                return doc;
            }
        }

        public static XRootNamespace Load(string xmlFile)
        {
            XRootNamespace root = new XRootNamespace();
            root.doc = XDocument.Load(xmlFile);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRootNamespace Load(string xmlFile, LoadOptions options)
        {
            XRootNamespace root = new XRootNamespace();
            root.doc = XDocument.Load(xmlFile, options);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRootNamespace Load(TextReader textReader)
        {
            XRootNamespace root = new XRootNamespace();
            root.doc = XDocument.Load(textReader);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRootNamespace Load(TextReader textReader, LoadOptions options)
        {
            XRootNamespace root = new XRootNamespace();
            root.doc = XDocument.Load(textReader, options);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRootNamespace Load(XmlReader xmlReader)
        {
            XRootNamespace root = new XRootNamespace();
            root.doc = XDocument.Load(xmlReader);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRootNamespace Parse(string text)
        {
            XRootNamespace root = new XRootNamespace();
            root.doc = XDocument.Parse(text);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRootNamespace Parse(string text, LoadOptions options)
        {
            XRootNamespace root = new XRootNamespace();
            root.doc = XDocument.Parse(text, options);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public virtual void Save(string fileName)
        {
            doc.Save(fileName);
        }

        public virtual void Save(TextWriter textWriter)
        {
            doc.Save(textWriter);
        }

        public virtual void Save(XmlWriter writer)
        {
            doc.Save(writer);
        }

        public virtual void Save(TextWriter textWriter, SaveOptions options)
        {
            doc.Save(textWriter, options);
        }

        public virtual void Save(string fileName, SaveOptions options)
        {
            doc.Save(fileName, options);
        }
    }

    public partial class XRoot
    {

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XDocument doc;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private XTypedElement rootObject;


        public request request { get { return rootObject as request; } }

        private XRoot()
        {
        }

        public XRoot(request root)
        {
            this.doc = new XDocument(root.Untyped);
            this.rootObject = root;
        }

        public XDocument XDocument
        {
            get
            {
                return doc;
            }
        }

        public static XRoot Load(string xmlFile)
        {
            XRoot root = new XRoot();
            root.doc = XDocument.Load(xmlFile);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRoot Load(string xmlFile, LoadOptions options)
        {
            XRoot root = new XRoot();
            root.doc = XDocument.Load(xmlFile, options);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRoot Load(TextReader textReader)
        {
            XRoot root = new XRoot();
            root.doc = XDocument.Load(textReader);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRoot Load(TextReader textReader, LoadOptions options)
        {
            XRoot root = new XRoot();
            root.doc = XDocument.Load(textReader, options);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRoot Load(XmlReader xmlReader)
        {
            XRoot root = new XRoot();
            root.doc = XDocument.Load(xmlReader);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRoot Parse(string text)
        {
            XRoot root = new XRoot();
            root.doc = XDocument.Parse(text);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public static XRoot Parse(string text, LoadOptions options)
        {
            XRoot root = new XRoot();
            root.doc = XDocument.Parse(text, options);
            XTypedElement typedRoot = XTypedServices.ToXTypedElement(root.doc.Root, LinqToXsdTypeManager.Instance);
            if ((typedRoot == null))
            {
                throw new LinqToXsdException("Invalid root element in xml document.");
            }
            root.rootObject = typedRoot;
            return root;
        }

        public virtual void Save(string fileName)
        {
            doc.Save(fileName);
        }

        public virtual void Save(TextWriter textWriter)
        {
            doc.Save(textWriter);
        }

        public virtual void Save(XmlWriter writer)
        {
            doc.Save(writer);
        }

        public virtual void Save(TextWriter textWriter, SaveOptions options)
        {
            doc.Save(textWriter, options);
        }

        public virtual void Save(string fileName, SaveOptions options)
        {
            doc.Save(fileName, options);
        }
    }

}