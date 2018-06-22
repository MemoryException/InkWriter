﻿using InkWriter.Data.EventHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace InkWriter.Data
{
    [XmlRoot()]
    public class InkWriterDocument
    {
        private int activePageIndex = -1;

        public InkWriterDocument()
        {
            this.Pages = new List<Page>();
            this.FilePath = string.Empty;
        }

        public void Save(string filePath)
        {
            using (TextWriter writer = new StreamWriter(filePath, false))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(InkWriterDocument));
                serializer.Serialize(writer, this);
                this.FilePath = filePath;
                this.IsDirty = false;
            }
        }

        public static InkWriterDocument Load(string filePath)
        {
            using (TextReader reader = new StreamReader(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(InkWriterDocument));
                InkWriterDocument loadedDocument = (InkWriterDocument)serializer.Deserialize(reader);
                loadedDocument.FilePath = filePath;
                foreach (Page page in loadedDocument.Pages)
                {
                    page.PageModified += loadedDocument.OnPageModified;
                }

                loadedDocument.IsDirty = false;
                return loadedDocument;
            }
        }

        public void OnPageModified(object sender, PageModifiedEventArgs e)
        {
            this.IsDirty = true;
        }

        public event EventHandler<ActivePageChangedEventArgs> PageChanged;

        [XmlIgnore()]
        public bool IsDirty { get; private set; }

        [XmlArray()]
        public List<Page> Pages { get; private set; }

        [XmlAttribute()]
        public int ActivePageIndex
        {
            get
            {
                return this.activePageIndex;
            }

            set
            {
                this.activePageIndex = value;

                this.PageChanged?.Invoke(this, new ActivePageChangedEventArgs(this.activePageIndex, this.ActivePage));
            }
        }

        [XmlIgnore()]
        public Page ActivePage
        {
            get
            {
                return this.ActivePageIndex < this.Pages.Count && this.ActivePageIndex > -1 ? this.Pages[this.ActivePageIndex] : null;
            }
        }

        [XmlIgnore()]
        public string FilePath { get; private set; }
    }
}
