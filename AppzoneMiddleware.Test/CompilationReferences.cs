using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CSharp;
using Newtonsoft.Json;

namespace AppzoneMiddleware.Test
{
    public static class CompilationReferences
    {
        private static readonly List<MetadataReference> _commandReferences = new List<MetadataReference>();

        private static readonly List<MetadataReference> _messageBrokerReferences = new List<MetadataReference>();

        private static readonly List<MetadataReference> _standardReferences = new List<MetadataReference>();

        public static List<MetadataReference> CommandReferences => _standardReferences.Concat(_commandReferences)
            .ToList();

        public static List<MetadataReference> MessageBrokerReferences => _standardReferences
            .Concat(_messageBrokerReferences).ToList();

        public static void LoadReferences()
        {
            //LoadStandardReferences();
            LoadSafeReferences();

            LoadMessageBrokerReferences();
        }

        private static void LoadStandardReferences()
        {
            var newtonsoftpath = typeof(JsonConverter).Assembly.Location;
            var newtonsoftdoc = Path.ChangeExtension(newtonsoftpath, "xml");
            var documentaionProvider = new FileBasedXmlDocumentationProvider(newtonsoftdoc);
            _standardReferences.Add(MetadataReference.CreateFromFile(newtonsoftpath, new MetadataReferenceProperties(),
                documentaionProvider));


            var CSharpLibpath = typeof(CSharpCodeProvider).Assembly.Location;
            var CSharpLibdocpath = Path.ChangeExtension(CSharpLibpath, "xml");
            var csharpdocprovider = new FileBasedXmlDocumentationProvider(CSharpLibdocpath);
            _standardReferences.Add(MetadataReference.CreateFromFile(CSharpLibpath, new MetadataReferenceProperties(),
                csharpdocprovider));

            var systemcorepath = typeof(Enumerable).Assembly.Location;
            var systemcoredocpath = Path.ChangeExtension(systemcorepath, "xml");
            var systemcoredocprovider = new FileBasedXmlDocumentationProvider(systemcoredocpath);
            _standardReferences.Add(MetadataReference.CreateFromFile(systemcorepath, new MetadataReferenceProperties(),
                systemcoredocprovider));

            var xmlcorepath = typeof(XElement).Assembly.Location;
            var xmlcorepathdoc = Path.ChangeExtension(xmlcorepath, "xml");
            var xmlcoredocprovider = new FileBasedXmlDocumentationProvider(xmlcorepathdoc);
            _standardReferences.Add(MetadataReference.CreateFromFile(xmlcorepath, new MetadataReferenceProperties(),
                xmlcoredocprovider));


            var sysXMLpath = typeof(System.Xml.XmlConvert).Assembly.Location;
            var sysXMLpathdoc = Path.ChangeExtension(sysXMLpath, "xml");
            var sysXMLdocprovider = new FileBasedXmlDocumentationProvider(sysXMLpathdoc);
            _standardReferences.Add(MetadataReference.CreateFromFile(sysXMLpath, new MetadataReferenceProperties(),
                sysXMLdocprovider));

            var sysXMLSerialization = typeof(System.Xml.Serialization.XmlSerializer).Assembly.Location;
            var sysXMLSerializationpathdoc = Path.ChangeExtension(sysXMLSerialization, "xml");
            var sysXMLSerialziationdocprovider = new FileBasedXmlDocumentationProvider(sysXMLSerializationpathdoc);
            _standardReferences.Add(MetadataReference.CreateFromFile(sysXMLSerialization, new MetadataReferenceProperties(),
                sysXMLSerialziationdocprovider));

            var systemxml = typeof(ConformanceLevel).Assembly.Location;
            var systemxmldoc = Path.ChangeExtension(systemxml, "xml");
            var systemdocprovider = new FileBasedXmlDocumentationProvider(systemxmldoc);
            _standardReferences.Add(MetadataReference.CreateFromFile(systemxml, new MetadataReferenceProperties(),
                systemdocprovider));


            var systemSecurity = typeof(MD5CryptoServiceProvider).Assembly.Location;
            var systemSecurityxmldoc = Path.ChangeExtension(systemSecurity, "xml");
            var systemSecuritydocprovider = new FileBasedXmlDocumentationProvider(systemSecurity);
            _standardReferences.Add(MetadataReference.CreateFromFile(systemSecurity, new MetadataReferenceProperties(),
                systemSecuritydocprovider));

            var systemxmllinq = typeof(ConformanceLevel).Assembly.Location;
            var systemxmldoclinq = Path.ChangeExtension(systemxmllinq, "xml");
            var systemdoclinqprovider = new FileBasedXmlDocumentationProvider(systemxmldoclinq);
            _standardReferences.Add(MetadataReference.CreateFromFile(systemxml, new MetadataReferenceProperties(),
                systemdoclinqprovider));

            var mscorlib = typeof(object).Assembly.Location;
            var mscorlidocbpat = Path.ChangeExtension(mscorlib, "xml");
            var mscorlibdocprovider = new FileBasedXmlDocumentationProvider(mscorlidocbpat);
            _standardReferences.Add(MetadataReference.CreateFromFile(mscorlib, new MetadataReferenceProperties(),
                mscorlibdocprovider));
        }

