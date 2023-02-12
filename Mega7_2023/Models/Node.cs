using CG.Web.MegaApiClient;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Syncfusion.Windows.Shared;

namespace Mega7_2023
{
    public class Node : NotificationObject
    {
        string? _nodeId;
        public string NodeId
        {
            get { return _nodeId; }
            set { _nodeId = value; RaisePropertyChanged(nameof(NodeId));}
        }
        NodeType _type;
        public NodeType Type
        {
            get { return _type; }
            set { _type = value; RaisePropertyChanged(nameof(Type));}

        }

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(nameof(Name));}
        }

        long _size;
        public long Size
        {
            get { return _size; }
            set { _size = value; RaisePropertyChanged(nameof(Size)); }
        }

        DateTime? _createdDate;
        public DateTime CreationDate
        {
            get { return (DateTime)_createdDate; }
            set { _createdDate = value; RaisePropertyChanged(nameof(CreationDate)); }

        }
        private string? _parentId;
        public string ParentId
        {
            get { return _parentId; }
            set { _parentId = value; RaisePropertyChanged(nameof(ParentId));}
        }

        ObservableCollection<Node> _items;
        public ObservableCollection<Node> Items
        { 
            get { return _items; }
            set { _items = value; RaisePropertyChanged(nameof(Items));}
        }

    }
}