using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CG.Web.MegaApiClient;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Syncfusion.UI.Xaml.Diagram;

namespace Mega7_2023
{
    public class ViewModel
    {

        public ViewModel()
        {
            {              
                
                this.Nodes = GetNodesCollection();
            }
        }

        private ObservableCollection<Node> _Nodes;
        /// <summary>
        /// Gets or sets the employee info.
        /// </summary>
        /// <value>The employee info.</value>
        public ObservableCollection<Node> Nodes
        {
            get
            {
                return _Nodes;
            }
            set
            {
                _Nodes = value;
            }
        }

        private object _selectedItem;
        public object SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        private bool? isselectAll = false;
        public bool? IsSelectAll
        {
            get
            {
                return isselectAll;
            }
            set
            {
                isselectAll = value;
                RaisePropertyChanged("IsSelectAll");
            }
        }

        public ObservableCollection<Node> GetNodesCollection()
        {
            ObservableCollection<Node> _nodesCollection = new();
            MegaApiClient client = new MegaApiClient();
            Uri folderLink = new("https://mega.nz/folder/gdozjZxL#uI5SheetsAd-NYKMeRjf2A");
            client.LoginAnonymous();

            IEnumerable<CG.Web.MegaApiClient.INode> nodes;
            nodes = client.GetNodesFromLink(folderLink);

            CG.Web.MegaApiClient.INode root;
            root = nodes.Single(n => n.Type == NodeType.Root);

            Node rootNode = new Node() { NodeId = root.Id, Type = root.Type, Name = root.Name, Size = root.Size, CreationDate = (DateTime)root.CreationDate, IsSelected = false};
           
            GetNodesRecursive(rootNode, nodes, root);
            _nodesCollection.Add(rootNode);

            return _nodesCollection;


        }



        void GetNodesRecursive(Node rootNode , IEnumerable<CG.Web.MegaApiClient.INode> nodes, CG.Web.MegaApiClient.INode parent, int level = 0)
        {
            
            IEnumerable<CG.Web.MegaApiClient.INode> children = nodes.Where(x => x.ParentId == parent.Id);

            foreach (CG.Web.MegaApiClient.INode child in children)
            {
                Node _nextNode = new Node() { NodeId = child.Id, Type = child.Type, Name = child.Name, Size = child.Size, ParentId = child.ParentId, CreationDate = (DateTime)child.CreationDate };
                
                if (child.Type == NodeType.Directory)
                {
                    rootNode.Items??= new ObservableCollection<Node>();  
                    rootNode.Items.Add(_nextNode);
                    GetNodesRecursive(_nextNode, nodes, child, level + 1);
                       
                }
                else
                {
                    rootNode.Items??= new ObservableCollection<Node>();  
                    rootNode.Items.Add(new Node() { NodeId = child.Id, Type = child.Type, Name = child.Name, Size = child.Size, ParentId = child.ParentId, CreationDate = (DateTime)child.CreationDate });
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }

}