        private static void LoadSafeReferences()
        {
            var newtonsoftpath = typeof(JsonConverter).Assembly.Location;
            //var newtonsoftdoc = Path.ChangeExtension(newtonsoftpath, "xml");
            //var documentaionProvider = new FileBasedXmlDocumentationProvider(newtonsoftdoc);
            _standardReferences.Add(
                MetadataReference
                    .CreateFromFile(newtonsoftpath)); //, new MetadataReferenceProperties(), documentaionProvider));


            var CSharpLibpath = typeof(CSharpCodeProvider).Assembly.Location;
            //var CSharpLibdocpath = Path.ChangeExtension(CSharpLibpath, "xml");
            // var csharpdocprovider = new FileBasedXmlDocumentationProvider(CSharpLibdocpath);
            _standardReferences.Add(
                MetadataReference
                    .CreateFromFile(CSharpLibpath)); //, new MetadataReferenceProperties(), csharpdocprovider));

            var systemcorepath = typeof(Enumerable).Assembly.Location;
            //          var systemcoredocpath = Path.ChangeExtension(systemcorepath, "xml");
            //            var systemcoredocprovider = new FileBasedXmlDocumentationProvider(systemcoredocpath);
            _standardReferences.Add(
                MetadataReference
                    .CreateFromFile(systemcorepath)); //, new MetadataReferenceProperties(), systemcoredocprovider));

            var systemXmlcorepath = typeof(System.Xml.Serialization.XmlSerializer).Assembly.Location;
            _standardReferences.Add(MetadataReference
                .CreateFromFile(systemXmlcorepath));

            var xmlcorepath = typeof(XElement).Assembly.Location;
            //           var xmlcorepathdoc = Path.ChangeExtension(xmlcorepath, "xml");
            //          var xmlcoredocprovider = new FileBasedXmlDocumentationProvider(xmlcorepathdoc);
            _standardReferences.Add(MetadataReference
                .CreateFromFile(xmlcorepath)); //, new MetadataReferenceProperties(), xmlcoredocprovider));

            var systemxml = typeof(ConformanceLevel).Assembly.Location;
            //          var systemxmldoc = Path.ChangeExtension(systemxml, "xml");
            //          var systemdocprovider = new FileBasedXmlDocumentationProvider(systemxmldoc);
            _standardReferences.Add(MetadataReference
                .CreateFromFile(systemxml)); //, new MetadataReferenceProperties(), systemdocprovider));

            var systemSecurity = typeof(MD5CryptoServiceProvider).Assembly.Location;
            //          var systemSecurityxmldoc = Path.ChangeExtension(systemSecurity, "xml");
            //         var systemSecuritydocprovider = new FileBasedXmlDocumentationProvider(systemSecurity);
            _standardReferences.Add(
                MetadataReference
                    .CreateFromFile(
                        systemSecurity)); //, new MetadataReferenceProperties(), systemSecuritydocprovider));

            var systemxmllinq = typeof(ConformanceLevel).Assembly.Location;
            //         var systemxmldoclinq = Path.ChangeExtension(systemxmllinq, "xml");
            //         var systemdoclinqprovider = new FileBasedXmlDocumentationProvider(systemxmldoclinq);
            _standardReferences.Add(MetadataReference
                .CreateFromFile(systemxml)); //, new MetadataReferenceProperties(), systemdoclinqprovider));

            var mscorlib = typeof(object).Assembly.Location;
            //          var mscorlidocbpat = Path.ChangeExtension(mscorlib, "xml");
            //          var mscorlibdocprovider = new FileBasedXmlDocumentationProvider(mscorlidocbpat);
            _standardReferences.Add(MetadataReference
                .CreateFromFile(mscorlib)); //, new MetadataReferenceProperties(), mscorlibdocprovider));
        }

        private static void LoadMessageBrokerReferences()
        {
            //   var contractPath = typeof(IProcessor).Assembly.Location;
            //   _messageBrokerReferences.Add(MetadataReference.CreateFromFile(contractPath));
        }
    }

    internal class FileBasedXmlDocumentationProvider : DocumentationProvider
    {
        private readonly Lazy<Dictionary<string, string>> docComments;
        private readonly string filePath;


        public FileBasedXmlDocumentationProvider(string filePath)
        {
            this.filePath = filePath;
            docComments = new Lazy<Dictionary<string, string>>(CreateDocComments, true);
        }


        public override bool Equals(object obj)
        {
            var other = obj as FileBasedXmlDocumentationProvider;
            return other != null && filePath == other.filePath;
        }

        public override int GetHashCode()
        {
            return filePath.GetHashCode();
        }

        //protected override string GetDocumentationForSymbol(string documentationMemberID, CultureInfo preferredCulture,
        //    CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    string docComment;
        //    return docComments.Value.TryGetValue(documentationMemberID, out docComment) ? docComment : "";
        //}

        protected override string GetDocumentationForSymbol(string documentationMemberID, CultureInfo preferredCulture, CancellationToken cancellationToken = default(CancellationToken))
        {
            string docComment;
            return docComments.Value.TryGetValue(documentationMemberID, out docComment) ? docComment : "";
        }

        private Dictionary<string, string> CreateDocComments()
        {
            var commentsDictionary = new Dictionary<string, string>();
            try
            {
                var foundPath = GetDocumentationFilePath(filePath);
                if (!string.IsNullOrEmpty(foundPath))
                    using (var reader = XmlReader.Create(foundPath))
                    {
                        var document = XDocument.Load(reader);
                        foreach (var element in document.Descendants("member"))
                            if (element.Attribute("name") != null)
                                commentsDictionary[element.Attribute("name").Value] = string.Concat(element.Nodes());
                    }
            }
            catch (Exception)
            {
            }
            return commentsDictionary;
        }

        private static string GetDocumentationFilePath(string originalPath)
        {
            if (File.Exists(originalPath)) return originalPath;

            var fileName = Path.GetFileName(originalPath);
            const string netFrameworkPathPart = @"Reference Assemblies\Microsoft\Framework\.NETFramework";

            var newPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                netFrameworkPathPart, @"v4.6", fileName);
            if (File.Exists(newPath)) return newPath;

            newPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                netFrameworkPathPart, @"v4.5.1", fileName);
            if (File.Exists(newPath)) return newPath;

            newPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                netFrameworkPathPart, @"v4.5", fileName);
            if (File.Exists(newPath)) return newPath;

            return null;
        }
    }
}
